using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Blueprints;
using AviaryClasses.Classes.Features;
using System;
using System.Linq;
using Kingmaker.EntitySystem.Stats;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators;
using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils.Types;
using Kingmaker.Enums;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;

namespace AviaryClasses.Classes {
    public class BeastTamer2 {
        private static readonly LogWrapper Logger = LogWrapper.Get("BeastTamer2");

        public static BlueprintArchetype archetypeRef;
        public static readonly string featName = "BeastTamer2";
        public static readonly string featGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac2eee";

        // Pet-only Inspire Greatness
        private static readonly string petInspireGreatnessName = "BeastTamerInspireGreatness";
        private static readonly string petInspireGreatnessGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac2efd";
        private static readonly string petInspireGreatnessAreaName = "BeastTamerInspireGreatnessArea";
        private static readonly string petInspireGreatnessAreaGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac2efe";
        private static readonly string petInspireGreatnessBuffName = "BeastTamerInspireGreatnessBuff";
        private static readonly string petInspireGreatnessBuffGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac2fff";
        private static readonly string petInspireGreatnessToggleName = "BeastTamerInspireGreatnessToggle";
        private static readonly string petInspireGreatnessToggleGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac3000";

        // Pet-only Inspire Heroics
        private static readonly string petInspireHeroicsName = "BeastTamerInspireHeroics";
        private static readonly string petInspireHeroicsGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac3001";
        private static readonly string petInspireHeroicsAreaName = "BeastTamerInspireHeroicsArea";
        private static readonly string petInspireHeroicsAreaGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac3002";
        private static readonly string petInspireHeroicsBuffName = "BeastTamerInspireHeroicsBuff";
        private static readonly string petInspireHeroicsBuffGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac3003";
        private static readonly string petInspireHeroicsToggleName = "BeastTamerInspireHeroicsToggle";
        private static readonly string petInspireHeroicsToggleGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac3004";

        // Pet-only Soothing Performance
        private static readonly string petSoothingPerformanceName = "BeastTamerSoothingPerformance";
        private static readonly string petSoothingPerformanceGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac3005";
        private static readonly string petSoothingPerformanceFeatureName = "BeastTamerSoothingPerformanceFeature";
        private static readonly string petSoothingPerformanceFeatureGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac300c";

        // Bonus Talent Selection
        private static readonly string bonusTalentSelectionName = "BeastTamerBonusTalentSelection";
        private static readonly string bonusTalentSelectionGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac3006";

        // Beast Master's Bond (mimics Nature Mystery for prerequisite compatibility)
        private static readonly string beastMasterBondName = "BeastMasterBond";
        private static readonly string beastMasterBondGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac3007";

        public static void Configure() {
            try {
                // Create the Release The Bees spell
                Features.ReleaseTheBees.Configure();

                // Create Friend to Animals feature
                Features.FriendToAnimals.Configure();

                // Create Pack Tactics feature (shares teamwork feats with pets and summons)
                Features.PackTactics.Configure();

                // Create Beast Master's Bond feature (allows selecting nature revelations)
                ConfigureBeastMasterBond();

                // Create pet-only performance features
                ConfigurePetOnlyInspireGreatness();
                ConfigurePetOnlyInspireHeroics();
                ConfigurePetOnlySoothingPerformance();

                // Create bonus talent selection
                ConfigureBonusTalentSelection();

                // Create the archetype
                ArchetypeConfigurator archetype = ArchetypeConfigurator.New(featName, featGuid, CharacterClassRefs.BardClass)
                .CopyFrom(ArchetypeRefs.BeastTamerArchetype)
                .SetLocalizedName(featName + ".Name")
                .SetLocalizedDescription(featName + ".Description")
                .SetOverrideAttributeRecommendations(true)
                .SetRecommendedAttributes(
                    StatType.Charisma,
                    StatType.Dexterity
                );

                // Configure archetype progression
                ConfigureArchetypeProgression(archetype);

                archetypeRef = archetype.Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }
        }


        private static void ConfigureArchetypeProgression(ArchetypeConfigurator archetype) {

            // Level 5-20: Animal Companion Ranks
            for (int level = 5; level <= 20; level++) {
                archetype.AddToAddFeatures(level, FeatureRefs.AnimalCompanionRank.ToString());
            }

            // Remove the party-wide buff features and replace with pet-only versions
            archetype

                // Level 1: Add Beast Master's Bond (allows selecting nature-themed talents)
                .AddToAddFeatures(1, beastMasterBondGuid)

                // Add Release The Bees spell to the spellbook at level 1
                .AddToAddFeatures(1, Features.ReleaseTheBees.CreateReleaseTheBeesFeature())

                // Add animal companion starting at level 4
                .AddToAddFeatures(4, FeatureSelectionRefs.AnimalCompanionSelectionDruid.ToString())

                // level 8
                .AddToAddFeatures(8, bonusTalentSelectionGuid)

                // Level 9: Replace Inspire Greatness with pet-only version
                .AddToRemoveFeatures(9, FeatureRefs.InspireGreatnessFeature.ToString())
                .AddToAddFeatures(9, petInspireGreatnessGuid)

                .AddToAddFeatures(11, FeatureRefs.SuperiorSummoning.ToString())

                // Level 12: Replace Soothing Performance with pet-only version, add Pack Tactics
                .AddToRemoveFeatures(12, FeatureRefs.SoothingPerformanceFeature.ToString())
                .AddToAddFeatures(12, petSoothingPerformanceFeatureGuid)
                .AddToAddFeatures(12, Features.PackTactics.featureGuid)
                .AddToAddFeatures(12, Features.PackTactics.selectionGuid)

                // Level 15: Replace Inspire Heroics with pet-only version
                .AddToRemoveFeatures(15, FeatureRefs.InspireHeroicsFeature.ToString())
                .AddToAddFeatures(15, petInspireHeroicsGuid)

                // level 16
                .AddToAddFeatures(16, bonusTalentSelectionGuid);
        }


        private static void ConfigurePetOnlyInspireGreatness() {
            var originalToggle = BlueprintTool.Get<BlueprintActivatableAbility>(ActivatableAbilityRefs.InspireGreatnessToggleAbility.ToString());
            var originalBuff = BlueprintTool.Get<Kingmaker.UnitLogic.Buffs.Blueprints.BlueprintBuff>(BuffRefs.InspireGreatnessBuff.ToString());
            var originalArea = BlueprintTool.Get<BlueprintAbilityAreaEffect>(AbilityAreaEffectRefs.InspireGreatnessArea.ToString());

            var petOnlyTypeCondition = ConditionsBuilder.New()
                .UseOr()
                .Add<Kingmaker.UnitLogic.Mechanics.Conditions.ContextConditionIsAnimalCompanion>()
                .HasFact(BuffRefs.SummonedUnitBuff.ToString());

            var petOnlyCondition = ConditionsBuilder.New()
                .Add<Kingmaker.UnitLogic.Mechanics.Conditions.ContextConditionIsAlly>()
                .AddOrAndLogic(petOnlyTypeCondition);

            var petOnlyDiscordantCondition = ConditionsBuilder.New()
                .Add<Kingmaker.UnitLogic.Mechanics.Conditions.ContextConditionIsAlly>()
                .AddOrAndLogic(petOnlyTypeCondition)
                .CasterHasFact(FeatureRefs.DiscordantVoice.ToString());

            var areaEffect = AbilityAreaEffectConfigurator.New(petInspireGreatnessAreaName, petInspireGreatnessAreaGuid)
                .SetTargetType(BlueprintAbilityAreaEffect.TargetType.Any)
                .SetSpellResistance(false)
                .SetAffectEnemies(false)
                .SetAggroEnemies(true)
                .SetAffectDead(false)
                .SetIgnoreSleepingUnits(false)
                .SetShape(AreaEffectShape.Cylinder)
                .SetSize(30.Feet())
                .SetFx(originalArea.Fx)
                .AddAbilityAreaEffectBuff(
                    buff: BuffRefs.InspireGreatnessEffectBuff.ToString(),
                    checkConditionEveryRound: false,
                    condition: petOnlyCondition)
                .AddAbilityAreaEffectBuff(
                    buff: BuffRefs.DiscordantVoiceBuff.ToString(),
                    checkConditionEveryRound: false,
                    condition: petOnlyDiscordantCondition)
                .Configure();

            var bonusDice = new ContextDiceValue() {
                DiceType = DiceType.D10,
                DiceCountValue = ContextValues.Constant(2),
                BonusValue = ContextValues.Constant(0)
            };

            var buff = BuffConfigurator.New(petInspireGreatnessBuffName, petInspireGreatnessBuffGuid)
                .SetDisplayName(petInspireGreatnessName + ".Name")
                .SetDescription(petInspireGreatnessName + ".Description")
                .SetIcon(originalBuff.m_Icon)
                .SetFlags(BlueprintBuff.Flags.HiddenInUi | BlueprintBuff.Flags.StayOnDeath)
                .SetStacking(StackingType.Replace)
                .SetFrequency(DurationRate.Rounds)
                .SetFxOnStart(originalBuff.FxOnStart)
                .AddAreaEffect(areaEffect)
                .AddContextCalculateSharedValue(
                    valueType: AbilitySharedValue.StatBonus,
                    value: bonusDice,
                    modifier: 1.0)
                .AddAbilityUseTrigger(
                    action: ActionsBuilder.New()
                        .Conditional(
                            conditions: ConditionsBuilder.New()
                                .CasterHasFact(FeatureRefs.HarmonicSpell.ToString()),
                            ifTrue: ActionsBuilder.New()
                                .RestoreResource(AbilityResourceRefs.BardicPerformanceResource.ToString())),
                    actionsOnAllTargets: false,
                    actionsOnTarget: false,
                    afterCast: false,
                    checkAbilityType: true,
                    type: AbilityType.Spell,
                    checkAoE: false,
                    checkDescriptor: false,
                    checkRange: false,
                    checkSpellSchool: false,
                    checkSourceItemType: false,
                    fromSpellbook: false,
                    minSpellLevel: true,
                    minSpellLevelLimit: 1,
                    exactSpellLevel: false,
                    exactSpellLevelLimit: 0,
                    useCastRule: true,
                    onlyOnce: true,
                    oncePerContext: true,
                    range: AbilityRange.Touch,
                    isAoE: false)
                .Configure();

            var toggle = ActivatableAbilityConfigurator.New(petInspireGreatnessToggleName, petInspireGreatnessToggleGuid)
                .SetDisplayName(petInspireGreatnessName + ".Name")
                .SetDescription(petInspireGreatnessName + ".Description")
                .SetIcon(originalToggle.m_Icon)
                .SetBuff(buff)
                .SetGroup(ActivatableAbilityGroup.BardicPerformance)
                .SetWeightInGroup(1)
                .SetIsOnByDefault(false)
                .SetDeactivateIfCombatEnded(false)
                .SetDeactivateAfterFirstRound(false)
                .SetDeactivateImmediately(false)
                .SetIsTargeted(false)
                .SetDeactivateIfOwnerDisabled(true)
                .SetDeactivateIfOwnerUnconscious(false)
                .SetOnlyInCombat(false)
                .SetDoNotTurnOffOnRest(false)
                .SetActionBarAutoFillIgnored(false)
                .SetIsRuntimeOnly(false)
                .SetHiddenInUI(false)
                .SetActivationType(AbilityActivationType.WithUnitCommand)
                .SetActivateWithUnitCommand(UnitCommand.CommandType.Standard)
                .SetActivateOnUnitAction(AbilityActivateOnUnitActionType.Attack)
                .AddActivatableAbilityResourceLogic(
                    requiredResource: AbilityResourceRefs.BardicPerformanceResource.ToString(),
                    spendType: ActivatableAbilityResourceLogic.ResourceSpendType.NewRound)
                .AddTriggerOnActivationChanged(
                    actionList: ActionsBuilder.New()
                        .Conditional(
                            conditions: ConditionsBuilder.New()
                                .CasterHasFact(BuffRefs.DivaStyleBuff.ToString()),
                            ifTrue: ActionsBuilder.New()
                                .ApplyBuff(
                                    BuffRefs.DivaStyleFreeFaintBuff.ToString(),
                                    durationValue: ContextDuration.Fixed(1, DurationRate.Rounds),
                                    asChild: true,
                                    toCaster: true)),
                    stage: AddTriggerOnActivationChanged.Stage.OnSwitchOn)
                .SetResourceAssetIds(
                    "c87c798cd0a410c419ee4bafd4adb68f",
                    "3a0228650295f6a40bc335385a929a07")
                .Configure();

            FeatureConfigurator.New(petInspireGreatnessName, petInspireGreatnessGuid)
                .SetDisplayName(petInspireGreatnessName + ".Name")
                .SetDescription(petInspireGreatnessName + ".Description")
                .SetIcon(originalToggle.m_Icon)
                .SetIsClassFeature(true)
                .SetRanks(5)
                .AddFacts(new() { toggle })
                .Configure();
        }


        private static void ConfigurePetOnlyInspireHeroics() {
            var originalToggle = BlueprintTool.Get<BlueprintActivatableAbility>(ActivatableAbilityRefs.InspireHeroicsToggleAbility.ToString());
            var originalBuff = BlueprintTool.Get<Kingmaker.UnitLogic.Buffs.Blueprints.BlueprintBuff>(BuffRefs.InspireHeroicsBuff.ToString());
            var originalArea = BlueprintTool.Get<BlueprintAbilityAreaEffect>(AbilityAreaEffectRefs.InspireHeroicsArea.ToString());

            var petOnlyTypeCondition = ConditionsBuilder.New()
                .UseOr()
                .Add<Kingmaker.UnitLogic.Mechanics.Conditions.ContextConditionIsAnimalCompanion>()
                .HasFact(BuffRefs.SummonedUnitBuff.ToString());

            var petOnlyCondition = ConditionsBuilder.New()
                .Add<Kingmaker.UnitLogic.Mechanics.Conditions.ContextConditionIsAlly>()
                .AddOrAndLogic(petOnlyTypeCondition);

            var petOnlyDiscordantCondition = ConditionsBuilder.New()
                .Add<Kingmaker.UnitLogic.Mechanics.Conditions.ContextConditionIsAlly>()
                .AddOrAndLogic(petOnlyTypeCondition)
                .CasterHasFact(FeatureRefs.DiscordantVoice.ToString());

            var areaEffect = AbilityAreaEffectConfigurator.New(petInspireHeroicsAreaName, petInspireHeroicsAreaGuid)
                .SetTargetType(BlueprintAbilityAreaEffect.TargetType.Any)
                .SetSpellResistance(false)
                .SetAffectEnemies(false)
                .SetAggroEnemies(true)
                .SetAffectDead(false)
                .SetIgnoreSleepingUnits(false)
                .SetShape(AreaEffectShape.Cylinder)
                .SetSize(30.Feet())
                .SetFx(originalArea.Fx)
                .AddAbilityAreaEffectRunAction(
                    unitEnter: ActionsBuilder.New()
                        .Conditional(
                            conditions: petOnlyCondition,
                            ifTrue: ActionsBuilder.New()
                                .ApplyBuffPermanent(BuffRefs.InspireHeroicsEffectBuff.ToString(), asChild: true))
                        .Conditional(
                            conditions: petOnlyDiscordantCondition,
                            ifTrue: ActionsBuilder.New()
                                .ApplyBuffPermanent(BuffRefs.DiscordantVoiceBuff.ToString(), asChild: true)),
                    unitExit: ActionsBuilder.New()
                        .Conditional(
                            conditions: petOnlyCondition,
                            ifTrue: ActionsBuilder.New()
                                .RemoveBuff(BuffRefs.DLC3_InspireHeroicsEffectBuff.ToString(), onlyFromCaster: true)
                                .RemoveBuff(BuffRefs.InspireHeroicsEffectBuff.ToString(), onlyFromCaster: true)
                                .RemoveBuff(BuffRefs.DiscordantVoiceBuff.ToString(), onlyFromCaster: true)))
                .Configure();

            var buff = BuffConfigurator.New(petInspireHeroicsBuffName, petInspireHeroicsBuffGuid)
                .SetDisplayName(petInspireHeroicsName + ".Name")
                .SetDescription(petInspireHeroicsName + ".Description")
                .SetIcon(originalBuff.m_Icon)
                .SetFlags(BlueprintBuff.Flags.HiddenInUi | BlueprintBuff.Flags.StayOnDeath)
                .SetStacking(StackingType.Replace)
                .SetFrequency(DurationRate.Rounds)
                .SetFxOnStart(originalBuff.FxOnStart)
                .AddAreaEffect(areaEffect)
                .AddAbilityUseTrigger(
                    action: ActionsBuilder.New()
                        .Conditional(
                            conditions: ConditionsBuilder.New()
                                .CasterHasFact(FeatureRefs.HarmonicSpell.ToString()),
                            ifTrue: ActionsBuilder.New()
                                .RestoreResource(AbilityResourceRefs.BardicPerformanceResource.ToString())),
                    actionsOnAllTargets: false,
                    actionsOnTarget: false,
                    afterCast: false,
                    checkAbilityType: true,
                    type: AbilityType.Spell,
                    checkAoE: false,
                    checkDescriptor: false,
                    checkRange: false,
                    checkSpellSchool: false,
                    checkSourceItemType: false,
                    fromSpellbook: false,
                    minSpellLevel: true,
                    minSpellLevelLimit: 1,
                    exactSpellLevel: false,
                    exactSpellLevelLimit: 0,
                    useCastRule: true,
                    onlyOnce: true,
                    oncePerContext: true,
                    range: AbilityRange.Touch,
                    isAoE: false)
                .Configure();

            var toggle = ActivatableAbilityConfigurator.New(petInspireHeroicsToggleName, petInspireHeroicsToggleGuid)
                .SetDisplayName(petInspireHeroicsName + ".Name")
                .SetDescription(petInspireHeroicsName + ".Description")
                .SetIcon(originalToggle.m_Icon)
                .SetBuff(buff)
                .SetGroup(ActivatableAbilityGroup.BardicPerformance)
                .SetWeightInGroup(1)
                .SetIsOnByDefault(false)
                .SetDeactivateIfCombatEnded(false)
                .SetDeactivateAfterFirstRound(false)
                .SetDeactivateImmediately(false)
                .SetIsTargeted(false)
                .SetDeactivateIfOwnerDisabled(true)
                .SetDeactivateIfOwnerUnconscious(false)
                .SetOnlyInCombat(false)
                .SetDoNotTurnOffOnRest(false)
                .SetActionBarAutoFillIgnored(false)
                .SetIsRuntimeOnly(false)
                .SetHiddenInUI(false)
                .SetActivationType(AbilityActivationType.WithUnitCommand)
                .SetActivateWithUnitCommand(UnitCommand.CommandType.Standard)
                .SetActivateOnUnitAction(AbilityActivateOnUnitActionType.Attack)
                .AddActivatableAbilityResourceLogic(
                    requiredResource: AbilityResourceRefs.BardicPerformanceResource.ToString(),
                    spendType: ActivatableAbilityResourceLogic.ResourceSpendType.NewRound)
                .AddTriggerOnActivationChanged(
                    actionList: ActionsBuilder.New()
                        .Conditional(
                            conditions: ConditionsBuilder.New()
                                .CasterHasFact(BuffRefs.DivaStyleBuff.ToString()),
                            ifTrue: ActionsBuilder.New()
                                .ApplyBuff(
                                    BuffRefs.DivaStyleFreeFaintBuff.ToString(),
                                    durationValue: ContextDuration.Fixed(1, DurationRate.Rounds),
                                    asChild: true,
                                    toCaster: true)),
                    stage: AddTriggerOnActivationChanged.Stage.OnSwitchOn)
                .SetResourceAssetIds(
                    "c87c798cd0a410c419ee4bafd4adb68f",
                    "79665f3d500fdf44083feccf4cbfc00a",
                    "c7e1609eb3da9f446bc4a69622c486dc")
                .Configure();

            FeatureConfigurator.New(petInspireHeroicsName, petInspireHeroicsGuid)
                .SetDisplayName(petInspireHeroicsName + ".Name")
                .SetDescription(petInspireHeroicsName + ".Description")
                .SetIcon(originalToggle.m_Icon)
                .SetIsClassFeature(true)
                .SetRanks(5)
                .AddFacts(new() { toggle })
                .Configure();
        }


        private static void ConfigurePetOnlySoothingPerformance() {
            // Get the original icon from SoothingPerformanceAbility
            var originalAbility = BlueprintTool.Get<Kingmaker.UnitLogic.Abilities.Blueprints.BlueprintAbility>(AbilityRefs.SoothingPerformanceAbility.ToString());

            // Create a new pet-only Soothing Performance ability from scratch
            AbilityConfigurator.New(petSoothingPerformanceName, petSoothingPerformanceGuid)
                .CopyFrom(AbilityRefs.SoothingPerformanceAbility, c => true)
                .SetDisplayName(petSoothingPerformanceName + ".Name")
                .SetDescription(petSoothingPerformanceName + ".Description")
                .SetIcon(originalAbility.m_Icon)
                .OnConfigure(bp => {
                    // Find AbilityTargetsAround component and modify conditions
                    var targetsComp = bp.ComponentsArray.OfType<Kingmaker.UnitLogic.Abilities.Components.AbilityTargetsAround>().FirstOrDefault();
                    if (targetsComp != null) {
                        // Replace condition to only affect animal companions (pets)
                        targetsComp.m_Condition = ConditionsBuilder.New()
                            .Add<Kingmaker.UnitLogic.Mechanics.Conditions.ContextConditionIsAlly>()
                            .AddOrAndLogic(
                                ConditionsBuilder.New()
                                    .Add<Kingmaker.UnitLogic.Mechanics.Conditions.ContextConditionIsAnimalCompanion>()
                            )
                            .Build();
                    }
                })
                .Configure();

            // Create feature that grants the pet-only ability
            FeatureConfigurator.New(petSoothingPerformanceFeatureName, petSoothingPerformanceFeatureGuid)
                .SetDisplayName(petSoothingPerformanceFeatureName + ".Name")
                .SetDescription(petSoothingPerformanceFeatureName + ".Description")
                .SetIcon(originalAbility.m_Icon)
                .SetIsClassFeature(true)
                .AddFacts(new() { petSoothingPerformanceGuid })
                .AddPrerequisiteClassLevel(CharacterClassRefs.BardClass.ToString(), 12)
                .Configure();
        }


        private static void ConfigureBeastMasterBond() {
            // Create a feature that grants Oracle Nature Mystery for revelation prerequisites
            // This allows Beast Tamer to select Oracle Nature revelations as bonus talents
            FeatureConfigurator.New(beastMasterBondName, beastMasterBondGuid)
                .SetDisplayName(beastMasterBondName + ".Name")
                .SetDescription(beastMasterBondName + ".Description")
                .SetIsClassFeature(true)
                .AddFacts(new() { FeatureRefs.OracleNatureMysteryFeature.ToString() })
                .Configure();
        }


        private static void ConfigureBonusTalentSelection() {
            FeatureSelectionConfigurator featureSelection = FeatureSelectionConfigurator.New(bonusTalentSelectionName, bonusTalentSelectionGuid)
                .CopyFrom(FeatureSelectionRefs.BasicFeatSelection);

            featureSelection.SetDisplayName(bonusTalentSelectionName + ".Name");
            featureSelection.SetDescription(bonusTalentSelectionName + ".Description");

            featureSelection.ClearAllFeatures();

            // Beast Tamer custom features
            featureSelection.AddToAllFeatures(FriendToAnimals.featureGuid); // Friend to Animals (custom version)

            // Oracle Nature Mystery Revelations
            featureSelection.AddToAllFeatures(FeatureRefs.OracleRevelationNatureWhispers.ToString()); // Nature Whispers
            featureSelection.AddToAllFeatures(FeatureRefs.OracleRevelationSpiritOfNature.ToString()); // Spirit of Nature
            featureSelection.AddToAllFeatures(FeatureRefs.OracleRevelationErosionTouch.ToString()); // Spirit of Nature
            featureSelection.AddToAllFeatures(FeatureRefs.CompanionBoon.ToString()); // Boon Companion

            featureSelection.Configure();
        }
    }
}
