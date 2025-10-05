using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.PubSubSystem;
using Kingmaker.UI.Common;
using Kingmaker.UI.MVVM._VM.ServiceWindows.Spellbook;
using Kingmaker.UI.MVVM._ConsoleView.ServiceWindows.Spellbook;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AviaryClasses.Fix {
    /// <summary>
    /// Prevents temporary spells (from items) from being available in Magic Fusion UI.
    ///
    /// Root Cause: Temporary spells from items have not been tested with Magic Fusion and could
    /// create overpowered or broken combinations. This is a defense-in-depth measure that works
    /// in conjunction with the whitelist system.
    ///
    /// The Fix: Track when Magic Hack mode is active via SpellbookVM.MagicalHackBuilderMode,
    /// and filter temporary spells from the UI only when that mode is active.
    /// </summary>
    [HarmonyPatch]
    public class MagicHackTemporarySpellFilter {
        private static readonly LogWrapper Logger = LogWrapper.Get("MagicHackTemporarySpellFilter");

        private const string MagicDeceiverSpellbookGuid = "587066af76a74f47a904bb017697ba08";

        // Track whether Magic Hack mode is currently active
        private static bool IsMagicHackModeActive = false;

        static MagicHackTemporarySpellFilter() {
            Logger.Info("MagicHackTemporarySpellFilter class loaded");
        }

        /// <summary>
        /// Patch to track when Magic Hack mode is activated/deactivated.
        /// Hooks into the method that's called when the Magic Fusion button is clicked.
        /// </summary>
        [HarmonyPatch(typeof(Kingmaker.UI.MVVM._VM.ServiceWindows.Spellbook.SpellbookVM), "OnMagicHackModeSwitch")]
        [HarmonyPostfix]
        public static void OnMagicHackModeSwitch_Postfix(Kingmaker.UI.MVVM._VM.ServiceWindows.Spellbook.SpellbookVM __instance) {
            try {
                // The method toggles the mode, so read the current state after the toggle
                IsMagicHackModeActive = __instance.MagicalHackBuilderMode.Value;
                Logger.Info($"Magic Hack mode {(IsMagicHackModeActive ? "activated" : "deactivated")}");

                // Force UI refresh by accessing m_CurrentLevel and triggering its subscription
                if (__instance.CurrentSpellbookLevel != null) {
                    var currentLevel = __instance.CurrentSpellbookLevel.Value;
                    if (currentLevel != null) {
                        // Access the private m_CurrentLevel field in SpellbookKnownSpellsVM
                        var knownSpellsVM = __instance.SpellbookKnownSpellsVM;
                        var m_CurrentLevelField = knownSpellsVM?.GetType().GetField("m_CurrentLevel",
                            BindingFlags.NonPublic | BindingFlags.Instance);

                        if (m_CurrentLevelField != null) {
                            // Get the reactive property
                            var m_CurrentLevel = m_CurrentLevelField.GetValue(knownSpellsVM);

                            // Trigger by setting to null then back (forces subscription to fire)
                            var setValueMethod = m_CurrentLevel?.GetType().GetMethod("set_Value");
                            var getValueMethod = m_CurrentLevel?.GetType().GetMethod("get_Value");

                            if (setValueMethod != null && getValueMethod != null) {
                                setValueMethod.Invoke(m_CurrentLevel, new object[] { null });
                                setValueMethod.Invoke(m_CurrentLevel, new object[] { currentLevel });
                                Logger.Info($"Forced UI refresh by toggling m_CurrentLevel");
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Logger.Error($"OnMagicHackModeSwitch_Postfix failed: {ex}");
            }
        }

        /// <summary>
        /// Reset the Magic Hack mode flag when the spellbook is closed.
        /// This ensures the flag is in sync when the spellbook is reopened (always starts closed).
        /// </summary>
        [HarmonyPatch(typeof(Kingmaker.UI.MVVM._VM.ServiceWindows.Spellbook.SpellbookVM), "DisposeImplementation")]
        [HarmonyPostfix]
        public static void DisposeImplementation_Postfix() {
            try {
                if (IsMagicHackModeActive) {
                    IsMagicHackModeActive = false;
                    Logger.Info("Reset Magic Hack mode flag on spellbook close");
                }
            } catch (Exception ex) {
                Logger.Error($"DisposeImplementation_Postfix failed: {ex}");
            }
        }

        /// <summary>
        /// Filter temporary spells from the UI spell list when Magic Hack mode is active.
        /// </summary>
        [HarmonyPatch(typeof(UIUtilityUnit), "GetKnownSpellsForLevel")]
        [HarmonyPostfix]
        public static void GetKnownSpellsForLevel_Postfix(int level, Spellbook spellbook, ref List<AbilityData> __result) {
            try {
                // Only filter when Magic Hack mode is active
                if (!IsMagicHackModeActive) {
                    return;
                }

                // Only filter for Magic Deceiver spellbook
                if (spellbook == null || spellbook.Blueprint.AssetGuid.ToString() != MagicDeceiverSpellbookGuid) {
                    return;
                }

                if (__result == null) {
                    return;
                }

                // Filter out temporary spells - create new list to avoid modifying shared references
                int originalCount = __result.Count;
                __result = __result.Where(spell => spell != null && !spell.IsTemporary).ToList();
                int filteredCount = originalCount - __result.Count;

                if (filteredCount > 0) {
                    Logger.Info($"Filtered {filteredCount} temporary spell(s) from Magic Fusion UI at level {level}");
                }
            } catch (Exception ex) {
                Logger.Error($"GetKnownSpellsForLevel_Postfix failed: {ex}");
            }
        }
    }
}
