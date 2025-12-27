using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using Kingmaker.EntitySystem.Stats;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints.Classes;
using BlueprintCore.Blueprints.CustomConfigurators;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Mechanics.Properties;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.Blueprints;
using System;
using System.Collections.Generic;

namespace AviaryClasses.Classes.Features {
    public class CantripSpecialization {
        private static readonly LogWrapper Logger = LogWrapper.Get("CantripSpecialization");

        public static readonly string featName = "CantripSpecialization";
        public static readonly string featGuid = "3ffc60b2-6cb3-4c97-9be9-f3e74b100d53";

        public static readonly string featureName = featName + ".Name";
        public static readonly string featureDescription = featName + ".Description";
        public static readonly string baseBonusBuffName = "CantripSpecializationBonusBuff";
        public static readonly string baseBonusBuffGuid = "e460f58d-d3a0-4df2-8cac-db021fd4c616";
        public static readonly string fortuneBonusBuffName = "CantripSpecializationFortuneBonusBuff";
        public static readonly string fortuneBonusBuffGuid = "6a8c2dc3-3989-4525-b290-f8d6fd95c478";

        public static void Configure(LuckLevels luckLevel) {
            try {
                BlueprintFeature baseFeature = BlueprintTool.Get<BlueprintFeature>(FeatureRefs.CantripMasteryFeature.ToString());
                FeatureConfigurator feature = FeatureConfigurator.New(featName, featGuid);

                feature.SetDescription(featureDescription);
                feature.SetDisplayName(featureName);
                feature.SetIsClassFeature(true);
                feature.SetIcon(baseFeature.m_Icon);

                int[] luckBonus = [-2, -1, 0, 1, 5];
                int baseIndex = (int)luckLevel;
                int fortuneIndex = Math.Min(baseIndex + 2, (int)LuckLevels.EXTREME);
                int baseBonus = luckBonus[baseIndex];
                int fortuneBonus = luckBonus[fortuneIndex];
                var baseStatConfig = ContextRankConfigs.StatBonus(stat: StatType.Intelligence, type: AbilityRankType.DamageDice, max: 17, min: -2)
                    .WithBonusValueProgression(baseBonus);
                var fortuneStatConfig = ContextRankConfigs.StatBonus(stat: StatType.Intelligence, type: AbilityRankType.DamageDice, max: 17, min: -2)
                    .WithBonusValueProgression(fortuneBonus);

                BuffConfigurator.New(baseBonusBuffName, baseBonusBuffGuid)
                .SetFlags(BlueprintBuff.Flags.HiddenInUi)
                .SetStacking(StackingType.Replace)
                .AddContextRankConfig(baseStatConfig)
                .AddRecalculateOnStatChange(null, ComponentMerge.Replace, StatType.Intelligence, useKineticistMainStat: false)
                .AdditionalDiceOnDamage(
                    [AbilityRefs.Ignition.ToString(), AbilityRefs.RayOfFrost.ToString(), AbilityRefs.Jolt.ToString(), AbilityRefs.AcidSplash.ToString()],
                    abilityType: AbilityType.Spell,
                    applyCriticalModifier: true,
                    compareType: Kingmaker.UnitLogic.Mechanics.CompareOperation.Type.Equal,
                    checkWeaponType: false,
                    isOneAtack: false,
                    checkAbilityType: true,
                    checkSpellDescriptor: false,
                    checkSpellParent: true,
                    checkEnergyDamageType: false,
                    energyType: Kingmaker.Enums.Damage.DamageEnergyType.Fire,
                    mainDamageTypeUse: true,
                    ignoreDamageFromThisFact: true
                )
                .EditComponent<AdditionalDiceOnDamage>(c => c.m_DamageEntriesUse = Kingmaker.UnitLogic.Mechanics.Components.AdditionalDiceOnDamage.DamageEntriesUse.Simple)
                .EditComponent<AdditionalDiceOnDamage>(c => c.MainDamageTypeUse = true)
                .EditComponent<AdditionalDiceOnDamage>(c => c.DiceValue.BonusValue.Property = UnitProperty.StatBonusIntelligence)
                .EditComponent<AdditionalDiceOnDamage>(c => c.DiceValue.BonusValue.ValueType = Kingmaker.UnitLogic.Mechanics.ContextValueType.Rank)
                .EditComponent<AdditionalDiceOnDamage>(c => c.DiceValue.BonusValue.ValueRank = Kingmaker.Enums.AbilityRankType.DamageDice)
                .EditComponent<AdditionalDiceOnDamage>(c => c.DiceValue.BonusValue.ValueShared = Kingmaker.UnitLogic.Abilities.AbilitySharedValue.Damage)
                .EditComponent<AdditionalDiceOnDamage>(c => c.DiceValue.BonusValue.m_AbilityParameter = Kingmaker.UnitLogic.Mechanics.AbilityParameterType.Level)
                .EditComponent<AdditionalDiceOnDamage>(c => c.DiceValue.BonusValue.Value = 0)
                .Configure();

                BuffConfigurator.New(fortuneBonusBuffName, fortuneBonusBuffGuid)
                .SetFlags(BlueprintBuff.Flags.HiddenInUi)
                .SetStacking(StackingType.Replace)
                .AddContextRankConfig(fortuneStatConfig)
                .AddRecalculateOnStatChange(null, ComponentMerge.Replace, StatType.Intelligence, useKineticistMainStat: false)
                .AdditionalDiceOnDamage(
                    [AbilityRefs.Ignition.ToString(), AbilityRefs.RayOfFrost.ToString(), AbilityRefs.Jolt.ToString(), AbilityRefs.AcidSplash.ToString()],
                    abilityType: AbilityType.Spell,
                    applyCriticalModifier: true,
                    compareType: Kingmaker.UnitLogic.Mechanics.CompareOperation.Type.Equal,
                    checkWeaponType: false,
                    isOneAtack: false,
                    checkAbilityType: true,
                    checkSpellDescriptor: false,
                    checkSpellParent: true,
                    checkEnergyDamageType: false,
                    energyType: Kingmaker.Enums.Damage.DamageEnergyType.Fire,
                    mainDamageTypeUse: true,
                    ignoreDamageFromThisFact: true
                )
                .EditComponent<AdditionalDiceOnDamage>(c => c.m_DamageEntriesUse = Kingmaker.UnitLogic.Mechanics.Components.AdditionalDiceOnDamage.DamageEntriesUse.Simple)
                .EditComponent<AdditionalDiceOnDamage>(c => c.MainDamageTypeUse = true)
                .EditComponent<AdditionalDiceOnDamage>(c => c.DiceValue.BonusValue.Property = UnitProperty.StatBonusIntelligence)
                .EditComponent<AdditionalDiceOnDamage>(c => c.DiceValue.BonusValue.ValueType = Kingmaker.UnitLogic.Mechanics.ContextValueType.Rank)
                .EditComponent<AdditionalDiceOnDamage>(c => c.DiceValue.BonusValue.ValueRank = Kingmaker.Enums.AbilityRankType.DamageDice)
                .EditComponent<AdditionalDiceOnDamage>(c => c.DiceValue.BonusValue.ValueShared = Kingmaker.UnitLogic.Abilities.AbilitySharedValue.Damage)
                .EditComponent<AdditionalDiceOnDamage>(c => c.DiceValue.BonusValue.m_AbilityParameter = Kingmaker.UnitLogic.Mechanics.AbilityParameterType.Level)
                .EditComponent<AdditionalDiceOnDamage>(c => c.DiceValue.BonusValue.Value = 0)
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

                feature.AddFactContextActions(
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
                );

                feature.Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }
        }
    }
}
