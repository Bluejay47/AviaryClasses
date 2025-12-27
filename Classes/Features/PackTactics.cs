using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.CustomConfigurators;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils;
using PackTacticsSharedFeats = AviaryClasses.Classes.Features.PackTacticsShared.PackTacticsSharedFeats;
using Kingmaker.Enums;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UnitLogic.Abilities.Blueprints;

namespace AviaryClasses.Classes.Features {
    public class PackTactics {
        private static readonly LogWrapper Logger = LogWrapper.Get("PackTactics");

        private static readonly string featureName = "BeastTamerPackTactics";
        public static readonly string featureGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac300e";
        private static readonly string selectionName = "BeastTamerPackTacticsSelection";
        public static readonly string selectionGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac300f";
        private static readonly string areaName = "BeastTamerPackTacticsArea";
        private static readonly string areaGuid = "e814144e-b73c-4d65-9275-82f082d4deca";
        private static readonly string auraBuffName = "BeastTamerPackTacticsAuraBuff";
        private static readonly string auraBuffGuid = "0e402175-b3e9-4d90-980e-07034f175271";
        private static readonly string effectBuffName = "BeastTamerPackTacticsEffectBuff";
        private static readonly string effectBuffGuid = "27eac6f4-eb93-4060-b43f-75a2bb31566f";

        public static void Configure() {
            try {
                PackTacticsSharedFeats.Configure();

                // Create the teamwork feat selection
                var selection = FeatureSelectionConfigurator.New(selectionName, selectionGuid)
                    .SetDisplayName("BeastTamerPackTactics.Name")
                    .SetDescription("BeastTamerPackTactics.Description")
                    .SetIcon(FeatureSelectionRefs.TeamworkFeat.Reference.Get().Icon)
                    .SetIsClassFeature(true)
                    .SetGroup(FeatureGroup.TeamworkFeat)
                    .SetIgnorePrerequisites(true)
                    .SetRanks(1);

                selection.ClearAllFeatures();
                selection.AddToAllFeatures(PackTacticsSharedFeats.BackToBackGuid);
                selection.AddToAllFeatures(PackTacticsSharedFeats.CoordinatedDefenseGuid);
                selection.AddToAllFeatures(PackTacticsSharedFeats.CoordinatedManeuversGuid);
                selection.AddToAllFeatures(PackTacticsSharedFeats.OutflankGuid);
                selection.AddToAllFeatures(PackTacticsSharedFeats.PreciseStrikeGuid);
                selection.AddToAllFeatures(PackTacticsSharedFeats.SeizeTheMomentGuid);
                selection.AddToAllFeatures(PackTacticsSharedFeats.ShakeItOffGuid);
                selection.AddToAllFeatures(PackTacticsSharedFeats.ShieldWallGuid);
                selection.AddToAllFeatures(PackTacticsSharedFeats.TandemTripGuid);
                selection.Configure();

                // Create the effect buff that shares teamwork feats (applied to pets/summons in the aura)
                var effectBuff = BuffConfigurator.New(effectBuffName, effectBuffGuid)
                    .SetDisplayName("BeastTamerPackTactics.Name")
                    .SetDescription("BeastTamerPackTactics.Description")
                    .SetIcon(FeatureSelectionRefs.TeamworkFeat.Reference.Get().Icon)
                    .SetFlags(Kingmaker.UnitLogic.Buffs.Blueprints.BlueprintBuff.Flags.HiddenInUi)
                    .AddFactsFromCaster(
                        featureFromSelection: true,
                        selection: selectionGuid)
                    .Configure();

                // Create the area effect (30 feet)
                var companionOrSummonCondition = ConditionsBuilder.New()
                    .UseOr()
                    .Add<Kingmaker.UnitLogic.Mechanics.Conditions.ContextConditionIsAnimalCompanion>()
                    .HasFact(BuffRefs.SummonedUnitBuff.ToString());

                var area = AbilityAreaEffectConfigurator.New(areaName, areaGuid)
                    .SetAffectEnemies(false)
                    .SetTargetType(Kingmaker.UnitLogic.Abilities.Blueprints.BlueprintAbilityAreaEffect.TargetType.Ally)
                    .SetShape(Kingmaker.UnitLogic.Abilities.Blueprints.AreaEffectShape.Cylinder)
                    .SetSize(new Kingmaker.Utility.Feet(30))
                    .AddAbilityAreaEffectRunAction(
                        unitEnter: ActionsBuilder.New()
                            .Conditional(
                                conditions: ConditionsBuilder.New()
                                    .Add<Kingmaker.UnitLogic.Mechanics.Conditions.ContextConditionIsAlly>()
                                    .AddOrAndLogic(companionOrSummonCondition)
                                    .Build(),
                                ifTrue: ActionsBuilder.New()
                                    .ApplyBuffPermanent(effectBuffGuid, isNotDispelable: true, asChild: true)
                                    .Build(),
                                ifFalse: ActionsBuilder.New().Build())
                            .Build(),
                        unitExit: ActionsBuilder.New()
                            .RemoveBuff(effectBuffGuid)
                            .Build())
                    .Configure();

                // Create the aura buff that creates the area effect
                var auraBuff = BuffConfigurator.New(auraBuffName, auraBuffGuid)
                    .SetDisplayName("BeastTamerPackTactics.Name")
                    .SetDescription("BeastTamerPackTactics.Description")
                    .SetIcon(FeatureSelectionRefs.TeamworkFeat.Reference.Get().Icon)
                    .SetFlags(Kingmaker.UnitLogic.Buffs.Blueprints.BlueprintBuff.Flags.HiddenInUi)
                    .AddAreaEffect(areaGuid)
                    .Configure();

                // Create the main feature that grants the selection and aura
                FeatureConfigurator.New(featureName, featureGuid)
                    .SetDisplayName("BeastTamerPackTactics.Name")
                    .SetDescription("BeastTamerPackTactics.Description")
                    .SetIcon(FeatureSelectionRefs.TeamworkFeat.Reference.Get().Icon)
                    .SetIsClassFeature(true)
                    .AddFacts(new() { auraBuffGuid })
                    .Configure();

                Logger.Info("Pack Tactics feature configured successfully");

            } catch (System.Exception ex) {
                Logger.Error("Failed to configure Pack Tactics", ex);
            }
        }
    }
}
