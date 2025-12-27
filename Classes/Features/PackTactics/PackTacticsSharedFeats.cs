using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using System;
using System.Reflection;

namespace AviaryClasses.Classes.Features.PackTacticsShared {
    public static class PackTacticsSharedFeats {
        private static readonly LogWrapper Logger = LogWrapper.Get("PackTacticsSharedFeats");

        private const string backToBackName = "BeastTamerPackTacticsBackToBack";
        public const string BackToBackGuid = "333f6e75-8a01-49de-8f2f-4cb9ae065c1a";
        private const string coordinatedDefenseName = "BeastTamerPackTacticsCoordinatedDefense";
        public const string CoordinatedDefenseGuid = "f1177fa3-c8bd-4fe4-b89c-b66461a5b455";
        private const string coordinatedManeuversName = "BeastTamerPackTacticsCoordinatedManeuvers";
        public const string CoordinatedManeuversGuid = "0a90fadb-91b0-4ec0-a316-b109daa0db8c";
        private const string outflankName = "BeastTamerPackTacticsOutflank";
        public const string OutflankGuid = "798a32d8-76b4-468f-9fef-22a62602c3b3";
        private const string preciseStrikeName = "BeastTamerPackTacticsPreciseStrike";
        public const string PreciseStrikeGuid = "abf1ccd4-e1d4-4c09-9281-f9390103c184";
        private const string seizeTheMomentName = "BeastTamerPackTacticsSeizeTheMoment";
        public const string SeizeTheMomentGuid = "dd31a695-baaf-4ed6-8224-2194c1fc680e";
        private const string shakeItOffName = "BeastTamerPackTacticsShakeItOff";
        public const string ShakeItOffGuid = "a6bffe63-ebf6-4a12-9631-b289aa84df69";
        private const string shieldWallName = "BeastTamerPackTacticsShieldWall";
        public const string ShieldWallGuid = "315e98e2-327c-4270-b8ae-87ca9c4e9033";
        private const string tandemTripName = "BeastTamerPackTacticsTandemTrip";
        public const string TandemTripGuid = "4eb7e270-a66e-438b-bf22-b7f3e18675eb";

        public static void Configure() {
            try {
                ConfigureSharedFeat(
                    backToBackName,
                    BackToBackGuid,
                    "BeastTamerPackTacticsBackToBack.Name",
                    FeatureRefs.BackToBack);
                ConfigureSharedFeat(
                    coordinatedDefenseName,
                    CoordinatedDefenseGuid,
                    "BeastTamerPackTacticsCoordinatedDefense.Name",
                    FeatureRefs.CoordinatedDefense);
                ConfigureSharedFeat(
                    coordinatedManeuversName,
                    CoordinatedManeuversGuid,
                    "BeastTamerPackTacticsCoordinatedManeuvers.Name",
                    FeatureRefs.CoordinatedManeuvers);
                ConfigureSharedFeat(
                    outflankName,
                    OutflankGuid,
                    "BeastTamerPackTacticsOutflank.Name",
                    FeatureRefs.Outflank);
                ConfigureSharedFeat(
                    preciseStrikeName,
                    PreciseStrikeGuid,
                    "BeastTamerPackTacticsPreciseStrike.Name",
                    FeatureRefs.PreciseStrike);
                ConfigureSharedFeat(
                    seizeTheMomentName,
                    SeizeTheMomentGuid,
                    "BeastTamerPackTacticsSeizeTheMoment.Name",
                    FeatureRefs.SiezeTheMoment);
                ConfigureSharedFeat(
                    shakeItOffName,
                    ShakeItOffGuid,
                    "BeastTamerPackTacticsShakeItOff.Name",
                    FeatureRefs.ShakeItOff);
                ConfigureSharedFeat(
                    shieldWallName,
                    ShieldWallGuid,
                    "BeastTamerPackTacticsShieldWall.Name",
                    FeatureRefs.ShieldWall);
                ConfigureSharedFeat(
                    tandemTripName,
                    TandemTripGuid,
                    "BeastTamerPackTacticsTandemTrip.Name",
                    FeatureRefs.TandemTrip);

                Logger.Info("Pack Tactics shared feats configured successfully");
            } catch (Exception ex) {
                Logger.Error("Failed to configure Pack Tactics shared feats", ex);
            }
        }

        private static void ConfigureSharedFeat(
            string featureName,
            string featureGuid,
            string displayNameKey,
            Blueprint<BlueprintReference<BlueprintFeature>> baseFeature) {
            FeatureConfigurator.New(featureName, featureGuid)
                .CopyFrom(baseFeature.Cast<BlueprintReference<BlueprintScriptableObject>>(), c => true)
                .SetDisplayName(displayNameKey)
                .SetIsClassFeature(true)
                .OnConfigure(bp => {
                    // Retarget self-references so the shared feats only interact with each other.
                    ReplaceFeatureReferences(bp, baseFeature.ToString(), featureGuid);
                    bp.Groups = Array.Empty<FeatureGroup>();
                })
                .Configure();
        }

        private static void ReplaceFeatureReferences(BlueprintFeature bp, string oldGuid, string newGuid) {
            var oldId = BlueprintGuid.Parse(oldGuid);
            var newId = BlueprintGuid.Parse(newGuid);

            foreach (var component in bp.ComponentsArray) {
                var fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var field in fields) {
                    if (typeof(BlueprintReferenceBase).IsAssignableFrom(field.FieldType)) {
                        var reference = field.GetValue(component) as BlueprintReferenceBase;
                        if (reference != null && reference.Guid == oldId) {
                            reference.ReadGuidFromGuid(newId);
                        }
                        continue;
                    }

                    if (!field.FieldType.IsArray) {
                        continue;
                    }

                    var elementType = field.FieldType.GetElementType();
                    if (elementType == null || !typeof(BlueprintReferenceBase).IsAssignableFrom(elementType)) {
                        continue;
                    }

                    var array = field.GetValue(component) as Array;
                    if (array == null) {
                        continue;
                    }

                    for (var i = 0; i < array.Length; i++) {
                        if (array.GetValue(i) is BlueprintReferenceBase reference && reference.Guid == oldId) {
                            reference.ReadGuidFromGuid(newId);
                        }
                    }
                }
            }
        }
    }
}
