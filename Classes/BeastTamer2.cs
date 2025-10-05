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
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;

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
            // Level 1: Add Beast Master's Bond (allows selecting nature-themed talents)
            archetype.AddToAddFeatures(1, beastMasterBondGuid);

            // Add Release The Bees spell to the spellbook at level 1
            archetype.AddToAddFeatures(1, Features.ReleaseTheBees.CreateReleaseTheBeesFeature());

            // Add animal companion starting at level 4
            archetype.AddToAddFeatures(4, FeatureSelectionRefs.AnimalCompanionSelectionDruid.ToString());

            // Level 5-20: Animal Companion Ranks
            for (int level = 5; level <= 20; level++) {
                archetype.AddToAddFeatures(level, FeatureRefs.AnimalCompanionRank.ToString());
            }

            // Remove the party-wide buff features and replace with pet-only versions
            archetype
                //level 8
                .AddToAddFeatures(8, bonusTalentSelectionGuid)

                // Level 9: Replace Inspire Greatness with pet-only version
                .AddToRemoveFeatures(9, FeatureRefs.InspireGreatnessFeature.ToString())
                .AddToAddFeatures(9, petInspireGreatnessGuid)

                .AddToAddFeatures(11, FeatureRefs.SuperiorSummoning.ToString())

                // Level 12: Replace Soothing Performance with pet-only version, add Pack Tactics
                .AddToRemoveFeatures(12, FeatureRefs.SoothingPerformanceFeature.ToString())
                .AddToAddFeatures(12, petSoothingPerformanceFeatureGuid)
                .AddToAddFeatures(12, Features.PackTactics.featureGuid)

                // Level 15: Replace Inspire Heroics with pet-only version
                .AddToRemoveFeatures(15, FeatureRefs.InspireHeroicsFeature.ToString())
                .AddToAddFeatures(15, petInspireHeroicsGuid)

                //level 16
                .AddToAddFeatures(16, bonusTalentSelectionGuid);
        }


        private static void ConfigurePetOnlyInspireGreatness() {
            // Create area effect that only affects pets/animal companions and summoned creatures
            var areaEffect = AbilityAreaEffectConfigurator.New(petInspireGreatnessAreaName, petInspireGreatnessAreaGuid)
                .CopyFrom(AbilityAreaEffectRefs.InspireGreatnessArea, c => true)
                .OnConfigure(bp => {
                    // Find all AbilityAreaEffectBuff components and modify their conditions
                    var buffComps = bp.ComponentsArray.OfType<Kingmaker.UnitLogic.Abilities.Components.AreaEffects.AbilityAreaEffectBuff>();
                    foreach (var comp in buffComps) {
                        // Replace condition to only affect animal companions (pets) or summoned creatures
                        comp.Condition = ConditionsBuilder.New()
                            .Add<Kingmaker.UnitLogic.Mechanics.Conditions.ContextConditionIsAlly>()
                            .AddOrAndLogic(
                                ConditionsBuilder.New()
                                    .Add<Kingmaker.UnitLogic.Mechanics.Conditions.ContextConditionIsAnimalCompanion>()
                            )
                            .Build();
                    }
                })
                .Configure();

            // Create buff that uses the pet-only area effect
            var buff = BuffConfigurator.New(petInspireGreatnessBuffName, petInspireGreatnessBuffGuid)
                .CopyFrom(BuffRefs.InspireGreatnessBuff, c => true)
                .OnConfigure(bp => {
                    // Find AddAreaEffect component and update it
                    var areaComp = bp.ComponentsArray.OfType<Kingmaker.UnitLogic.Buffs.Components.AddAreaEffect>().FirstOrDefault();
                    if (areaComp != null) {
                        areaComp.m_AreaEffect = areaEffect.ToReference<BlueprintAbilityAreaEffectReference>();
                    }
                })
                .Configure();

            // Create toggle ability
            var toggle = ActivatableAbilityConfigurator.New(petInspireGreatnessToggleName, petInspireGreatnessToggleGuid)
                .CopyFrom(ActivatableAbilityRefs.InspireGreatnessToggleAbility, c => true)
                .SetBuff(buff)
                .Configure();

            // Create feature
            FeatureConfigurator.New(petInspireGreatnessName, petInspireGreatnessGuid)
                .CopyFrom(FeatureRefs.InspireGreatnessFeature, c => true)
                .SetDisplayName(petInspireGreatnessName + ".Name")
                .SetDescription(petInspireGreatnessName + ".Description")
                .AddFacts(new() { toggle })
                .Configure();
        }


        private static void ConfigurePetOnlyInspireHeroics() {
            // Create area effect that only affects pets/animal companions and summoned creatures
            var areaEffect = AbilityAreaEffectConfigurator.New(petInspireHeroicsAreaName, petInspireHeroicsAreaGuid)
                .CopyFrom(AbilityAreaEffectRefs.InspireHeroicsArea, c => true)
                .OnConfigure(bp => {
                    // Find all AbilityAreaEffectBuff components and modify their conditions
                    var buffComps = bp.ComponentsArray.OfType<Kingmaker.UnitLogic.Abilities.Components.AreaEffects.AbilityAreaEffectBuff>();
                    foreach (var comp in buffComps) {
                        // Replace condition to only affect animal companions (pets) or summoned creatures
                        comp.Condition = ConditionsBuilder.New()
                            .Add<Kingmaker.UnitLogic.Mechanics.Conditions.ContextConditionIsAlly>()
                            .AddOrAndLogic(
                                ConditionsBuilder.New()
                                    .Add<Kingmaker.UnitLogic.Mechanics.Conditions.ContextConditionIsAnimalCompanion>()
                            )
                            .Build();
                    }
                })
                .Configure();

            // Create buff that uses the pet-only area effect
            var buff = BuffConfigurator.New(petInspireHeroicsBuffName, petInspireHeroicsBuffGuid)
                .CopyFrom(BuffRefs.InspireHeroicsBuff, c => true)
                .OnConfigure(bp => {
                    // Find AddAreaEffect component and update it
                    var areaComp = bp.ComponentsArray.OfType<Kingmaker.UnitLogic.Buffs.Components.AddAreaEffect>().FirstOrDefault();
                    if (areaComp != null) {
                        areaComp.m_AreaEffect = areaEffect.ToReference<BlueprintAbilityAreaEffectReference>();
                    }
                })
                .Configure();

            // Create toggle ability
            var toggle = ActivatableAbilityConfigurator.New(petInspireHeroicsToggleName, petInspireHeroicsToggleGuid)
                .CopyFrom(ActivatableAbilityRefs.InspireHeroicsToggleAbility, c => true)
                .SetBuff(buff)
                .Configure();

            // Create feature
            FeatureConfigurator.New(petInspireHeroicsName, petInspireHeroicsGuid)
                .CopyFrom(FeatureRefs.InspireHeroicsFeature, c => true)
                .SetDisplayName(petInspireHeroicsName + ".Name")
                .SetDescription(petInspireHeroicsName + ".Description")
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

            featureSelection.Configure();
        }
    }
}
