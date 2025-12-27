using HarmonyLib;
using System.Reflection;
using UnityModManagerNet;
using BlueprintCore.Utils;
using AviaryClasses.Classes;
using AviaryClasses.Items;
using AviaryClasses.Fix;
using Kingmaker.PubSubSystem;
using BlueprintCore.Blueprints.Configurators.Root;
using System;
using Kingmaker.Blueprints.JsonSystem;


namespace AviaryClasses {

    public static class Main {

        private static readonly LogWrapper Logger = LogWrapper.Get("AviaryClassesMain");
        internal static Harmony HarmonyInstance;
        internal static UnityModManager.ModEntry ModEntry;
        public static bool enabled;
        private static bool patchesApplied;


        public static bool Load(UnityModManager.ModEntry modEntry) {

            LogWrapper.EnableInternalVerboseLogs();

            try {
                ModEntry = modEntry;

                modEntry.OnGUI = OnGUI;
                modEntry.OnToggle = OnToggle;
                HarmonyInstance = new Harmony(modEntry.Info.Id);
                enabled = modEntry.Enabled;
                if (enabled) {
                    HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
                    patchesApplied = true;
                }

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }

            return true;

        }

        public static void OnGUI(UnityModManager.ModEntry modEntry) {
        }

        public static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
            if (enabled == value) {
                return true;
            }
            enabled = value;

            try {
                if (enabled && !patchesApplied) {
                    HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
                    patchesApplied = true;
                } else if (!enabled && patchesApplied) {
                    HarmonyInstance.UnpatchAll(modEntry.Info.Id);
                    patchesApplied = false;
                }
            } catch (Exception ex) {
                Logger.Error("Failed to toggle patches.", ex);
            }
            return true;
        }

        [HarmonyPatch(typeof(BlueprintsCache))]
        static class BlueprintsCaches_Patch {
            private static bool Initialized = false;

            [HarmonyPriority(Priority.First)]
            [HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]
            static void Postfix() {
                try {
                    if (!Main.enabled) {
                        return;
                    }
                    if (Initialized) {
                        return;
                    }
                    Initialized = true;

                    string nameVal = LocalizationTool.GetString("Mod.Name");

                    LifeSensate.Configure();
                    //LingeringEnergiesBuff.Configure();
                    ArcaneSkirmisher.Configure();
                    KeenEyedAdventurer2.Configure();
                    Dustwalker.Configure();
                    BeastTamer2.Configure();
                    MonkArchetypeAlignmentFix.Configure();
                    CharlatansNecklaceFix.Configure();
                    TriceratopsStatuetteItemAlt.Configure();

                } catch (Exception ex) {
                    Logger.Error("Failed to initialize.", ex);
                }
            }

        }

    }

}
