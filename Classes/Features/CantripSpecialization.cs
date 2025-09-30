using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using Kingmaker.EntitySystem.Stats;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints.Classes;
using BlueprintCore.Blueprints.CustomConfigurators;
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

        public static void Configure() {
            try {
                BlueprintFeature baseFeature = BlueprintTool.Get<BlueprintFeature>(FeatureRefs.CantripMasteryFeature.ToString());
                FeatureConfigurator feature = FeatureConfigurator.New(featName, featGuid);

                feature.SetDescription(featureDescription);
                feature.SetDisplayName(featureName);
                feature.SetIsClassFeature(true);
                feature.SetIcon(baseFeature.m_Icon);

                var statConfig = ContextRankConfigs.StatBonus(stat: StatType.Intelligence, type: AbilityRankType.DamageDice, max: 15, min: 0).WithBonusValueProgression(0);
                feature.AddContextRankConfig(statConfig);

                feature.AddRecalculateOnStatChange(null, ComponentMerge.Replace, StatType.Intelligence, useKineticistMainStat: false);

                feature.AdditionalDiceOnDamage(
                    [AbilityRefs.Ignition.ToString(), AbilityRefs.RayOfFrost.ToString(), AbilityRefs.Jolt.ToString(), AbilityRefs.AcidSplash.ToString()],
                    abilityType: AbilityType.Spell,
                    applyCriticalModifier: true,
                    compareType: Kingmaker.UnitLogic.Mechanics.CompareOperation.Type.Equal,
                    checkWeaponType: false,
                    isOneAtack: false,
                    checkAbilityType: true,
                    checkSpellDescriptor: false,
                    checkEnergyDamageType: false,
                    energyType: Kingmaker.Enums.Damage.DamageEnergyType.Fire,
                    mainDamageTypeUse: true,
                    ignoreDamageFromThisFact: true
                );

                feature.EditComponent<AdditionalDiceOnDamage>(c => c.m_DamageEntriesUse = Kingmaker.UnitLogic.Mechanics.Components.AdditionalDiceOnDamage.DamageEntriesUse.Simple);
                feature.EditComponent<AdditionalDiceOnDamage>(c => c.MainDamageTypeUse = true);

                feature.EditComponent<AdditionalDiceOnDamage>(c => c.DiceValue.BonusValue.Property = UnitProperty.StatBonusIntelligence);
                feature.EditComponent<AdditionalDiceOnDamage>(c => c.DiceValue.BonusValue.ValueType = Kingmaker.UnitLogic.Mechanics.ContextValueType.Rank);
                feature.EditComponent<AdditionalDiceOnDamage>(c => c.DiceValue.BonusValue.ValueRank = Kingmaker.Enums.AbilityRankType.DamageDice);
                feature.EditComponent<AdditionalDiceOnDamage>(c => c.DiceValue.BonusValue.ValueShared = Kingmaker.UnitLogic.Abilities.AbilitySharedValue.Damage);
                feature.EditComponent<AdditionalDiceOnDamage>(c => c.DiceValue.BonusValue.m_AbilityParameter = Kingmaker.UnitLogic.Mechanics.AbilityParameterType.Level);
                feature.EditComponent<AdditionalDiceOnDamage>(c => c.DiceValue.BonusValue.Value = 0);

                feature.EditComponent<AdditionalDiceOnDamage>(c => c.TargetValue.ValueType = Kingmaker.UnitLogic.Mechanics.ContextValueType.Rank);
                feature.EditComponent<AdditionalDiceOnDamage>(c => c.TargetValue.ValueRank = Kingmaker.Enums.AbilityRankType.Default);
                feature.EditComponent<AdditionalDiceOnDamage>(c => c.TargetValue.ValueShared = Kingmaker.UnitLogic.Abilities.AbilitySharedValue.Damage);
                feature.EditComponent<AdditionalDiceOnDamage>(c => c.TargetValue.m_AbilityParameter = Kingmaker.UnitLogic.Mechanics.AbilityParameterType.Level);

                feature.Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }
        }
    }
}