using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Enums;
using System;
using AviaryClasses;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Blueprints.Classes;
using BlueprintCore.Blueprints.CustomConfigurators;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Properties;
using Kingmaker.Utility;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Mechanics.Actions;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils.Assets;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.ElementsSystem;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.RuleSystem.Rules.Damage;
using BlueprintCore.Actions.Builder.BasicEx;
using Kingmaker.Designers.EventConditionActionSystem.Evaluators;
using BlueprintCore.Blueprints.Configurators.Items;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Blueprints;
using System;
using BlueprintCore.Actions.Builder.AVEx;

namespace AviaryClasses.Classes.Features {
    public class ReleaseTheBees {
        private static readonly LogWrapper Logger = LogWrapper.Get("ReleaseTheBees");

        public static readonly string spellName = "ReleaseTheBees";
        public static readonly string spellGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac3008";

        private static readonly string releaseTheBeesFeatureName = "BeastTamerReleaseTheBeesFeature";
        private static readonly string releaseTheBeesFeatureGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac3009";

        public static void Configure() {
            try {
                // Load custom icon with fallback to Jolt icon
                var baseAbility = BlueprintTool.Get<BlueprintAbility>(AbilityRefs.Jolt.ToString());
                var customIcon = AviaryClasses.Utils.LoadIcon("releasebees.png", baseAbility.m_Icon);

                // Create context dice value: 1d3 per caster level + Charisma bonus
                var damageValue = new ContextDiceValue() {
                    DiceType = DiceType.D3,
                    DiceCountValue = new ContextValue() {
                        ValueType = ContextValueType.Rank,
                        ValueRank = Kingmaker.Enums.AbilityRankType.Default
                    },
                    BonusValue = new ContextValue() {
                        ValueType = ContextValueType.Shared,
                        ValueShared = AbilitySharedValue.StatBonus
                    }
                };

                AbilityConfigurator.New(spellName, spellGuid)
                    .SetDisplayName(spellName + ".Name")
                    .SetDescription(spellName + ".Description")
                    .SetIcon(customIcon)
                    .SetType(AbilityType.Spell)
                    .SetRange(AbilityRange.Medium)
                    .SetCanTargetEnemies(true)
                    .SetCanTargetFriends(false)
                    .SetCanTargetSelf(false)
                    .SetSpellResistance(false)
                    .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
                    .SetAnimation(Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell.CastAnimationStyle.Point)
                    .SetActionType(Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Standard)
                    .SetAvailableMetamagic(
                        Metamagic.Empower,
                        Metamagic.Maximize,
                        Metamagic.Quicken,
                        Metamagic.Heighten,
                        Metamagic.Reach
                    )
                    .AddAbilityEffectRunAction(
                        actions: ActionsBuilder.New()
                            .DealDamage(
                                damageType: DamageTypes.Force(),
                                value: damageValue
                            )
                    )
                    .AddAbilityDeliverProjectile(
                        projectiles: new() { ElementalEffects.SwarmInfest },
                        type: AbilityProjectileType.Simple
                    )
                    .AddContextRankConfig(
                        ContextRankConfigs.CasterLevel()
                            .WithCustomProgression(
                                (1, 1), (2, 1),   // Levels 1-2: 1d3
                                (3, 2), (4, 2),   // Levels 3-4: 2d3
                                (5, 3), (6, 3),   // Levels 5-6: 3d3
                                (7, 4), (8, 4),   // Levels 7-8: 4d3
                                (9, 5), (10, 5),  // Levels 9-10: 5d3
                                (11, 6), (12, 6), // Levels 11-12: 6d3
                                (13, 7), (14, 7), // Levels 13-14: 7d3
                                (15, 8), (16, 8), // Levels 15-16: 8d3
                                (17, 9), (18, 9), // Levels 17-18: 9d3
                                (19, 10), (20, 10) // Levels 19-20: 10d3
                            )
                    )
                    .AddContextCalculateSharedValue(
                        valueType: AbilitySharedValue.StatBonus,
                        value: new ContextDiceValue() {
                            DiceType = DiceType.Zero,
                            DiceCountValue = 0,
                            BonusValue = new ContextValue() {
                                ValueType = ContextValueType.CasterProperty,
                                Property = Kingmaker.UnitLogic.Mechanics.Properties.UnitProperty.StatBonusCharisma
                            }
                        }
                    )
                    .AddSpellComponent(SpellSchool.Conjuration)
                    .Configure();

                Logger.Info("Release The Bees spell configured successfully");

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }
        }


        public static string CreateReleaseTheBeesFeature() {
            try {
                // Load custom icon with fallback to Jolt icon
                var baseAbility = BlueprintTool.Get<BlueprintAbility>(AbilityRefs.Jolt.ToString());
                var customIcon = AviaryClasses.Utils.LoadIcon("releasebees.png", baseAbility.m_Icon);

                // Create a feature that grants the Release The Bees spell as a known spell
                FeatureConfigurator.New(releaseTheBeesFeatureName, releaseTheBeesFeatureGuid)
                    .SetDisplayName(releaseTheBeesFeatureName + ".Name")
                    .SetDescription(releaseTheBeesFeatureName + ".Description")
                    .SetIcon(customIcon)
                    .SetIsClassFeature(true)
                    .SetHideInUI(false)
                    .AddKnownSpell(
                        characterClass: CharacterClassRefs.BardClass.ToString(),
                        spell: spellGuid,
                        spellLevel: 0
                    )
                    .Configure();

                Logger.Info("Release The Bees feature configured successfully");
                return releaseTheBeesFeatureGuid;

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
                return releaseTheBeesFeatureGuid;
            }
        }
    }
}
