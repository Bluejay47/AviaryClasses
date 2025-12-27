using AviaryClasses;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using Kingmaker.EntitySystem.Stats;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints.Classes;
using BlueprintCore.Blueprints.CustomConfigurators;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Mechanics.Properties;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.Utility;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Mechanics.Actions;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Actions.Builder;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils.Assets;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.ElementsSystem;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.RuleSystem;
using BlueprintCore.Actions.Builder.BasicEx;
using Kingmaker.Enums.Damage;
using Kingmaker.Designers.EventConditionActionSystem.Evaluators;
using BlueprintCore.Blueprints.Configurators.Items;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Designers;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using System;
using BlueprintCore.Actions.Builder.AVEx;


namespace AviaryClasses.Classes {


    public class OverpoweredVortex {

        private static readonly LogWrapper Logger = LogWrapper.Get("OverpoweredVortex");

        public static readonly string featName = "OverpoweredVortex";
        public static readonly string featGuid = "28f691e6-e61f-4f88-873d-8e593f68109d";

        public static readonly string buffName = "OverpoweredVortexBuff";
        public static readonly string buffGuid = "77190430-022e-4ce7-b144-d74ab929ddae";

        public static readonly string buff2Name = "VortexActivatedBuff";
        public static readonly string buff2Guid = "4dc7582d-001e-472e-9ded-7ccdfc164dd5";

        public static readonly string abilityName = "OverpoweredVortexAbility";
        public static readonly string abilityGuid = "0068c868-2786-406a-9576-b67bc0f1d539";

        private static readonly int[] VortexTargetCounts = [1, 2, 2, 2, 5];
        private const bool EnableVortexDebugLogging = true;

        private static void LogDebug(string message) {
            if (!EnableVortexDebugLogging) {
                return;
            }
            Logger.Info(message);
        }

        private static void LogDebugError(string message, Exception ex) {
            if (!EnableVortexDebugLogging) {
                return;
            }
            Logger.Error($"{message}: {ex}");
        }


        public static void Configure(LuckLevels luckLevel) {

            try {
                LogDebug($"AV:VORTEX_SETUP feature configure start luck={luckLevel} targets={VortexTargetCounts[(int)luckLevel]}");

                BlueprintFeature baseAbility = BlueprintTool.Get<BlueprintFeature>(FeatureRefs.BolsteredSpellFeat.ToString());
                var customIcon = Utils.LoadIcon("BolsteredVortex.png", baseAbility.m_Icon);

                // Hidden buff to track when the feature is active
                BuffConfigurator.New(buffName, buffGuid)
                .SetFlags(BlueprintBuff.Flags.HiddenInUi)
                .AddComponent<VortexBuffDebugComponent>(component => {
                    component.Prefix = "AV:VORTEX_TOGGLE_BUFF";
                    component.LuckLevel = (int)luckLevel;
                    component.TargetCount = VortexTargetCounts[(int)luckLevel];
                    component.FeatureBuffGuid = buffGuid;
                    component.CooldownBuffGuid = BuffRefs.ElementalVortexBuffCooldown.ToString();
                    component.RoundBuffGuid = buff2Guid;
                })
                .Configure();

                // Hidden buff with duration that is applied when BuffRefs.ElementalVortexBuffCooldown gets applied
                BuffConfigurator.New(buff2Name, buff2Guid)
                .SetFlags(BlueprintBuff.Flags.RemoveOnRest)
                .SetFlags(BlueprintBuff.Flags.RemoveOnResurrect)
                .AddRemoveBuffIfPartyNotInCombat()
                .SetFlags(BlueprintBuff.Flags.HiddenInUi)
                .AddComponent<VortexBuffDebugComponent>(component => {
                    component.Prefix = "AV:VORTEX_ROUND_BUFF";
                    component.LuckLevel = (int)luckLevel;
                    component.TargetCount = VortexTargetCounts[(int)luckLevel];
                    component.FeatureBuffGuid = buffGuid;
                    component.CooldownBuffGuid = BuffRefs.ElementalVortexBuffCooldown.ToString();
                    component.RoundBuffGuid = buff2Guid;
                })
                .Configure();

                // Works as expected
                var applyBuff2Action = ActionsBuilder.New()
                .AddAll(BuildVortexDebugAction("AV:VORTEX_FACT_GAINED", luckLevel, VortexTargetCounts[(int)luckLevel]).Build())
                .ApplyBuff(
                    buff2Guid,
                    durationValue: ContextDuration.Fixed(3, DurationRate.Rounds)
                )
                .AddAll(BuildVortexDebugAction("AV:VORTEX_BUFF2_APPLIED", luckLevel, VortexTargetCounts[(int)luckLevel]).Build());

                // Toggle ability for the feature
                var toggle = ActivatableAbilityConfigurator.New(abilityName, abilityGuid)
                .SetDisplayName(featName + ".Name")
                .SetDescription(featName + ".Description")
                .SetBuff(buffGuid)
                .SetIcon(customIcon)
                .Configure();

                // Main feature that grants the toggle
                FeatureConfigurator feature = FeatureConfigurator.New(featName, featGuid);

                feature.SetDescription(featName + ".Description")
                .SetDisplayName(featName + ".Name")
                .SetIsClassFeature(true)
                .AddFacts(new() { toggle })
                .SetIcon(customIcon)
                .AddFeatureTagsComponent(featureTags: FeatureTag.ClassSpecific | FeatureTag.Magic)
                .AddComponent<VortexFactDebugComponent>(component => {
                    component.Prefix = "AV:VORTEX_FEATURE";
                    component.LuckLevel = (int)luckLevel;
                    component.TargetCount = VortexTargetCounts[(int)luckLevel];
                    component.FeatureBuffGuid = buffGuid;
                    component.CooldownBuffGuid = BuffRefs.ElementalVortexBuffCooldown.ToString();
                    component.RoundBuffGuid = buff2Guid;
                })
                .AddFactsChangeTrigger(
                    [BuffRefs.ElementalVortexBuffCooldown.ToString()],
                    applyBuff2Action
                )
                .Configure();


                // Hook into Elemental Vortex to add cantrip casting
                VortexCantripEnhancement.Configure(luckLevel);

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }
        }

        private static ActionsBuilder BuildVortexDebugAction(string prefix, LuckLevels luckLevel, int targetCount) {
            return ActionsBuilder.New()
                .Add<VortexDebugAction>(action => {
                    action.Prefix = prefix;
                    action.LuckLevel = (int)luckLevel;
                    action.TargetCount = targetCount;
                    action.FeatureBuffGuid = buffGuid;
                    action.CooldownBuffGuid = BuffRefs.ElementalVortexBuffCooldown.ToString();
                    action.RoundBuffGuid = buff2Guid;
                });
        }

        private static ActionsBuilder BuildVortexTargetsDebugAction(string prefix, LuckLevels luckLevel, int targetCount, float radiusMeters) {
            return ActionsBuilder.New()
                .Add<VortexTargetsDebugAction>(action => {
                    action.Prefix = prefix;
                    action.LuckLevel = (int)luckLevel;
                    action.TargetCount = targetCount;
                    action.RadiusMeters = radiusMeters;
                });
        }

        private static ActionsBuilder BuildVortexAreaEffectAction(
            string prefix,
            ActionList actions,
            LuckLevels luckLevel,
            int targetCount,
            string nameFilter) {
            return ActionsBuilder.New()
                .Add<VortexAreaEffectAction>(action => {
                    action.Prefix = prefix;
                    action.Actions = actions;
                    action.LuckLevel = (int)luckLevel;
                    action.TargetCount = targetCount;
                    action.NameFilter = nameFilter;
                });
        }

        private static ActionsBuilder BuildVortexDamageDebugAction(string prefix, string energyType) {
            return ActionsBuilder.New()
                .Add<VortexDamageDebugAction>(action => {
                    action.Prefix = prefix;
                    action.EnergyType = energyType;
                });
        }


        internal class VortexCantripEnhancement {
            public static void Configure(LuckLevels luckLevel) {
                try {

                    int baseIndex = (int)luckLevel;
                    int fortuneIndex = Math.Min(baseIndex + 2, (int)LuckLevels.EXTREME);
                    int baseTargets = VortexTargetCounts[baseIndex];
                    int fortuneTargets = VortexTargetCounts[fortuneIndex];
                    LuckLevels fortuneLuckLevel = (LuckLevels)fortuneIndex;

                    LogDebug($"AV:VORTEX_SETUP cantrip configure luck={luckLevel} targets={baseTargets} fortuneLuck={fortuneLuckLevel} fortuneTargets={fortuneTargets}");

                    //Modification to Vortex to Fire Additional Attacks
                    ContextDiceValue splashDice = ContextDice.Value(Kingmaker.RuleSystem.DiceType.D3, 10, ContextValues.Property(UnitProperty.StatBonusIntelligence));

                    // Create individual action builders for each energy type
                    var electricityAction = ActionsBuilder.New()
                    .OnRandomTargetsAround(
                        actions: ActionsBuilder.New()
                            .AddAll(BuildVortexDamageDebugAction("AV:VORTEX_HIT", "Electricity").Build())
                            .DealDamage(
                                new DamageTypeDescription() {
                                    Type = DamageType.Energy,
                                    Energy = DamageEnergyType.Electricity
                                },
                                value: splashDice,
                                setFactAsReason: true,
                                addAdditionalDamage: true
                            )
                            .SpawnFx(ElementalEffects.Lightning),
                        onEnemies: true,
                        numberOfTargets: baseTargets,
                        radius: 30.Feet()
                    );

                    var electricityActionFortune = ActionsBuilder.New()
                    .OnRandomTargetsAround(
                        actions: ActionsBuilder.New()
                            .AddAll(BuildVortexDamageDebugAction("AV:VORTEX_HIT", "Electricity").Build())
                            .DealDamage(
                                new DamageTypeDescription() {
                                    Type = DamageType.Energy,
                                    Energy = DamageEnergyType.Electricity
                                },
                                value: splashDice,
                                setFactAsReason: true,
                                addAdditionalDamage: true
                            )
                            .SpawnFx(ElementalEffects.Lightning),
                        onEnemies: true,
                        numberOfTargets: fortuneTargets,
                        radius: 30.Feet()
                    );

                    var fireAction = ActionsBuilder.New()
                    .OnRandomTargetsAround(
                        actions: ActionsBuilder.New()
                            .AddAll(BuildVortexDamageDebugAction("AV:VORTEX_HIT", "Fire").Build())
                            .DealDamage(
                                new DamageTypeDescription() {
                                    Type = DamageType.Energy,
                                    Energy = DamageEnergyType.Fire
                                },
                                value: splashDice,
                                setFactAsReason: true,
                                addAdditionalDamage: true
                            )
                            .SpawnFx(ElementalEffects.Fire),
                        onEnemies: true,
                        numberOfTargets: baseTargets,
                        radius: 30.Feet()
                    );

                    var fireActionFortune = ActionsBuilder.New()
                    .OnRandomTargetsAround(
                        actions: ActionsBuilder.New()
                            .AddAll(BuildVortexDamageDebugAction("AV:VORTEX_HIT", "Fire").Build())
                            .DealDamage(
                                new DamageTypeDescription() {
                                    Type = DamageType.Energy,
                                    Energy = DamageEnergyType.Fire
                                },
                                value: splashDice,
                                setFactAsReason: true,
                                addAdditionalDamage: true
                            )
                            .SpawnFx(ElementalEffects.Fire),
                        onEnemies: true,
                        numberOfTargets: fortuneTargets,
                        radius: 30.Feet()
                    );

                    var coldAction = ActionsBuilder.New()
                    .OnRandomTargetsAround(
                        actions: ActionsBuilder.New()
                            .AddAll(BuildVortexDamageDebugAction("AV:VORTEX_HIT", "Cold").Build())
                            .DealDamage(
                                new DamageTypeDescription() {
                                    Type = DamageType.Energy,
                                    Energy = DamageEnergyType.Cold
                                },
                                value: splashDice,
                                setFactAsReason: true,
                                addAdditionalDamage: true
                            )
                            .SpawnFx(ElementalEffects.Cold),
                        onEnemies: true,
                        numberOfTargets: baseTargets,
                        radius: 30.Feet()
                    );

                    var coldActionFortune = ActionsBuilder.New()
                    .OnRandomTargetsAround(
                        actions: ActionsBuilder.New()
                            .AddAll(BuildVortexDamageDebugAction("AV:VORTEX_HIT", "Cold").Build())
                            .DealDamage(
                                new DamageTypeDescription() {
                                    Type = DamageType.Energy,
                                    Energy = DamageEnergyType.Cold
                                },
                                value: splashDice,
                                setFactAsReason: true,
                                addAdditionalDamage: true
                            )
                            .SpawnFx(ElementalEffects.Cold),
                        onEnemies: true,
                        numberOfTargets: fortuneTargets,
                        radius: 30.Feet()
                    );

                    var acidAction = ActionsBuilder.New()
                    .OnRandomTargetsAround(
                        actions: ActionsBuilder.New()
                            .AddAll(BuildVortexDamageDebugAction("AV:VORTEX_HIT", "Acid").Build())
                            .DealDamage(
                                new DamageTypeDescription() {
                                    Type = DamageType.Energy,
                                    Energy = DamageEnergyType.Acid
                                },
                                value: splashDice,
                                setFactAsReason: true,
                                addAdditionalDamage: true
                            )
                            .SpawnFx(ElementalEffects.Acid),
                        onEnemies: true,
                        numberOfTargets: baseTargets,
                        radius: 30.Feet()
                    );

                    var acidActionFortune = ActionsBuilder.New()
                    .OnRandomTargetsAround(
                        actions: ActionsBuilder.New()
                            .AddAll(BuildVortexDamageDebugAction("AV:VORTEX_HIT", "Acid").Build())
                            .DealDamage(
                                new DamageTypeDescription() {
                                    Type = DamageType.Energy,
                                    Energy = DamageEnergyType.Acid
                                },
                                value: splashDice,
                                setFactAsReason: true,
                                addAdditionalDamage: true
                            )
                            .SpawnFx(ElementalEffects.Acid),
                        onEnemies: true,
                        numberOfTargets: fortuneTargets,
                        radius: 30.Feet()
                    );

                    // Create random cantrip action using Randomize for runtime energy type selection
                    ActionList randomCantripAction = ActionsBuilder.New()
                    .Randomize(
                        (electricityAction, 1),
                        (fireAction, 1),
                        (coldAction, 1),
                        (acidAction, 1)
                    )
                    .Build();

                    ActionList randomCantripActionFortune = ActionsBuilder.New()
                    .Randomize(
                        (electricityActionFortune, 1),
                        (fireActionFortune, 1),
                        (coldActionFortune, 1),
                        (acidActionFortune, 1)
                    )
                    .Build();

                    // Condition to check if our buff is active and if Elemental Vortex is active
                    ConditionsBuilder cantripCondition = ConditionsBuilder.New()
                    .CasterHasFact(buffGuid)
                    .CasterHasFact(BuffRefs.ElementalVortexBuffCooldown.ToString())
                    .CasterHasFact(buff2Guid);

                    ConditionsBuilder fortuneCondition = ConditionsBuilder.New()
                    .CasterHasFact(BuffRefs.WitchHexFortuneBuff.ToString());

                    var debugAction = BuildVortexDebugAction("AV:VORTEX_TICK", luckLevel, baseTargets);
                    var targetsDebugAction = BuildVortexTargetsDebugAction("AV:VORTEX_TARGETS", luckLevel, baseTargets, 30.Feet().Meters);
                    var vortexActions = ActionsBuilder.New()
                        .AddAll(targetsDebugAction.Build())
                        .AddAll(randomCantripAction)
                        .Build();
                    var vortexAreaAction = BuildVortexAreaEffectAction(
                        "AV:VORTEX_AREA",
                        vortexActions,
                        luckLevel,
                        baseTargets,
                        "Vortex");

                    var targetsDebugActionFortune = BuildVortexTargetsDebugAction("AV:VORTEX_TARGETS", fortuneLuckLevel, fortuneTargets, 30.Feet().Meters);
                    var vortexActionsFortune = ActionsBuilder.New()
                        .AddAll(targetsDebugActionFortune.Build())
                        .AddAll(randomCantripActionFortune)
                        .Build();
                    var vortexAreaActionFortune = BuildVortexAreaEffectAction(
                        "AV:VORTEX_AREA",
                        vortexActionsFortune,
                        fortuneLuckLevel,
                        fortuneTargets,
                        "Vortex");

                    var vortexSelection = ActionsBuilder.New()
                        .Conditional(
                            conditions: fortuneCondition,
                            ifTrue: vortexAreaActionFortune,
                            ifFalse: vortexAreaAction
                        );

                    //  Add per round casts to Overpowered Vortex feature to add turn-based cantrip casting
                    FeatureConfigurator.For(featGuid.ToString())
                    .AddNewRoundTrigger(
                        newRoundActions: ActionsBuilder.New()
                            .AddAll(debugAction.Build())
                            .Conditional(
                                conditions: cantripCondition,
                                ifTrue: vortexSelection
                            )
                    )
                    .Configure();

                } catch (Exception ex) {
                    Logger.Error(ex.ToString());
                }
            }
        }

        [TypeId("69c014c3-6ef6-4e4b-8cab-97785e5b44e2")]
        private class VortexDebugAction : ContextAction {
            public string Prefix;
            public int LuckLevel;
            public int TargetCount;
            public string FeatureBuffGuid;
            public string CooldownBuffGuid;
            public string RoundBuffGuid;

            public override string GetCaption() {
                return "Overpowered Vortex Debug Log";
            }

            public override void RunAction() {
                try {
                    var context = Context;
                    var caster = context?.MaybeCaster;
                    var descriptor = caster?.Descriptor;
                    bool contextMissing = context == null;
                    bool casterMissing = caster == null;
                    var featureBuff = ResourcesLibrary.TryGetBlueprint<BlueprintFact>(FeatureBuffGuid);
                    var cooldownBuff = ResourcesLibrary.TryGetBlueprint<BlueprintFact>(CooldownBuffGuid);
                    var roundBuff = ResourcesLibrary.TryGetBlueprint<BlueprintFact>(RoundBuffGuid);
                    string casterName = caster?.CharacterName ?? "null";
                    string targetName = context?.MainTarget?.Unit?.CharacterName ?? "none";
                    bool hasFeatureBuff = descriptor != null && featureBuff != null && descriptor.HasFact(featureBuff);
                    bool hasCooldownBuff = descriptor != null && cooldownBuff != null && descriptor.HasFact(cooldownBuff);
                    bool hasRoundBuff = descriptor != null && roundBuff != null && descriptor.HasFact(roundBuff);
                    string blueprintName = context?.AssociatedBlueprint?.name ?? "none";

                    LogDebug($"{Prefix} blueprint={blueprintName} caster={casterName} target={targetName} luck={LuckLevel} targets={TargetCount} ctxNull={contextMissing} casterNull={casterMissing} featureBuff={hasFeatureBuff} cooldownBuff={hasCooldownBuff} roundBuff={hasRoundBuff}");
                } catch (Exception ex) {
                    LogDebugError($"{Prefix} failed", ex);
                }
            }
        }

        [TypeId("07b9a8e7-5cb7-4b1a-8d80-8f03f59b3d9a")]
        private class VortexAreaEffectAction : ContextAction {
            public string Prefix;
            public ActionList Actions;
            public int LuckLevel;
            public int TargetCount;
            public string NameFilter;

            public override string GetCaption() {
                return "Overpowered Vortex Area Effect Action";
            }

            public override void RunAction() {
                try {
                    var context = Context;
                    var caster = context?.MaybeCaster;
                    if (context == null || caster == null || Actions == null) {
                        LogDebug($"{Prefix} caster={(caster?.CharacterName ?? "null")} ctxNull={context == null} actionsNull={Actions == null}");
                        return;
                    }

                    int total = 0;
                    int matched = 0;
                    foreach (AreaEffectEntityData areaEffect in Game.Instance.State.AreaEffects) {
                        if (areaEffect.Context?.MaybeCaster != caster) {
                            continue;
                        }
                        total++;
                        string effectName = areaEffect.Blueprint?.name ?? "none";
                        string abilityName = areaEffect.Context?.SourceAbility?.name ?? "none";
                        bool matches = string.IsNullOrEmpty(NameFilter)
                            || effectName.IndexOf(NameFilter, StringComparison.OrdinalIgnoreCase) >= 0
                            || abilityName.IndexOf(NameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                        if (!matches) {
                            continue;
                        }
                        matched++;
                        using (AreaEffectContextData.Request().Setup(areaEffect)) {
                            using (var dataScope = context.GetDataScope(areaEffect.Position)) {
                                Actions.Run();
                            }
                        }
                    }

                    LogDebug($"{Prefix} caster={caster.CharacterName} luck={LuckLevel} targets={TargetCount} totalEffects={total} matched={matched} filter={NameFilter}");
                } catch (Exception ex) {
                    LogDebugError($"{Prefix} failed", ex);
                }
            }
        }

        [TypeId("2e0b2cff-6c1b-4c5e-9f1f-9f34d7c1580a")]
        private class VortexTargetsDebugAction : ContextAction {
            public string Prefix;
            public int LuckLevel;
            public int TargetCount;
            public float RadiusMeters;

            public override string GetCaption() {
                return "Overpowered Vortex Target Debug Log";
            }

            public override void RunAction() {
                try {
                    var context = Context;
                    var caster = context?.MaybeCaster;
                    var targetWrapper = Target ?? context?.MainTarget;
                    bool targetMissing = targetWrapper == null;
                    if (targetMissing) {
                        LogDebug($"{Prefix} caster={(caster?.CharacterName ?? "null")} target=none luck={LuckLevel} targets={TargetCount} radiusMeters={RadiusMeters} total=0 alive=0 enemy=0 aliveEnemy=0");
                        return;
                    }
                    var targetPoint = targetWrapper.Point;
                    string casterName = caster?.CharacterName ?? "null";
                    string targetUnitName = targetWrapper.Unit?.CharacterName ?? "none";
                    int total = 0;
                    int alive = 0;
                    int enemy = 0;
                    int aliveEnemy = 0;

                    foreach (var unit in GameHelper.GetTargetsAround(targetPoint, RadiusMeters, true, false)) {
                        total++;
                        if (!unit.Descriptor.State.IsDead) {
                            alive++;
                        }
                        if (caster != null && unit.IsEnemy(caster)) {
                            enemy++;
                            if (!unit.Descriptor.State.IsDead) {
                                aliveEnemy++;
                            }
                        }
                    }

                    LogDebug($"{Prefix} caster={casterName} target={targetUnitName} luck={LuckLevel} targets={TargetCount} radiusMeters={RadiusMeters} total={total} alive={alive} enemy={enemy} aliveEnemy={aliveEnemy}");
                } catch (Exception ex) {
                    LogDebugError($"{Prefix} failed", ex);
                }
            }
        }

        [TypeId("9f2a2e8b-35aa-4c36-b28e-3c44d4a4b57f")]
        private class VortexDamageDebugAction : ContextAction {
            public string Prefix;
            public string EnergyType;

            public override string GetCaption() {
                return "Overpowered Vortex Damage Debug Log";
            }

            public override void RunAction() {
                try {
                    var context = Context;
                    var caster = context?.MaybeCaster;
                    var target = Target?.Unit;
                    string casterName = caster?.CharacterName ?? "null";
                    string targetName = target?.CharacterName ?? "none";
                    string blueprintName = context?.AssociatedBlueprint?.name ?? "none";
                    string isEnemy = (caster != null && target != null) ? target.IsEnemy(caster).ToString() : "unknown";

                    LogDebug($"{Prefix} energy={EnergyType} caster={casterName} target={targetName} isEnemy={isEnemy} blueprint={blueprintName}");
                } catch (Exception ex) {
                    LogDebugError($"{Prefix} failed", ex);
                }
            }
        }

        [TypeId("c7f266c8-7d1b-4e8b-9c49-037a9f11c256")]
        private class VortexFactDebugComponent : UnitFactComponentDelegate {
            public string Prefix;
            public int LuckLevel;
            public int TargetCount;
            public string FeatureBuffGuid;
            public string CooldownBuffGuid;
            public string RoundBuffGuid;

            public override void OnActivate() {
                LogState("ACTIVATE");
            }

            public override void OnDeactivate() {
                LogState("DEACTIVATE");
            }

            public override void OnTurnOn() {
                LogState("TURN_ON");
            }

            public override void OnTurnOff() {
                LogState("TURN_OFF");
            }

            private void LogState(string state) {
                try {
                    var owner = Owner;
                    var descriptor = owner?.Descriptor;
                    var featureBuff = ResourcesLibrary.TryGetBlueprint<BlueprintFact>(FeatureBuffGuid);
                    var cooldownBuff = ResourcesLibrary.TryGetBlueprint<BlueprintFact>(CooldownBuffGuid);
                    var roundBuff = ResourcesLibrary.TryGetBlueprint<BlueprintFact>(RoundBuffGuid);
                    bool hasFeatureBuff = descriptor != null && featureBuff != null && descriptor.HasFact(featureBuff);
                    bool hasCooldownBuff = descriptor != null && cooldownBuff != null && descriptor.HasFact(cooldownBuff);
                    bool hasRoundBuff = descriptor != null && roundBuff != null && descriptor.HasFact(roundBuff);
                    string ownerName = owner?.CharacterName ?? "null";
                    string factName = Fact?.Blueprint?.name ?? "none";

                    LogDebug($"{Prefix}:{state} owner={ownerName} fact={factName} luck={LuckLevel} targets={TargetCount} featureBuff={hasFeatureBuff} cooldownBuff={hasCooldownBuff} roundBuff={hasRoundBuff}");
                } catch (Exception ex) {
                    LogDebugError($"{Prefix}:{state} failed", ex);
                }
            }
        }

        [TypeId("e7f85f32-7769-4d9d-8f0c-5c63f2ce977c")]
        private class VortexBuffDebugComponent : UnitFactComponentDelegate {
            public string Prefix;
            public int LuckLevel;
            public int TargetCount;
            public string FeatureBuffGuid;
            public string CooldownBuffGuid;
            public string RoundBuffGuid;

            public override void OnActivate() {
                LogState("ACTIVATE");
            }

            public override void OnDeactivate() {
                LogState("DEACTIVATE");
            }

            private void LogState(string state) {
                try {
                    var owner = Owner;
                    var descriptor = owner?.Descriptor;
                    var featureBuff = ResourcesLibrary.TryGetBlueprint<BlueprintFact>(FeatureBuffGuid);
                    var cooldownBuff = ResourcesLibrary.TryGetBlueprint<BlueprintFact>(CooldownBuffGuid);
                    var roundBuff = ResourcesLibrary.TryGetBlueprint<BlueprintFact>(RoundBuffGuid);
                    bool hasFeatureBuff = descriptor != null && featureBuff != null && descriptor.HasFact(featureBuff);
                    bool hasCooldownBuff = descriptor != null && cooldownBuff != null && descriptor.HasFact(cooldownBuff);
                    bool hasRoundBuff = descriptor != null && roundBuff != null && descriptor.HasFact(roundBuff);
                    string ownerName = owner?.CharacterName ?? "null";
                    string factName = Fact?.Blueprint?.name ?? "none";

                    LogDebug($"{Prefix}:{state} owner={ownerName} fact={factName} luck={LuckLevel} targets={TargetCount} featureBuff={hasFeatureBuff} cooldownBuff={hasCooldownBuff} roundBuff={hasRoundBuff}");
                } catch (Exception ex) {
                    LogDebugError($"{Prefix}:{state} failed", ex);
                }
            }
        }

    }
}


