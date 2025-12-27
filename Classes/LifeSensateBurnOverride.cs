using HarmonyLib;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Class.Kineticist;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic;
using Kingmaker.PubSubSystem;
using System;


namespace AviaryClasses.Classes {

    // UpdateBurnResourceIndicator patch removed - buffer tracking now handled by LingeringEnergiesBuff

    [HarmonyPatch(typeof(UnitPartKineticist), "AcceptBurn")]
    public static class AcceptBurnPatch {

        private static readonly LogWrapper Logger = LogWrapper.Get("AcceptBurnPatch");

        [HarmonyPostfix]
        public static void Postfix(UnitPartKineticist __instance, int burn, AbilityData ability, ref int __result) {

            //BlueprintArchetype lifeSensate = ResourcesLibrary.TryGetBlueprint<BlueprintArchetype>(LifeSensate.featGuid);

            if (ability.Caster.Progression.IsArchetype(LifeSensate.archetypeRef)) {

                UnitAbilityResource resource = ability.Caster.Resources.GetResource(LingeringEnergiesResource.ResourcePool);

                if (resource!= null && resource.Amount > 0) {

                    int healedAmount = Math.Min (burn, resource.Amount);
                    ability.Caster.Resources.Spend(LingeringEnergiesResource.ResourcePool, healedAmount);
                    __instance.HealBurn(healedAmount);

                }

                EventBus.RaiseEvent<IKineticistGlobalHandler>(delegate (IKineticistGlobalHandler h) {
                        h.HandleKineticistBurnValueChanged(__instance, 0, ability);
                }, true);

            }

        }

    }


    [HarmonyPatch(typeof(UnitAbilityResourceCollection), "Restore", new Type[] { typeof(BlueprintScriptableObject), typeof(int), typeof(bool) })]
    public static class ResourceRestorePatch {

        private static readonly LogWrapper Logger = LogWrapper.Get("ResourceRestorePatch");

        [HarmonyPostfix]
        public static void Postfix(UnitAbilityResourceCollection __instance, BlueprintScriptableObject blueprint) {


            // Check if this is the Lingering Energies resource being restored on a Life Sensate character
            if (blueprint == LingeringEnergiesResource.ResourcePool) {

                UnitDescriptor owner = __instance.m_Owner;
                if (owner != null && owner.Progression.IsArchetype(LifeSensate.archetypeRef)) {

                    var resource = owner.Resources.GetResource(LingeringEnergiesResource.ResourcePool);

                    // Find the kineticist part and trigger a burn display update
                    var kineticistPart = owner.Get<UnitPartKineticist>();
                    if (kineticistPart != null) {
                        // Trigger burn display update by raising the kineticist burn value changed event
                        EventBus.RaiseEvent<IKineticistGlobalHandler>(delegate (IKineticistGlobalHandler h) {
                            h.HandleKineticistBurnValueChanged(kineticistPart, 0, null);
                        }, true);

                    }
                }
            }
        }
    }

}