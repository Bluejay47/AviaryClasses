using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Parts;
using System;

namespace AviaryClasses.Fix {
  /// <summary>
  /// Fixes an old vanilla bug where Arcane Weapon Enchantment doesn't work when
  /// Enduring Blade is active and the Magus is mounted.
  ///
  /// Root Cause: When Enduring Blade is active and mounted, the ability runs two actions:
  /// 1. ContextActionWeaponEnchantPool - Enchants rider's weapon and clears the pool first
  /// 2. ContextActionsOnMount -> ContextActionShieldWeaponEnchantPool - Tries to enchant mount's
  ///    weapon (for Arcane Rider archetype) but ALSO clears the pool, wiping out the rider's
  ///    enchantments that were just applied milliseconds earlier.
  ///
  /// The Fix: Intercept ClearEnchantPool when called from ContextActionShieldWeaponEnchantPool
  /// and prevent it from clearing when the rider has Enduring Blade active and is mounted.
  /// This preserves the rider's weapon enchantments.
  /// </summary>
  [HarmonyPatch]
  public class EnduringBladeMountFix {
    private static readonly LogWrapper Logger = LogWrapper.Get("AviaryClasses");

    // Known blueprint GUIDs
    private const string EnduringBladeBuffGuid = "3c2fe8e0374d28d4185355121f4c4544";
 
    /// <summary>
    /// Prevent ContextActionShieldWeaponEnchantPool from clearing the rider's weapon enchantments
    /// when enchanting the mount's weapon. This is the core fix for the Enduring Blade + Mounted bug.
    /// </summary>
    [HarmonyPatch(typeof(UnitPartEnchantPoolData), nameof(UnitPartEnchantPoolData.ClearEnchantPool))]
    [HarmonyPrefix]
    public static bool ClearEnchantPool_Prefix(UnitPartEnchantPoolData __instance, EnchantPoolType type) {
      try {
        if (type != EnchantPoolType.ArcanePool) {
          return true; // Allow clearing for other pool types
        }

        // Check if this is being called from ContextActionShieldWeaponEnchantPool
        var stackTrace = new System.Diagnostics.StackTrace();
        bool isFromShieldWeaponEnchant = false;
        for (int i = 0; i < stackTrace.FrameCount; i++) {
          var method = stackTrace.GetFrame(i).GetMethod();
          if (method?.DeclaringType?.Name == "ContextActionShieldWeaponEnchantPool") {
            isFromShieldWeaponEnchant = true;
            break;
          }
        }

        if (isFromShieldWeaponEnchant) {
          var unit = __instance.Owner;
          var riderPart = unit?.Get<UnitPartRider>();
          var enduringBladeBuff = BlueprintTool.Get<BlueprintBuff>(EnduringBladeBuffGuid);

          // If mounted with Enduring Blade active, prevent clearing the rider's pool
          if (riderPart != null && riderPart.SaddledUnit != null &&
              enduringBladeBuff != null && unit.Descriptor.Buffs.HasFact(enduringBladeBuff)) {
            return false; // Skip the clear to preserve rider weapon enchantments
          }
        }

        return true; // Allow normal clearing
      } catch (Exception e) {
        Logger.Error($"EnduringBladeMountFix.ClearEnchantPool_Prefix failed: {e}");
        return true; // On error, allow normal behavior
      }
    }
  }
}
