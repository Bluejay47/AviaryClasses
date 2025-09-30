using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Localization;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Class.Kineticist;
using AviaryClasses.Classes;

namespace AviaryClasses.Classes {

    public static class LingeringEnergiesBuff {

        private static readonly LogWrapper Logger = LogWrapper.Get("LingeringEnergiesBuff");

        // Old name: LingeringEnergiesBuffTracker (caused conflict)
        public static readonly string BuffName = "AviaryLingeringEnergiesTracker";
        // Reverted to original GUID to avoid save game issues
        public static readonly string BuffGuid = "a1b2c3d4-e5f6-7890-abcd-123456789012";

        public static void Configure() {
            try {
                // Get a base feature for fallback icon
                BlueprintFeature baseFeature = BlueprintTool.Get<BlueprintFeature>(FeatureRefs.BolsteredSpellFeat.ToString());
                var customIcon = AviaryClasses.Utils.LoadIcon("life_sensate_burn_icon_medium.png", baseFeature.m_Icon);

                // Create base buff configuration
                BuffConfigurator.New(BuffName, BuffGuid)
                    .SetDisplayName(LocalizationTool.GetString("LingeringEnergiesBuff.Name"))
                    .SetDescription(LocalizationTool.GetString("LingeringEnergiesBuff.Description"))
                    .SetIcon(customIcon)
                    .SetStacking(StackingType.Replace) // Replace instead of stack to avoid duplicates
                    .SetRanks(20) // Set high max ranks to support high Intelligence characters
                    .AddToFlags(BlueprintBuff.Flags.StayOnDeath) // Persist through death
                    .Configure();

            } catch (System.Exception ex) {
                Logger.Error($"Failed to configure Lingering Energies Buff: {ex}");
            }
        }

        // Update the buff to match current buffer amount
        public static void UpdateBuffCharges(UnitDescriptor unit, int currentCharges) {
            try {
                // Get actual max buffer from the resource (intelligence-based)
                var resource = unit.Resources.GetResource(LingeringEnergiesResource.ResourcePool);
                int maxCharges = resource?.GetMaxAmount(unit) ?? 4; // Fallback to 4 if resource not found


                var buffBlueprint = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>(BuffGuid);
                if (buffBlueprint == null) {
                    Logger.Error("Could not find Lingering Energies Buff blueprint");
                    return;
                }

                var existingBuff = unit.Buffs.GetBuff(buffBlueprint);

                if (currentCharges <= 0) {
                    // Remove buff if no charges
                    if (existingBuff != null) {
                        existingBuff.Remove();
                    }
                } else {
                    if (existingBuff != null) {
                        // Update existing buff rank instead of removing/re-adding
                        existingBuff.SetRank(currentCharges);
                    } else {
                        // Add new buff with current charges
                        var newBuff = unit.Buffs.AddBuff(buffBlueprint, unit.Unit, null);
                        if (newBuff != null) {
                            // Set rank to current charges (this should show as a number on the buff icon)
                            newBuff.SetRank(currentCharges);
                        } else {
                            Logger.Error("Failed to add Lingering Energies buff");
                        }
                    }
                }

            } catch (System.Exception ex) {
                Logger.Error($"Failed to update Lingering Energies buff: {ex}");
            }
        }
    }

    // Patch to update the buff whenever resources change
    [HarmonyPatch(typeof(UnitAbilityResourceCollection), "Restore", new System.Type[] { typeof(BlueprintScriptableObject), typeof(int), typeof(bool) })]
    public static class LingeringEnergiesBuffPatch {

        private static readonly LogWrapper Logger = LogWrapper.Get("LingeringEnergiesBuffPatch");

        [HarmonyPostfix]
        public static void Postfix(UnitAbilityResourceCollection __instance, BlueprintScriptableObject blueprint) {

            try {
                // Only update for Lingering Energies resource on Life Sensate characters
                if (blueprint == LingeringEnergiesResource.ResourcePool) {

                    UnitDescriptor owner = __instance.m_Owner;
                    if (owner != null && owner.Progression.IsArchetype(LifeSensate.archetypeRef)) {

                        var resource = owner.Resources.GetResource(LingeringEnergiesResource.ResourcePool);
                        int currentCharges = resource?.Amount ?? 0;

                        LingeringEnergiesBuff.UpdateBuffCharges(owner, currentCharges);
                    }
                }

            } catch (System.Exception ex) {
                Logger.Error($"LingeringEnergiesBuffPatch error: {ex}");
            }
        }
    }

    // Patch to update the buff whenever resources are spent
    [HarmonyPatch(typeof(UnitPartKineticist), "AcceptBurn")]
    public static class LingeringEnergiesBuffSpendPatch {

        private static readonly LogWrapper Logger = LogWrapper.Get("LingeringEnergiesBuffSpendPatch");

        [HarmonyPostfix]
        public static void Postfix(UnitPartKineticist __instance, int burn, Kingmaker.UnitLogic.Abilities.AbilityData ability, ref int __result) {

            try {
                if (ability.Caster.Progression.IsArchetype(LifeSensate.archetypeRef)) {

                    var resource = ability.Caster.Resources.GetResource(LingeringEnergiesResource.ResourcePool);
                    int currentCharges = resource?.Amount ?? 0;

                    LingeringEnergiesBuff.UpdateBuffCharges(ability.Caster, currentCharges);
                }

            } catch (System.Exception ex) {
                Logger.Error($"LingeringEnergiesBuffSpendPatch error: {ex}");
            }
        }
    }
}