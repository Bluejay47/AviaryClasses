using BlueprintCore.Actions.Builder;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using System;
using System.Linq;

namespace AviaryClasses.Classes.Features {
    public class FriendToAnimals {
        private static readonly LogWrapper Logger = LogWrapper.Get("FriendToAnimals");

        private static readonly string featureName = "BeastTamerFriendToAnimals";
        public static readonly string featureGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac3010";
        private static readonly string buffName = "BeastTamerFriendToAnimalsBuff";
        private static readonly string buffGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac3011";
        private static readonly string areaName = "BeastTamerFriendToAnimalsArea";
        private static readonly string areaGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac3012";
        private static readonly string effectBuffName = "BeastTamerFriendToAnimalsEffectBuff";
        private static readonly string effectBuffGuid = "b7a8c395-d948-5124-bb2c-0e4aa6ac3013";


        public static void Configure() {
            try {
                // Create effect buff by copying from Oracle's version
                BuffConfigurator.New(effectBuffName, effectBuffGuid)
                    .CopyFrom(BuffRefs.OracleRevelationFriendToAnimalsEffectBuff, c => true)
                    .Configure();

                // Create area effect by copying Oracle's version and modifying conditions
                var areaEffect = AbilityAreaEffectConfigurator.New(areaName, areaGuid)
                    .CopyFrom(AbilityAreaEffectRefs.OracleRevelationFriendToAnimalsArea, c => true)
                    .OnConfigure(bp => {
                        // Find AbilityAreaEffectRunAction component and modify conditions
                        var runActionComp = bp.ComponentsArray.OfType<Kingmaker.UnitLogic.Abilities.Components.AreaEffects.AbilityAreaEffectRunAction>().FirstOrDefault();
                        if (runActionComp != null) {
                            // The UnitEnter action has a Conditional that checks for AnimalType
                            // We need to replace that condition to check for animal companions instead
                            var conditionalAction = runActionComp.UnitEnter.Actions.OfType<Kingmaker.Designers.EventConditionActionSystem.Actions.Conditional>().FirstOrDefault();
                            if (conditionalAction != null) {
                                conditionalAction.ConditionsChecker = ConditionsBuilder.New()
                                    .Add<Kingmaker.UnitLogic.Mechanics.Conditions.ContextConditionIsAlly>()
                                    .AddOrAndLogic(
                                        ConditionsBuilder.New()
                                            .Add<Kingmaker.UnitLogic.Mechanics.Conditions.ContextConditionIsAnimalCompanion>()
                                    )
                                    .Build();
                            }

                            // Update the buff reference to our new effect buff
                            var applyBuffAction = conditionalAction?.IfTrue.Actions.OfType<Kingmaker.UnitLogic.Mechanics.Actions.ContextActionApplyBuff>().FirstOrDefault();
                            if (applyBuffAction != null) {
                                applyBuffAction.m_Buff = BlueprintTool.GetRef<BlueprintBuffReference>(effectBuffGuid);
                            }

                            // Update the RemoveBuff action to use our new effect buff
                            var removeBuffAction = runActionComp.UnitExit.Actions.OfType<Kingmaker.UnitLogic.Mechanics.Actions.ContextActionRemoveBuff>().FirstOrDefault();
                            if (removeBuffAction != null) {
                                removeBuffAction.m_Buff = BlueprintTool.GetRef<BlueprintBuffReference>(effectBuffGuid);
                            }
                        }
                    })
                    .Configure();

                // Create buff that adds the area effect
                var buff = BuffConfigurator.New(buffName, buffGuid)
                    .CopyFrom(BuffRefs.OracleRevelationFriendToAnimalsBuff, c => true)
                    .OnConfigure(bp => {
                        // Find AddAreaEffect component and update it to use our new area effect
                        var areaComp = bp.ComponentsArray.OfType<Kingmaker.UnitLogic.Buffs.Components.AddAreaEffect>().FirstOrDefault();
                        if (areaComp != null) {
                            areaComp.m_AreaEffect = areaEffect.ToReference<BlueprintAbilityAreaEffectReference>();
                        }
                    })
                    .Configure();

                // Get the icon from the Oracle feature
                var oracleFeature = BlueprintTool.Get<Kingmaker.Blueprints.Classes.BlueprintFeature>(FeatureRefs.OracleRevelationFriendToAnimals.ToString());

                // Create feature that grants the buff
                FeatureConfigurator.New(featureName, featureGuid)
                    .SetDisplayName(featureName + ".Name")
                    .SetDescription(featureName + ".Description")
                    .SetIcon(oracleFeature.m_Icon)
                    .SetIsClassFeature(true)
                    .AddFacts(new() { buffGuid })
                    .Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }
        }
    }
}
