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
using Kingmaker.Blueprints;
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

        public static void Configure() {

            try {

                BlueprintFeature baseAbility = BlueprintTool.Get<BlueprintFeature>(FeatureRefs.BolsteredSpellFeat.ToString());
                var customIcon = Utils.LoadIcon("BolsteredVortex.png", baseAbility.m_Icon);

                // Hidden buff to track when the feature is active
                BuffConfigurator.New(buffName, buffGuid)
                .SetFlags(BlueprintBuff.Flags.HiddenInUi)
                .Configure();

                // Hidden buff with duration that is applied when BuffRefs.ElementalVortexBuffCooldown gets applied
                BuffConfigurator.New(buff2Name, buff2Guid)
                .SetFlags(BlueprintBuff.Flags.RemoveOnRest)
                .SetFlags(BlueprintBuff.Flags.RemoveOnResurrect)
                .AddRemoveBuffIfPartyNotInCombat()
                .SetFlags(BlueprintBuff.Flags.HiddenInUi)

                .Configure();

                // Works as expected
                var applyBuff2Action = ActionsBuilder.New()
                .ApplyBuff(
                    buff2Guid,
                    durationValue: ContextDuration.Fixed(3, DurationRate.Rounds)
                );    

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
                .AddFactsChangeTrigger(
                    [BuffRefs.ElementalVortexBuffCooldown.ToString()],
                    applyBuff2Action
                )
                .Configure();
                

                // Hook into Elemental Vortex to add cantrip casting
                VortexCantripEnhancement.Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }
        }

        internal class VortexCantripEnhancement {
            public static void Configure() {
                try {

                    //Modification to Vortex to Fire Additional Attacks
                    ContextDiceValue splashDice = ContextDice.Value(Kingmaker.RuleSystem.DiceType.D3, 10, ContextValues.Property(UnitProperty.StatBonusIntelligence));

                    // Random number of targets for this session (2-6)
                    Random random = new Random();
                    int numberOfTargets = random.Next(2, 7); // 2 to 6 inclusive, where are taking advantage of the fact that this value varies per game session

                    // Create individual action builders for each energy type
                    var electricityAction = ActionsBuilder.New()
                    .OnRandomTargetsAround(
                        actions: ActionsBuilder.New()
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
                        numberOfTargets: numberOfTargets,
                        radius: 30.Feet()
                    );

                    var fireAction = ActionsBuilder.New()
                    .OnRandomTargetsAround(
                        actions: ActionsBuilder.New()
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
                        numberOfTargets: numberOfTargets,
                        radius: 30.Feet()
                    );

                    var coldAction = ActionsBuilder.New()
                    .OnRandomTargetsAround(
                        actions: ActionsBuilder.New()
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
                        numberOfTargets: numberOfTargets,
                        radius: 30.Feet()
                    );

                    var acidAction = ActionsBuilder.New()
                    .OnRandomTargetsAround(
                        actions: ActionsBuilder.New()
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
                        numberOfTargets: numberOfTargets,
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

                    // Condition to check if our buff is active and if Elemental Vortex is active
                    ConditionsBuilder cantripCondition = ConditionsBuilder.New()
                    .CasterHasFact(buffGuid)
                    .CasterHasFact(BuffRefs.ElementalVortexBuffCooldown.ToString())
                    .CasterHasFact(buff2Guid);

                    //  Add per round casts to Overpowered Vortex feature to add turn-based cantrip casting
                    FeatureConfigurator.For(featGuid.ToString())
                    .AddNewRoundTrigger(
                        newRoundActions: ActionsBuilder.New()
                            .Conditional(
                                conditions: cantripCondition,
                                ifTrue: randomCantripAction
                            )
                    )
                    .Configure();

                } catch (Exception ex) {
                    Logger.Error(ex.ToString());
                }
            }
        }

    }
}





