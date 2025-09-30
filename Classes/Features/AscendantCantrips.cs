using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Enums.Damage;
using System;

namespace AviaryClasses.Classes.Features {
    public class AscendantCantrips {
        private static readonly LogWrapper Logger = LogWrapper.Get("AscendantCantrips");

        public static readonly string featName = "AscendantCantrips";
        public static readonly string featGuid = "e8f4c4a5-2d3b-4789-9abc-def123456789";

        // Individual element feature GUIDs
        public static readonly string fireElementGuid = "e8f4c4a5-2d3b-4789-9abc-def123456780";
        public static readonly string coldElementGuid = "e8f4c4a5-2d3b-4789-9abc-def123456781";
        public static readonly string electricityElementGuid = "e8f4c4a5-2d3b-4789-9abc-def123456782";
        public static readonly string acidElementGuid = "e8f4c4a5-2d3b-4789-9abc-def123456783";

        public static readonly string featureName = featName + ".Name";
        public static readonly string featureDescription = featName + ".Description";

        public static void Configure() {
            try {
                // Create individual element features first
                CreateIndividualElementFeatures();

                // Create the main feature that grants all four element features
                BlueprintFeature baseFeature = BlueprintTool.Get<BlueprintFeature>(FeatureRefs.CantripMasteryFeature.ToString());
                FeatureConfigurator feature = FeatureConfigurator.New(featName, featGuid);

                feature.SetDescription(featureDescription);
                feature.SetDisplayName(featureName);
                feature.SetIsClassFeature(true);
                feature.SetIcon(baseFeature.m_Icon);

                // Add all four element features as granted features
                feature.AddFacts(new() { fireElementGuid, coldElementGuid, electricityElementGuid, acidElementGuid });

                feature.Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }
        }

        private static void CreateIndividualElementFeatures() {
            // Fire element feature
            FeatureConfigurator.New("AscendantCantripsFire", fireElementGuid)
                .SetDisplayName("Ascendant Fire Cantrips")
                .SetDescription("Fire-based abilities ignore elemental resistance and immunity.")
                .SetHideInUI(true)
                .SetIsClassFeature(true)
                .AddComponent<AscendantElement>(c => {
                    c.Element = DamageEnergyType.Fire;
                })
                .Configure();

            // Cold element feature
            FeatureConfigurator.New("AscendantCantripsCold", coldElementGuid)
                .SetDisplayName("Ascendant Cold Cantrips")
                .SetDescription("Cold-based abilities ignore elemental resistance and immunity.")
                .SetHideInUI(true)
                .SetIsClassFeature(true)
                .AddComponent<AscendantElement>(c => {
                    c.Element = DamageEnergyType.Cold;
                })
                .Configure();

            // Electricity element feature
            FeatureConfigurator.New("AscendantCantripsElectricity", electricityElementGuid)
                .SetDisplayName("Ascendant Electricity Cantrips")
                .SetDescription("Electricity-based abilities ignore elemental resistance and immunity.")
                .SetHideInUI(true)
                .SetIsClassFeature(true)
                .AddComponent<AscendantElement>(c => {
                    c.Element = DamageEnergyType.Electricity;
                })
                .Configure();

            // Acid element feature
            FeatureConfigurator.New("AscendantCantripsAcid", acidElementGuid)
                .SetDisplayName("Ascendant Acid Cantrips")
                .SetDescription("Acid-based abilities ignore elemental resistance and immunity.")
                .SetHideInUI(true)
                .SetIsClassFeature(true)
                .AddComponent<AscendantElement>(c => {
                    c.Element = DamageEnergyType.Acid;
                })
                .Configure();
        }
    }
}