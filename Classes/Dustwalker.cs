using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Blueprints;
using System;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Alignments;
using Kingmaker.Blueprints.Classes.Prerequisites;
using BlueprintCore.Blueprints.Configurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators;
using System;

namespace AviaryClasses.Classes {
    public class Dustwalker {
        private static readonly LogWrapper Logger = LogWrapper.Get("Dustwalker");

        public static BlueprintArchetype archetypeRef;
        public static readonly string featName = "Dustwalker";
        public static readonly string featGuid = "d7a3b8f5-2c1e-4b9a-a8f3-1e5d4c7b9a2f";

        public static void Configure() {
            try {
                // Configure Dustwalker-specific features
                DustwalkerLoreNature.Configure();
                DustwalkerFlurryOfBlows.Configure();
                DustwalkerProficiencies.Configure();
                DustwalkerMountFeature.Configure();
                DustwalkerDevotedGuardian.Configure();
                DustwalkerMountedCombat.Configure();
                DustwalkerDesertTerrain.Configure();
                DustwalkerDesertExpertise.Configure();
                DustwalkerKiWeapon.Configure();
                DustwalkerNatureLore.Configure();
                DustwalkerPoisonResistance.Configure();
                DustwalkerAbyssTerrain.Configure();
                DustwalkerAbyssExpertise.Configure();
                DustwalkerEnhancedFlurry.Configure();
                DustwalkerCamouflage.Configure();

                // Create the archetype
                ArchetypeConfigurator archetype = ArchetypeConfigurator.New(featName, featGuid, CharacterClassRefs.MonkClass)
                .SetBuildChanging(true)
                .SetLocalizedName(featName + ".Name")
                .SetLocalizedDescription(featName + ".Description")
                .SetOverrideAttributeRecommendations(true)
                .SetRecommendedAttributes(
                    StatType.Wisdom,
                    StatType.Dexterity
                )
                .AddPrerequisiteAlignment(
                    AlignmentMaskType.TrueNeutral | AlignmentMaskType.NeutralGood,
                    archetypeAlignment: true
                );

                // Configure archetype progression
                ConfigureArchetypeProgression(archetype);

                archetypeRef = archetype.Configure();

                var characterClass = CharacterClassConfigurator.For(CharacterClassRefs.MonkClass.ToString())
                .AddPrerequisiteAlignment(
                    AlignmentMaskType.TrueNeutral | AlignmentMaskType.NeutralGood,
                    mergeBehavior: ComponentMerge.Merge,
                    merge: (existingComponent, newComponent) => {
                        var existing = existingComponent as PrerequisiteAlignment;
                        var newPrereq = newComponent as PrerequisiteAlignment;
                        if (existing != null && newPrereq != null) {
                            existing.Alignment = existing.Alignment | newPrereq.Alignment;
                        }
                    })
                .Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }
        }

        private static void ConfigureArchetypeProgression(ArchetypeConfigurator archetype) {

            // Dustwalker archetype progression - removes all ki strike abilities and adds terrain/combat expertise 4153755355a0b9b4e956c9ca232c22cf

            // Level 1 - Add Dustwalker combat features
            archetype.AddToAddFeatures(1, DustwalkerLoreNature.featGuid);
            archetype.AddToAddFeatures(1, DustwalkerFlurryOfBlows.featGuid);
            archetype.AddToAddFeatures(1, DustwalkerProficiencies.featGuid);
            archetype.AddToAddFeatures(1, DustwalkerMountFeature.featGuid);
            archetype.AddToAddFeatures(1, FeatureSelectionRefs.SoheiMonasticMountHorseSelection.ToString());
            archetype.AddToAddFeatures(1, FeatureSelectionRefs.SoheiMountedCombatFeatSelection.ToString());
            archetype.AddToAddFeatures(1, DustwalkerDevotedGuardian.featGuid);
            archetype.AddToAddFeatures(1, DustwalkerMountedCombat.featGuid);
            archetype.AddToRemoveFeatures(1, FeatureRefs.MonkUnarmedStrike.ToString());
            archetype.AddToRemoveFeatures(1, FeatureRefs.MonkFlurryOfBlowstUnlock.ToString());
            archetype.AddToRemoveFeatures(1, FeatureRefs.StunningFist.ToString());
            archetype.AddToRemoveFeatures(1, FeatureSelectionRefs.MonkBonusFeatSelectionLevel1.ToString());
            archetype.AddToRemoveFeatures(1, FeatureRefs.MonkWeaponProficiency.ToString());

            // Level 2 - Mounted combat feat
            archetype.AddToAddFeatures(2, FeatureSelectionRefs.SoheiMountedCombatFeatSelection.ToString());
            archetype.AddToRemoveFeatures(2, FeatureSelectionRefs.MonkBonusFeatSelectionLevel1.ToString());
            archetype.AddToAddFeatures(2, DustwalkerDesertTerrain.featGuid);

            // Level 3 - Desert expertise
            archetype.AddToAddFeatures(3, DustwalkerDesertExpertise.featGuid);
            archetype.AddToRemoveFeatures(3, FeatureRefs.MonkFastMovementUnlock.ToString());
            archetype.AddToRemoveFeatures(3, FeatureRefs.KiStrikeMagic.ToString());

            // Level 4 - Ki weapon feature
            archetype.AddToAddFeatures(4, DustwalkerKiWeapon.featGuid);
            archetype.AddToRemoveFeatures(4, FeatureRefs.StunningFistFatigueFeature.ToString());
            archetype.AddToRemoveFeatures(4, FeatureRefs.MonkUnarmedStrikeLevel4.ToString());
            archetype.AddToRemoveFeatures(4, FeatureSelectionRefs.MonkKiPowerSelection.ToString());

            // Level 5 - Nature focus and poison immunity
            archetype.AddToAddFeatures(5, DustwalkerNatureLore.featGuid);
            archetype.AddToAddFeatures(5, DustwalkerPoisonResistance.featGuid);
            archetype.AddToRemoveFeatures(5, FeatureRefs.PurityOfBody.ToString());

            // Level 6 - Mounted combat and weapon training
            archetype.AddToAddFeatures(6, FeatureSelectionRefs.SoheiMountedCombatFeatSelection6.ToString());
            archetype.AddToAddFeatures(6, FeatureSelectionRefs.SoheiWeaponTrainingSelection.ToString());
            //archetype.AddToAddFeatures(6, FeatureRefs.SoheiFlurryOfBlowsUnlock6level.ToString());
            archetype.AddToRemoveFeatures(6, FeatureSelectionRefs.MonkBonusFeatSelectionLevel6.ToString());

            // Level 7 -
            archetype.AddToRemoveFeatures(7, FeatureRefs.KiStrikeColdIronSilver.ToString());
            archetype.AddToAddFeatures(7, DustwalkerAbyssTerrain.featGuid);

            // Level 8 - Abyss expertise
            archetype.AddToAddFeatures(8, DustwalkerAbyssExpertise.featGuid);
            archetype.AddToRemoveFeatures(8, FeatureRefs.MonkUnarmedStrikeLevel8.ToString());
            archetype.AddToRemoveFeatures(8, FeatureRefs.StunningFistSickenedFeature.ToString());
            archetype.AddToAddFeatures(8, FeatureSelectionRefs.FighterFeatSelection.ToString());
            archetype.AddToRemoveFeatures(8, FeatureSelectionRefs.MonkKiPowerSelection.ToString());

            // Level 9 - Remove Improved Evasion
            archetype.AddToRemoveFeatures(9, FeatureSelectionRefs.MonkStyleStrike.ToString());

            // Level 10 - Mounted combat (no lawful ki strike)
            archetype.AddToAddFeatures(10, FeatureSelectionRefs.SoheiMountedCombatFeatSelection10.ToString());
            archetype.AddToRemoveFeatures(10, FeatureRefs.KiStrikeLawful.ToString());
            archetype.AddToRemoveFeatures(10, FeatureSelectionRefs.MonkBonusFeatSelectionLevel10.ToString());

            // Level 11 - Enhanced flurry and advanced weapon training
            archetype.AddToAddFeatures(11, DustwalkerEnhancedFlurry.featGuid);
            archetype.AddToAddFeatures(11, FeatureSelectionRefs.AdvancedWeaponTraining1.ToString());
            archetype.AddToAddFeatures(11, FeatureRefs.SoheiFlurryOfBlowstLevel11Unlock.ToString());
            archetype.AddToRemoveFeatures(11, FeatureRefs.MonkFlurryOfBlowstLevel11Unlock.ToString());

            // Level 12 - Weapon training advancement
            archetype.AddToAddFeatures(12, FeatureSelectionRefs.SoheiWeaponTrainingSelection.ToString());
            archetype.AddToAddFeatures(12, FeatureSelectionRefs.SoheiWeaponRankUpTrainingSelection.ToString());
            archetype.AddToRemoveFeatures(12, FeatureRefs.MonkUnarmedStrikeLevel12.ToString());
            archetype.AddToRemoveFeatures(12, FeatureSelectionRefs.MonkKiPowerSelection.ToString());

            // Level 13
            archetype.AddToRemoveFeatures(13, FeatureSelectionRefs.MonkStyleStrike.ToString());
            archetype.AddToAddFeatures(13, DustwalkerCamouflage.featGuid);

            // Level 14 - Remove Diamond Soul
            archetype.AddToRemoveFeatures(14, FeatureSelectionRefs.MonkBonusFeatSelectionLevel10.ToString());
            archetype.AddToAddFeatures(14, FeatureSelectionRefs.MonkStyleStrike.ToString());

            // Level 15
            archetype.AddToAddFeatures(15, FeatureSelectionRefs.FighterFeatSelection.ToString());

            // Level 16 - Second advanced weapon training (no adamantine ki strike)
            archetype.AddToAddFeatures(16, FeatureSelectionRefs.AdvancedWeaponTraining2.ToString());
            archetype.AddToRemoveFeatures(16, FeatureRefs.KiStrikeAdamantine.ToString());
            archetype.AddToRemoveFeatures(16, FeatureRefs.MonkUnarmedStrikeLevel16.ToString());
            archetype.AddToRemoveFeatures(16, FeatureSelectionRefs.MonkKiPowerSelection.ToString());

            //Level 17
            archetype.AddToRemoveFeatures(17, FeatureSelectionRefs.MonkStyleStrike.ToString());

            // Level 18 - Final mounted combat and weapon training
            archetype.AddToAddFeatures(18, FeatureSelectionRefs.SoheiWeaponTrainingSelection.ToString());
            archetype.AddToAddFeatures(18, FeatureSelectionRefs.SoheiWeaponRankUpTrainingSelection.ToString());
            archetype.AddToRemoveFeatures(18, FeatureSelectionRefs.MonkBonusFeatSelectionLevel10.ToString());

            // Level 20 - Remove final monk unarmed strike and Perfect Self
            archetype.AddToRemoveFeatures(20, FeatureRefs.MonkUnarmedStrikeLevel20.ToString());
            //archetype.AddToRemoveFeatures(20, FeatureRefs.KiPerfectSelfFeature.ToString());

        }
    }

    // Dustwalker-specific feature adaptations
    public class DustwalkerLoreNature {
        private static readonly LogWrapper Logger = LogWrapper.Get("DustwalkerLoreNature");
        public static readonly string featName = "SoheiLoreNatureFeature";
        public static readonly string featGuid = "d7a3b8f5-2c1e-4b9a-a8f3-1e5d4c7b9a3f";

        public static void Configure() {
            try {
                FeatureConfigurator.New(featName, featGuid).CopyFrom(FeatureRefs.SoheiLoreNatureFeature.ToString(), x => true)
                    .SetDescription("DustwalkerLoreNature.Description")
                    .SetDisplayName("DustwalkerLoreNature.Name")
                    .Configure();
            } catch (Exception ex) { Logger.Error(ex.ToString()); }
        }
    }

    public class DustwalkerFlurryOfBlows {
        private static readonly LogWrapper Logger = LogWrapper.Get("DustwalkerFlurryOfBlows");
        public static readonly string featName = "SoheiFlurryOfBlowsUnlock";
        public static readonly string featGuid = "d7a3b8f5-2c1e-4b9a-a8f3-1e5d4c7b9a4f";

        public static void Configure() {
            try {
                FeatureConfigurator.New(featName, featGuid).CopyFrom(FeatureRefs.SoheiFlurryOfBlowsUnlock.ToString(), x => true)
                    .SetDescription("DustwalkerFlurryOfBlows.Description")
                    .SetDisplayName("DustwalkerFlurryOfBlows.Name")
                    .Configure();
            } catch (Exception ex) { Logger.Error(ex.ToString()); }
        }
    }

    public class DustwalkerProficiencies {
        private static readonly LogWrapper Logger = LogWrapper.Get("DustwalkerProficiencies");
        public static readonly string featName = "SoheiProficiencies";
        public static readonly string featGuid = "d7a3b8f5-2c1e-4b9a-a8f3-1e5d4c7b9a5f";

        public static void Configure() {
            try {
                FeatureConfigurator.New(featName, featGuid).CopyFrom(FeatureRefs.SoheiProficiencies.ToString(), x => true)
                    .SetDescription("DustwalkerProficiencies.Description")
                    .SetDisplayName("DustwalkerProficiencies.Name")
                    .Configure();
            } catch (Exception ex) { Logger.Error(ex.ToString()); }
        }
    }

    public class DustwalkerMountFeature {
        private static readonly LogWrapper Logger = LogWrapper.Get("DustwalkerMountFeature");
        public static readonly string featName = "SoheiMonasticMountFeature";
        public static readonly string featGuid = "d7a3b8f5-2c1e-4b9a-a8f3-1e5d4c7b9a6f";

        public static void Configure() {
            try {
                FeatureConfigurator.New(featName, featGuid).CopyFrom(FeatureRefs.SoheiMonasticMountFeature.ToString(), x => true)
                    .SetDescription("DustwalkerMountFeature.Description")
                    .SetDisplayName("DustwalkerMountFeature.Name")
                    .Configure();
            } catch (Exception ex) { Logger.Error(ex.ToString()); }
        }
    }

    public class DustwalkerDevotedGuardian {
        private static readonly LogWrapper Logger = LogWrapper.Get("DustwalkerDevotedGuardian");
        public static readonly string featName = "SoheiDevotedGuardianFeature";
        public static readonly string featGuid = "d7a3b8f5-2c1e-4b9a-a8f3-1e5d4c7b9a7f";

        public static void Configure() {
            try {
                FeatureConfigurator.New(featName, featGuid).CopyFrom(FeatureRefs.SoheiDevotedGuardianFeature.ToString(), x => true)
                    .SetDescription("DustwalkerDevotedGuardian.Description")
                    .SetDisplayName("DustwalkerDevotedGuardian.Name")
                    .Configure();
            } catch (Exception ex) { Logger.Error(ex.ToString()); }
        }
    }

    public class DustwalkerMountedCombat {
        private static readonly LogWrapper Logger = LogWrapper.Get("DustwalkerMountedCombat");
        public static readonly string featName = "SoheiMountedCombatFeatFeature";
        public static readonly string featGuid = "d7a3b8f5-2c1e-4b9a-a8f3-1e5d4c7b9a8f";

        public static void Configure() {
            try {
                FeatureConfigurator.New(featName, featGuid).CopyFrom(FeatureRefs.SoheiMountedCombatFeatFeature.ToString(), x => true)
                    .SetDescription("DustwalkerMountedCombat.Description")
                    .SetDisplayName("DustwalkerMountedCombat.Name")
                    .Configure();
            } catch (Exception ex) { Logger.Error(ex.ToString()); }
        }
    }

    public class DustwalkerDesertTerrain {
        private static readonly LogWrapper Logger = LogWrapper.Get("DustwalkerDesertTerrain");
        public static readonly string featName = "FavoriteTerrainDesert";
        public static readonly string featGuid = "d7a3b8f5-2c1e-4b9a-a8f3-1e5d4c7b9a9f";

        public static void Configure() {
            try {
                FeatureConfigurator.New(featName, featGuid).CopyFrom(FeatureRefs.FavoriteTerrainDesert.ToString(), x => true)
                    .SetDescription("DustwalkerDesertTerrain.Description")
                    .SetDisplayName("DustwalkerDesertTerrain.Name")
                    .Configure();
            } catch (Exception ex) { Logger.Error(ex.ToString()); }
        }
    }

    public class DustwalkerDesertExpertise {
        private static readonly LogWrapper Logger = LogWrapper.Get("DustwalkerDesertExpertise");
        public static readonly string featName = "TerrainExpertiseDesert";
        public static readonly string featGuid = "d7a3b8f5-2c1e-4b9a-a8f3-1e5d4c7b9b0f";

        public static void Configure() {
            try {
                FeatureConfigurator.New(featName, featGuid).CopyFrom(FeatureRefs.TerrainExpertiseDesert.ToString(), x => true)
                    .SetDescription("DustwalkerDesertExpertise.Description")
                    .SetDisplayName("DustwalkerDesertExpertise.Name")
                    .Configure();
            } catch (Exception ex) { Logger.Error(ex.ToString()); }
        }
    }

    public class DustwalkerKiWeapon {
        private static readonly LogWrapper Logger = LogWrapper.Get("DustwalkerKiWeapon");
        public static readonly string featName = "SoheiKiWeaponFeature";
        public static readonly string featGuid = "d7a3b8f5-2c1e-4b9a-a8f3-1e5d4c7b9b1f";

        public static void Configure() {
            try {
                FeatureConfigurator.New(featName, featGuid).CopyFrom(FeatureRefs.SoheiKiWeaponFeature.ToString(), x => true)
                    .SetDescription("DustwalkerKiWeapon.Description")
                    .SetDisplayName("DustwalkerKiWeapon.Name")
                    .Configure();
            } catch (Exception ex) { Logger.Error(ex.ToString()); }
        }
    }

    public class DustwalkerNatureLore {
        private static readonly LogWrapper Logger = LogWrapper.Get("DustwalkerNatureLore");
        public static readonly string featName = "SkillFocusLoreNature";
        public static readonly string featGuid = "d7a3b8f5-2c1e-4b9a-a8f3-1e5d4c7b9b2f";

        public static void Configure() {
            try {
                FeatureConfigurator.New(featName, featGuid).CopyFrom(FeatureRefs.SkillFocusLoreNature.ToString(), x => true)
                    .SetDescription("DustwalkerNatureLore.Description")
                    .SetDisplayName("DustwalkerNatureLore.Name")
                    .Configure();
            } catch (Exception ex) { Logger.Error(ex.ToString()); }
        }
    }

    public class DustwalkerPoisonResistance {
        private static readonly LogWrapper Logger = LogWrapper.Get("DustwalkerPoisonResistance");
        public static readonly string featName = "PoisonImmunity";
        public static readonly string featGuid = "d7a3b8f5-2c1e-4b9a-a8f3-1e5d4c7b9b3f";

        public static void Configure() {
            try {
                FeatureConfigurator.New(featName, featGuid).CopyFrom(FeatureRefs.PoisonImmunity.ToString(), x => true)
                    .SetDescription("DustwalkerPoisonResistance.Description")
                    .SetDisplayName("DustwalkerPoisonResistance.Name")
                    .Configure();
            } catch (Exception ex) { Logger.Error(ex.ToString()); }
        }
    }

    public class DustwalkerAbyssTerrain {
        private static readonly LogWrapper Logger = LogWrapper.Get("DustwalkerAbyssTerrain");
        public static readonly string featName = "FavoriteTerrainAbyss";
        public static readonly string featGuid = "d7a3b8f5-2c1e-4b9a-a8f3-1e5d4c7b9b4f";

        public static void Configure() {
            try {
                FeatureConfigurator.New(featName, featGuid).CopyFrom(FeatureRefs.FavoriteTerrainAbyss.ToString(), x => true)
                    .SetDescription("DustwalkerAbyssTerrain.Description")
                    .SetDisplayName("DustwalkerAbyssTerrain.Name")
                    .Configure();
            } catch (Exception ex) { Logger.Error(ex.ToString()); }
        }
    }

    public class DustwalkerAbyssExpertise {
        private static readonly LogWrapper Logger = LogWrapper.Get("DustwalkerAbyssExpertise");
        public static readonly string featName = "TerrainExpertiseAbyss";
        public static readonly string featGuid = "d7a3b8f5-2c1e-4b9a-a8f3-1e5d4c7b9b5f";

        public static void Configure() {
            try {
                FeatureConfigurator.New(featName, featGuid).CopyFrom(FeatureRefs.TerrainExpertiseAbyss.ToString(), x => true)
                    .SetDescription("DustwalkerAbyssExpertise.Description")
                    .SetDisplayName("DustwalkerAbyssExpertise.Name")
                    .Configure();
            } catch (Exception ex) { Logger.Error(ex.ToString()); }
        }
    }

    public class DustwalkerEnhancedFlurry {
        private static readonly LogWrapper Logger = LogWrapper.Get("DustwalkerEnhancedFlurry");
        public static readonly string featName = "SoheiFlurryOfBlowstLevel11Unlock";
        public static readonly string featGuid = "d7a3b8f5-2c1e-4b9a-a8f3-1e5d4c7b9b6f";

        public static void Configure() {
            try {
                FeatureConfigurator.New(featName, featGuid).CopyFrom(FeatureRefs.SoheiFlurryOfBlowstLevel11Unlock.ToString(), x => true)
                    .SetDescription("DustwalkerEnhancedFlurry.Description")
                    .SetDisplayName("DustwalkerEnhancedFlurry.Name")
                    .Configure();
            } catch (Exception ex) { Logger.Error(ex.ToString()); }
        }
    }

    public class DustwalkerCamouflage {
        private static readonly LogWrapper Logger = LogWrapper.Get("DustwalkerCamouflage");
        public static readonly string featName = "Camouflage";
        public static readonly string featGuid = "d7a3b8f5-2c1e-4b9a-a8f3-1e5d4c7b9b7f";

        public static void Configure() {
            try {
                FeatureConfigurator.New(featName, featGuid).CopyFrom(FeatureRefs.Camouflage.ToString(), x => true)
                    .SetDescription("DustwalkerCamouflage.Description")
                    .SetDisplayName("DustwalkerCamouflage.Name")
                    .Configure();
            } catch (Exception ex) { Logger.Error(ex.ToString()); }
        }
    }
}