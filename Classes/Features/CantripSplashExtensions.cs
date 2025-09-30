using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.BasicEx;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils.Types;
using Kingmaker.ElementsSystem;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;
using System;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;

using Kingmaker.EntitySystem.Stats;
using BlueprintCore.Utils;

using Kingmaker.Blueprints.Classes;
using BlueprintCore.Blueprints.CustomConfigurators;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Mechanics.Properties;

using Kingmaker.UnitLogic.Abilities.Blueprints;

using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Mechanics.Actions;

using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;

using BlueprintCore.Utils.Assets;

using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;

using Kingmaker.RuleSystem;

using Kingmaker.Designers.EventConditionActionSystem.Evaluators;
using BlueprintCore.Blueprints.Configurators.Items;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Blueprints;
using System;
using BlueprintCore.Actions.Builder.AVEx;

namespace AviaryClasses.Classes.Features {
    public abstract class CantripSplashExtension {
        protected abstract DamageEnergyType EnergyType { get; }
        protected abstract string AbilityRef { get; }

        public void Configure(ContextDiceValue splashDice, ConditionsBuilder splashCondition) {
            ActionList internalSplashAction = ActionsBuilder.New()
                .Conditional(
                    conditions: ConditionsBuilder.New().IsMainTarget(),
                    ifFalse: ActionsBuilder.New().DealDamage(
                        new DamageTypeDescription() {
                            Type = DamageType.Energy,
                            Energy = EnergyType,
                        },
                        splashDice
                    )
                )
                .SpawnFx(ElementalEffects.GetEffectForEnergyType(EnergyType))
                .Build();

            ActionList splashAction = ActionsBuilder.New()
                .OnRandomTargetsAround(
                    actions: internalSplashAction,
                    onEnemies: true,
                    numberOfTargets: RandomUtils.GetRandomValue<int>([2, 3]),
                    radius: 20.Feet()
                )
                .Build();

            ActionList newActions = ActionsBuilder.New()
                .Conditional(conditions: splashCondition, ifTrue: splashAction)
                .Build();

            AbilityConfigurator.For(AbilityRef)
                .EditComponent<AbilityEffectRunAction>(
                    c => c.Actions = new ActionList(newActions, c.Actions)
                )
                .Configure();
        }
    }

    internal class IgnitionSplashExt : CantripSplashExtension {
        protected override DamageEnergyType EnergyType => DamageEnergyType.Fire;
        protected override string AbilityRef => AbilityRefs.Ignition.ToString();
    }

    internal class RayOfFrostSplashExt : CantripSplashExtension {
        protected override DamageEnergyType EnergyType => DamageEnergyType.Cold;
        protected override string AbilityRef => AbilityRefs.RayOfFrost.ToString();
    }

    internal class AcidSplashSplashExt : CantripSplashExtension {
        protected override DamageEnergyType EnergyType => DamageEnergyType.Acid;
        protected override string AbilityRef => AbilityRefs.AcidSplash.ToString();
    }

    internal class JoltSplashExt : CantripSplashExtension {
        protected override DamageEnergyType EnergyType => DamageEnergyType.Electricity;
        protected override string AbilityRef => AbilityRefs.Jolt.ToString();
    }

    public static class CantripSplashFactory {
        public static void ConfigureAllSplashExtensions(ContextDiceValue splashDice, ConditionsBuilder splashCondition) {
            new IgnitionSplashExt().Configure(splashDice, splashCondition);
            new RayOfFrostSplashExt().Configure(splashDice, splashCondition);
            new AcidSplashSplashExt().Configure(splashDice, splashCondition);
            new JoltSplashExt().Configure(splashDice, splashCondition);
        }
    }
}