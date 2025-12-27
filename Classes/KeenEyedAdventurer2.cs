using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Utils;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Blueprints;
using AviaryClasses.Classes.Features;
using System;
using Kingmaker.EntitySystem.Stats;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using Kingmaker.Utility;

namespace AviaryClasses.Classes {

    public enum LuckLevels {
        UNKIND = 0,
        WAVERING = 1,
        STEADY = 2,
        BRIGHT = 3,
        EXTREME = 4
    }

    public class KeenEyedAdventurer2 {
        private static readonly LogWrapper Logger = LogWrapper.Get("KeenEyedAdventurer2");

        public static BlueprintArchetype archetypeRef;
        public static readonly string featName = "KeenEyedAdventurer2";
        public static readonly string featGuid = "830ada44-825e-4e1e-9169-dba08b7d784a";


        public static void Configure() {

                // Determine luck level for this session
                LuckLevels[] luckValues = (LuckLevels[])Enum.GetValues(typeof(LuckLevels));
                LuckLevels luckLevel = luckValues[UnityEngine.Random.Range(0, luckValues.Length - 1)];

            try {
                // Configure all features
                MakeMyOwnLuckSpell.Configure();
                CantripSpecialization.Configure(luckLevel);
                OverpoweredCantrips.Configure(luckLevel);
                OverpoweredVortex.Configure(luckLevel);
                AscendantCantrips.Configure();
                LuckBuffFeature.Configure(luckLevel);

                // Create the archetype
                ArchetypeConfigurator archetype = ArchetypeConfigurator.New(featName, featGuid, CharacterClassRefs.WitchClass)
                .CopyFrom(ArchetypeRefs.RayMasterWitchArchetype)
                .SetReplaceSpellbook(MakeMyOwnLuckSpell.spellbookGuid)
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

            // Level 1 - Luck Tracker
            archetype.AddToAddFeatures(1, LuckBuffFeature.featGuid);

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

    internal class LuckBuffFeature {

        public static readonly string luckBuffName = "KeenEyedAdventurerLuckBuff";
        public static readonly string luckBuffGuid = "77190230-022e-4ce7-b144-d74ab921ddae";
        public static readonly string fortuneLuckBuffName = "KeenEyedAdventurerLuckFortuneBuff";
        public static readonly string fortuneLuckBuffGuid = "1b297e1c-960c-4f8b-8a3e-9ea82a9dbe9b";
        public static readonly string featName = "KeenEyedAdventurerLuckFeature";
        public static readonly string featGuid = "77190231-022e-4ce7-b144-d74bb921ddae";
        public static readonly string featureName = featName + ".Name";
        public static readonly string featureDescription = featName + ".Description";

        public static void Configure(LuckLevels luckLevel) {

            // Visible buff that displays session luck
            BlueprintFeature baseAbility = BlueprintTool.Get<BlueprintFeature>(FeatureRefs.BolsteredSpellFeat.ToString());
            var luckIcon = Utils.LoadIcon("luck.png", baseAbility.m_Icon);
            var highLuckIcon = Utils.LoadIcon("highluck.png", baseAbility.m_Icon);

            int baseIndex = (int)luckLevel;
            int fortuneIndex = Math.Min(baseIndex + 2, (int)LuckLevels.EXTREME);

            var luckBuff = BuffConfigurator.New(luckBuffName, luckBuffGuid)
            .SetDisplayName("OverpoweredVortexLuck" + baseIndex + ".Name")
            .SetDescription("OverpoweredVortexLuck" + baseIndex + ".Description")
            .SetIcon(luckIcon)
            .SetStacking(StackingType.Replace) // Replace instead of stack to avoid duplicates
            .SetRanks(1) // Set high max ranks to support high Intelligence characters
            .AddToFlags(BlueprintBuff.Flags.StayOnDeath) // Persist through death
            .Configure();

            var fortuneLuckBuff = BuffConfigurator.New(fortuneLuckBuffName, fortuneLuckBuffGuid)
            .SetDisplayName("OverpoweredVortexLuck" + fortuneIndex + ".Name")
            .SetDescription("OverpoweredVortexLuck" + fortuneIndex + ".Description")
            .SetIcon(highLuckIcon)
            .SetStacking(StackingType.Replace) // Replace instead of stack to avoid duplicates
            .SetRanks(1) // Set high max ranks to support high Intelligence characters
            .AddToFlags(BlueprintBuff.Flags.StayOnDeath) // Persist through death
            .Configure();

            ConditionsBuilder fortuneCondition = ConditionsBuilder.New()
            .CasterHasFact(BuffRefs.WitchHexFortuneBuff.ToString());

            var applyBaseLuck = ActionsBuilder.New()
            .RemoveBuff(fortuneLuckBuffGuid, toCaster: true)
            .ApplyBuffPermanent(luckBuffGuid, toCaster: true);

            var applyFortuneLuck = ActionsBuilder.New()
            .RemoveBuff(luckBuffGuid, toCaster: true)
            .ApplyBuffPermanent(fortuneLuckBuffGuid, toCaster: true);

            var clearLuck = ActionsBuilder.New()
            .RemoveBuff(luckBuffGuid, toCaster: true)
            .RemoveBuff(fortuneLuckBuffGuid, toCaster: true);

            var luckFeature = FeatureConfigurator.New(featName, featGuid)
            .SetDescription(featureDescription)
            .SetDisplayName(featureName)
            .SetHideInCharacterSheetAndLevelUp(true)
            .AddFactContextActions(
                activated: ActionsBuilder.New()
                    .Conditional(
                        conditions: fortuneCondition,
                        ifTrue: applyFortuneLuck,
                        ifFalse: applyBaseLuck
                    ),
                deactivated: clearLuck,
                dispose: clearLuck
            )
            .AddFactsChangeTrigger(
                checkedFacts: [BuffRefs.WitchHexFortuneBuff.ToString()],
                onFactGainedActions: applyFortuneLuck,
                onFactLostActions: applyBaseLuck
            )
            .Configure();

        }
    }

}
