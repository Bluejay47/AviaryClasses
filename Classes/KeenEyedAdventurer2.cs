using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Blueprints;
using AviaryClasses.Classes.Features;
using System;
using Kingmaker.EntitySystem.Stats;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;

namespace AviaryClasses.Classes {
    public class KeenEyedAdventurer2 {
        private static readonly LogWrapper Logger = LogWrapper.Get("KeenEyedAdventurer2");

        public static BlueprintArchetype archetypeRef;
        public static readonly string featName = "KeenEyedAdventurer2";
        public static readonly string featGuid = "830ada44-825e-4e1e-9169-dba08b7d784a";

        public static void Configure() {
            try {
                // Configure all features
                CantripSpecialization.Configure();
                OverpoweredCantrips.Configure();
                OverpoweredVortex.Configure();
                AscendantCantrips.Configure();

                // Create the archetype
                ArchetypeConfigurator archetype = ArchetypeConfigurator.New(featName, featGuid, CharacterClassRefs.WitchClass)
                .CopyFrom(ArchetypeRefs.RayMasterWitchArchetype)
                .SetLocalizedName(featName + ".Name")
                .SetOverrideAttributeRecommendations(true)
                .SetRecommendedAttributes(
                    StatType.Intelligence,
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

            // Level 1 - Cantrip Specialization
            archetype.AddToAddFeatures(1, CantripSpecialization.featGuid);

            // Level 4 - Animal Companion
            archetype.AddToAddFeatures(4, FeatureSelectionRefs.AnimalCompanionSelectionDivineHound.ToString());

            // Level 5-20 - Animal Companion Ranks
            for (int level = 5; level <= 20; level++) {
                archetype.AddToAddFeatures(level, FeatureRefs.AnimalCompanionRank.ToString());
            }

            // Level 9 - Overpowered Cantrips
            archetype.AddToAddFeatures(9, OverpoweredCantrips.featGuid);

            // Level 13 - Ascendant Cantrips
            archetype.AddToAddFeatures(13, AscendantCantrips.featGuid);

            // Level 16 - Replace Witch Hex with Overpowered Vortex
            archetype.AddToRemoveFeatures(16, FeatureSelectionRefs.WitchHexSelection.ToString());
            archetype.AddToAddFeatures(16, OverpoweredVortex.featGuid);

            // Level 20 - Remove Cantrip Specialization (replaced by better features)
            archetype.RemoveFromAddFeatures(20, [CantripSpecialization.featGuid]);
        }
        
    }
}