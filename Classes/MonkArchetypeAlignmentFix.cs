using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.UnitLogic.Alignments;
using System;

namespace AviaryClasses.Classes {
    public class MonkArchetypeAlignmentFix {
        private static readonly LogWrapper Logger = LogWrapper.Get("MonkArchetypeAlignmentFix");

        public static void Configure() {
            try {
                Logger.Info("Starting Monk archetype alignment fix...");

                AddLawfulAlignmentToArchetype("5868fc82eb11a4244926363983897279", "ScaledFist");
                AddLawfulAlignmentToArchetype("f8767821ec805bf479706392fcc3394c", "Sensei");
                AddLawfulAlignmentToArchetype("8f3242d209994884bbb6c0999660f4df", "Traditional");
                AddLawfulAlignmentToArchetype("2b1a58a7917084f49b097e86271df21c", "ZenArcher");
                AddLawfulAlignmentToArchetype("fad7c56737ed12e42aacc330acc86428", "Sohei");
                AddLawfulAlignmentToArchetype("3b81e57a75299b74ab6b144e830864e9", "StudentOfStone");
                AddLawfulAlignmentToArchetype("dde7724382ae4f63aa9786cb9b3b64b2", "QuarterstaffMaster");

                Logger.Info("Monk archetype alignment fix completed successfully");

            } catch (Exception ex) {
                Logger.Error($"Error in MonkArchetypeAlignmentFix.Configure: {ex}");
            }
        }

        private static void AddLawfulAlignmentToArchetype(string archetypeGuid, string archetypeName) {
            try {
                Logger.Info($"Adding Lawful alignment requirement to {archetypeName} archetype ({archetypeGuid})");

                ArchetypeConfigurator.For(archetypeGuid)
                    .AddPrerequisiteAlignment(
                        AlignmentMaskType.Lawful,
                        archetypeAlignment: true
                    )
                    .Configure();

                Logger.Info($"Successfully added Lawful alignment requirement to {archetypeName}");

            } catch (Exception ex) {
                Logger.Error($"Failed to add Lawful alignment requirement to {archetypeName}: {ex}");
            }
        }
    }
}