using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Designers.Mechanics.Facts;
using System;

namespace AviaryClasses.Fix {
    public class CharlatansNecklaceFix {
        private static readonly LogWrapper Logger = LogWrapper.Get("CharlatansNecklaceFix");

        public static void Configure() {
            try {
                // Fix for Locket Of Perfect Cantrips Enchant not working with Ignition
                var ignitionGuid = AbilityRefs.Ignition.ToString();

                FeatureConfigurator.For("08d677d6ed2c49b469e7bd1385826dc9")
                .EditComponent<AutoMetamagic>(
                    c => c.Abilities.Add(BlueprintTool.GetRef<BlueprintAbilityReference>(ignitionGuid))
                )
                .Configure();

                FeatureConfigurator.For("e2efab2d89e6e1a4993c81a6b098e670")
                .EditComponent<AutoMetamagic>(
                    c => c.Abilities.Add(BlueprintTool.GetRef<BlueprintAbilityReference>(ignitionGuid))
                )
                .Configure();

                FeatureConfigurator.For("9dcf0f276f741474cab1a6ad771c06a7")
                .EditComponent<AutoMetamagic>(
                    c => c.Abilities.Add(BlueprintTool.GetRef<BlueprintAbilityReference>(ignitionGuid))
                )
                .Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }
        }
    }
}