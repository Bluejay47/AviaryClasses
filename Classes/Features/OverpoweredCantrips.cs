using AviaryClasses;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints.Classes;
using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
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
using BlueprintCore.Utils.Types;

namespace AviaryClasses.Classes.Features {
    public class OverpoweredCantrips {
        private static readonly LogWrapper Logger = LogWrapper.Get("OverpoweredCantrips");

        public static readonly string featName = "OverpoweredCantrips";
        public static readonly string featGuid = "171662c8-b5eb-45e7-9008-4443c6437022";

        public static readonly string buffName = "OverpoweredCantripsBuff";
        public static readonly string buffGuid = "1d4af546-8751-4cf6-8e14-3116a408ac53";

        public static readonly string baseBonusBuffName = "OverpoweredCantripsBonusBuff";
        public static readonly string baseBonusBuffGuid = "64d0ead1-73e4-416b-9514-f0f9947839f9";

        public static readonly string fortuneBonusBuffName = "OverpoweredCantripsFortuneBonusBuff";
        public static readonly string fortuneBonusBuffGuid = "05225bdb-79c5-4e96-b290-f9260922452b";

        public static readonly string abilityName = "OverpoweredCantripsAbility";
        public static readonly string abilityGuid = "9edbb2e0-caac-4d92-b6af-c26ad810c903";

        public static void Configure(LuckLevels luckLevel) {
            try {
                BlueprintFeature baseAbility = BlueprintTool.Get<BlueprintFeature>(FeatureRefs.BolsteredSpellFeat.ToString());
                var customIcon = AviaryClasses.Utils.LoadIcon("BolsteredCantrips.png", baseAbility.m_Icon);

                int[] damageBonus = [-1, 0, 1, 1, 5];
                int baseIndex = (int)luckLevel;
                int fortuneIndex = Math.Min(baseIndex + 2, (int)LuckLevels.EXTREME);
                int baseBonus = damageBonus[baseIndex];
                int fortuneBonus = damageBonus[fortuneIndex];

                BuffConfigurator.New(baseBonusBuffName, baseBonusBuffGuid)
                .SetFlags(BlueprintBuff.Flags.HiddenInUi)
                .SetStacking(StackingType.Replace)
                .AddDiceDamageBonusOnSpell(
                    spells: [AbilityRefs.Ignition.ToString(), AbilityRefs.RayOfFrost.ToString(), AbilityRefs.Jolt.ToString(), AbilityRefs.AcidSplash.ToString()],
                    value: ContextValues.Constant(baseBonus),
                    useContextBonus: true
                )
                .Configure();

                BuffConfigurator.New(fortuneBonusBuffName, fortuneBonusBuffGuid)
                .SetFlags(BlueprintBuff.Flags.HiddenInUi)
                .SetStacking(StackingType.Replace)
                .AddDiceDamageBonusOnSpell(
                    spells: [AbilityRefs.Ignition.ToString(), AbilityRefs.RayOfFrost.ToString(), AbilityRefs.Jolt.ToString(), AbilityRefs.AcidSplash.ToString()],
                    value: ContextValues.Constant(fortuneBonus),
                    useContextBonus: true
                )
                .Configure();

                ConditionsBuilder fortuneCondition = ConditionsBuilder.New()
                .CasterHasFact(BuffRefs.WitchHexFortuneBuff.ToString());

                var applyBaseBonus = ActionsBuilder.New()
                .RemoveBuff(fortuneBonusBuffGuid, toCaster: true)
                .ApplyBuffPermanent(baseBonusBuffGuid, toCaster: true);

                var applyFortuneBonus = ActionsBuilder.New()
                .RemoveBuff(baseBonusBuffGuid, toCaster: true)
                .ApplyBuffPermanent(fortuneBonusBuffGuid, toCaster: true);

                var clearBonus = ActionsBuilder.New()
                .RemoveBuff(baseBonusBuffGuid, toCaster: true)
                .RemoveBuff(fortuneBonusBuffGuid, toCaster: true);

                // Overpowered Cantrips Toggle Buff
                BuffConfigurator.New(buffName, buffGuid)
                .SetFlags(BlueprintBuff.Flags.HiddenInUi)
                .AddFactContextActions(
                    activated: ActionsBuilder.New()
                        .Conditional(
                            conditions: fortuneCondition,
                            ifTrue: applyFortuneBonus,
                            ifFalse: applyBaseBonus
                        ),
                    deactivated: clearBonus,
                    dispose: clearBonus
                )
                .AddFactsChangeTrigger(
                    checkedFacts: [BuffRefs.WitchHexFortuneBuff.ToString()],
                    onFactGainedActions: applyFortuneBonus,
                    onFactLostActions: applyBaseBonus
                )
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
                .Configure();

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

                CantripSplashFactory.ConfigureAllSplashExtensions(splashDice, splashCondition, luckLevel);

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }
        }
    }
}
