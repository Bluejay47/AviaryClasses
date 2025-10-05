using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Parts;
using System;

namespace AviaryClasses.Fix {
    /// <summary>
    /// Allows Magic Deceiver archetype to receive bonus spells from Red Salamandra Ring
    /// while maintaining the general restriction on AddSpellKnownTemporary effects.
    ///
    /// Root Cause: The Magic Deceiver spellbook has IsIgnoreAddSpellKnownTemporary set to true,
    /// which prevents ALL items from granting temporary spells. This is intentional to prevent
    /// overpowered Magic Fusion combinations, but Red Salamandra's fire spells are considered
    /// safe and thematically appropriate.
    ///
    /// The Fix: Patch the spellbook's IsIgnoreAddSpellKnownTemporary check to allow specific
    /// whitelisted features (like Red Salamandra) while blocking everything else.
    ///
    /// ========================================
    /// AVAILABLE TEMPORARY SPELL SOURCES:
    /// ========================================
    ///
    /// ELEMENTAL RINGS (Equipment - Chapters 3-4):
    /// - Red Salamandra (Fire) - GUID: 07d734e0740ee084593d3b9ce2e03c86 ✅ WHITELISTED
    ///   Spells: Fireball, Controlled Fireball, Firesnake, Hellfire Ray, Fire Storm, Fiery Body
    ///
    /// - Earth Unleashed (Earth) - GUID: f96cfd9ef0997d94b8801180ab9b4698
    ///   Spells: Spike Stones, Stone Skin, Stoneskin (Communal), Stone to Flesh, Earthquake
    ///
    /// - Boreal Might (Cold) - GUID: fcef74b5753dc9249b0b1879629fd73c
    ///   Spells: Ice Storm, Cone of Cold, Cold Ice Strike, Polar Ray, Polar Midnight
    ///
    /// - Dark Omen (Necromancy) - GUID: f5f62828b6ebaa14c94d9ff32b3692d0
    ///   Spells: Ray of Enfeeblement, Ghoul Touch, Vampiric Touch, Enervation, Waves of Fatigue, Circle of Death
    ///
    /// - Sacred Commandment - GUID: Check blueprints/Equipment/Rings/UniquePF2/Chapter4/SacredCommandmentFeature.json
    ///
    /// STORY/COMPANION FEATURES:
    /// - Ember's Storm of Burning Righteousness - GUID: ab6a8d5aca4b4d33bf40e725bdcf386f
    /// - Ember's Secret of Serenity - GUID: Check blueprints/Units/Companions/Ember/
    ///
    /// SPECIAL ITEMS:
    /// - Stormlord's Resolve (Backer Item) - GUID: Check blueprints/Equipment/SpecialEditions/UniquePF2/BackersItems/
    ///
    /// DLC/MYTHIC:
    /// - Superiority of Cold (DLC5) - GUID: Check blueprints/Mythic/DLC5ShardsAbilities/
    ///
    /// ========================================
    /// BALANCE CONSIDERATIONS:
    /// ========================================
    /// Before whitelisting an item, consider:
    /// 1. Spell complexity - Simple damage spells (Red Salamandra) are safer than complex buffs/debuffs
    /// 2. Magic Fusion potential - Could these spells create broken combinations?
    /// 3. Number of spells - Items granting many spells need more careful review
    /// 4. Spell levels - High-level spells may trivialize content when fused
    ///
    /// Remember: Layer 2 (MagicHackTemporarySpellFilter) prevents temporary spells
    /// from appearing in Magic Fusion UI as a safety net, but Layer 1 (this whitelist)
    /// is your primary balance control.
    /// </summary>
    [HarmonyPatch(typeof(UnitPartTemporarySpellsKnown), "RefreshKnownSpells")]
    public class MagicDeceiverRedSalamandraFix {
        private static readonly LogWrapper Logger = LogWrapper.Get("MagicDeceiverRedSalamandraFix");

        private const string MagicDeceiverSpellbookGuid = "587066af76a74f47a904bb017697ba08";

        // Whitelisted feature GUIDs that are allowed to grant temporary spells to Magic Deceiver
        // To add new items: Simply add the feature GUID to this array
        private static readonly string[] WhitelistedFeatureGuids = new string[] {
            "07d734e0740ee084593d3b9ce2e03c86", // Red Salamandra (Fire)
            // Add additional GUIDs here as needed
            // "f96cfd9ef0997d94b8801180ab9b4698", // Earth Unleashed (Earth)
            // "fcef74b5753dc9249b0b1879629fd73c", // Boreal Might (Cold)
            // "f5f62828b6ebaa14c94d9ff32b3692d0", // Dark Omen (Necromancy)
        };

        /// <summary>
        /// Patch RefreshKnownSpells to manually add whitelisted temporary spells for Magic Deceiver.
        /// This runs after the normal logic and adds spells that were skipped due to IsIgnoreAddSpellKnownTemporary.
        /// </summary>
        [HarmonyPostfix]
        public static void Postfix(UnitPartTemporarySpellsKnown __instance) {
            try {
                var owner = __instance.Owner;
                if (owner == null) {
                    return;
                }

                // Get the Magic Deceiver spellbook
                Spellbook magicDeceiverSpellbookInstance = null;
                foreach (var spellbook in owner.Spellbooks) {
                    if (spellbook.Blueprint.AssetGuid.ToString() == MagicDeceiverSpellbookGuid) {
                        magicDeceiverSpellbookInstance = spellbook;
                        break;
                    }
                }

                if (magicDeceiverSpellbookInstance == null) {
                    return; // This unit doesn't have Magic Deceiver spellbook
                }

                // Access the private m_Entries field to check for whitelisted features
                var entriesField = typeof(UnitPartTemporarySpellsKnown).GetField("m_Entries",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (entriesField == null) {
                    Logger.Error("Could not find m_Entries field");
                    return;
                }

                var entries = entriesField.GetValue(__instance);
                if (entries == null) {
                    return;
                }

                // Use reflection to access the list since we don't have direct access to the Entry type
                var entryList = entries as System.Collections.IList;
                if (entryList == null) {
                    return;
                }

                // Check each entry for whitelisted features
                foreach (var entry in entryList) {
                    if (entry == null) continue;

                    var entryType = entry.GetType();

                    // Component is a property, but Spells and Source are public fields
                    var componentProperty = entryType.GetProperty("Component");
                    var spellsField = entryType.GetField("Spells");
                    var sourceField = entryType.GetField("Source");

                    if (componentProperty == null || spellsField == null || sourceField == null) {
                        continue;
                    }

                    var component = componentProperty.GetValue(entry) as Kingmaker.UnitLogic.FactLogic.AddSpellKnownTemporary;
                    var spells = spellsField.GetValue(entry) as System.Collections.Generic.List<Kingmaker.UnitLogic.Abilities.AbilityData>;
                    var source = sourceField.GetValue(entry) as Kingmaker.EntitySystem.EntityFact;

                    if (component == null || spells == null || source == null) {
                        continue;
                    }

                    // Check if this entry is from a whitelisted feature
                    var sourceGuid = source.Blueprint?.AssetGuid.ToString();
                    if (!string.IsNullOrEmpty(sourceGuid) && Array.Exists(WhitelistedFeatureGuids, guid => guid == sourceGuid)) {
                        // Check if the spell was already added
                        bool alreadyAdded = false;
                        foreach (var existingSpell in spells) {
                            if (existingSpell?.Spellbook == magicDeceiverSpellbookInstance) {
                                alreadyAdded = true;
                                break;
                            }
                        }

                        // If not already added, add it now
                        if (!alreadyAdded && component.Spell != null) {
                            var abilityData = magicDeceiverSpellbookInstance.AddKnownTemporary(component.Level, component.Spell);
                            if (abilityData != null) {
                                spells.Add(abilityData);
                                Logger.Info($"Added {component.Spell.name} (level {component.Level}) to Magic Deceiver from {source.Blueprint.name}");
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Logger.Error($"MagicDeceiverRedSalamandraFix.RefreshKnownSpells_Postfix failed: {ex}");
            }
        }
    }
}
