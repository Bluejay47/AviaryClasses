using AviaryClasses;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Abilities;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using System;
using System.Collections.Generic;

namespace AviaryClasses.Classes.Features {
    public class OverpoweredCantrips {
        private static readonly LogWrapper Logger = LogWrapper.Get("OverpoweredCantrips");

        public static readonly string featName = "OverpoweredCantrips";
        public static readonly string featGuid = "171662c8-b5eb-45e7-9008-4443c6437022";

        public static readonly string buffName = "OverpoweredCantripsBuff";
        public static readonly string buffGuid = "1d4af546-8751-4cf6-8e14-3116a408ac53";

        public static readonly string abilityName = "OverpoweredCantripsAbility";
        public static readonly string abilityGuid = "9edbb2e0-caac-4d92-b6af-c26ad810c903";

        public static void Configure() {
            try {
                BlueprintFeature baseAbility = BlueprintTool.Get<BlueprintFeature>(FeatureRefs.BolsteredSpellFeat.ToString());
                var customIcon = AviaryClasses.Utils.LoadIcon("BolsteredCantrips.png", baseAbility.m_Icon);

                // Overpowered Cantrips Toggle Buff
                BuffConfigurator.New(buffName, buffGuid)
                .SetFlags(BlueprintBuff.Flags.HiddenInUi)
                .Configure();

                var toggle = ActivatableAbilityConfigurator.New(abilityName, abilityGuid)
                .SetDisplayName(featName + ".Name")
                .SetDescription(featName + ".Description")
                .SetBuff(buffGuid)
                .SetIcon(customIcon)
                .Configure();

                FeatureConfigurator feature = FeatureConfigurator.New(featName, featGuid);

                feature.SetDescription(featName + ".Description")
                .SetDisplayName(featName + ".Name")
                .SetIsClassFeature(true)
                .AddFacts(new() { toggle })
                .SetIcon(customIcon)
                .AddDiceDamageBonusOnSpell(
                    spells: [AbilityRefs.Ignition.ToString(), AbilityRefs.RayOfFrost.ToString(), AbilityRefs.Jolt.ToString(), AbilityRefs.AcidSplash.ToString()],
                    value: 1
                )
                .Configure();

                // Configure splash extensions using factory pattern
                ContextDiceValue splashDice = new ContextDiceValue() {
                    DiceType = Kingmaker.RuleSystem.DiceType.D3,
                    DiceCountValue = new ContextValue() {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.Default,
                        ValueShared = AbilitySharedValue.Damage,
                        m_AbilityParameter = AbilityParameterType.Level
                    },
                    BonusValue = new ContextValue() {
                        ValueShared = AbilitySharedValue.Damage,
                        ValueRank = AbilityRankType.DamageDice,
                        m_AbilityParameter = AbilityParameterType.Level,
                        Property = Kingmaker.UnitLogic.Mechanics.Properties.UnitProperty.StatBonusIntelligence
                    },
                };

                ConditionsBuilder splashCondition = ConditionsBuilder.New()
                .CasterHasFact(buffGuid)
                .IsMainTarget()
                .Build();

                CantripSplashFactory.ConfigureAllSplashExtensions(splashDice, splashCondition);

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }
        }
    }
}
