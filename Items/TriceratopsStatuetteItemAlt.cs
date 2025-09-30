
using System;
using System.Collections.Generic;
using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.BasicEx;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.Configurators.Items.Equipment;
using BlueprintCore.Blueprints.Configurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.EntitySystem.Stats;
using BlueprintCore.Utils.Types;
using BlueprintCore.Blueprints.CustomConfigurators;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Properties;
using BlueprintCore.Blueprints.Configurators.Classes;
using BlueprintCore.Blueprints.Configurators.Classes.Selection;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic.Class.Kineticist;
using Kingmaker.Designers.Mechanics.Facts;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Alignments;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Utility;
using System;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
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
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.RuleSystem;
using Kingmaker.Enums.Damage;
using Kingmaker.Designers.EventConditionActionSystem.Evaluators;
using Kingmaker.Visual.HitSystem;



namespace AviaryClasses.Items {

    /// <summary>
    /// Mods the TriceratopsStatuetteItem to have a permanent ability grant
    /// </summary>
    public class TriceratopsStatuetteItemAlt {


        private static readonly LogWrapper logger = LogWrapper.Get("TriceratopsStatuetteItemAlt");
        public static readonly string itemGuid = "d8d32918de5e11246a048269e7e0bbb9";
        public static readonly string abilityName = "GrantTriceratopsAbility";
        public static readonly string abilityGuid = "daa3ca2b-7938-43af-8f72-cc022a5e79d4";
        private const string AbilityNameKey = "AVC.TriceratopsStatuette.Ability.Name";
        private const string AbilityDescriptionKey = "AVC.TriceratopsStatuette.Ability.Description";
        private static readonly Blueprint<BlueprintUnitFactReference>[] CompanionFacts = {
            FeatureRefs.AnimalCompanionFeatureArmoredPony_ForNPC.Cast<BlueprintUnitFactReference>(),
            FeatureRefs.AnimalCompanionFeatureBear.Cast<BlueprintUnitFactReference>(),
            FeatureRefs.AnimalCompanionFeatureBoar.Cast<BlueprintUnitFactReference>(),
            FeatureRefs.AnimalCompanionFeatureCentipede.Cast<BlueprintUnitFactReference>(),
            FeatureRefs.AnimalCompanionFeatureDog.Cast<BlueprintUnitFactReference>(),
            FeatureRefs.AnimalCompanionFeatureElk.Cast<BlueprintUnitFactReference>(),
            FeatureRefs.AnimalCompanionFeatureHorse.Cast<BlueprintUnitFactReference>(),
            FeatureRefs.AnimalCompanionFeatureHorse_PreorderBonus.Cast<BlueprintUnitFactReference>(),
            FeatureRefs.AnimalCompanionFeatureLeopard.Cast<BlueprintUnitFactReference>(),
            FeatureRefs.AnimalCompanionFeatureMammoth.Cast<BlueprintUnitFactReference>(),
            FeatureRefs.AnimalCompanionFeatureMonitor.Cast<BlueprintUnitFactReference>(),
            FeatureRefs.AnimalCompanionFeatureSmilodon.Cast<BlueprintUnitFactReference>(),
            FeatureRefs.AnimalCompanionFeatureSmilodon_PreorderBonus.Cast<BlueprintUnitFactReference>(),
            FeatureRefs.AnimalCompanionFeatureTriceratops.Cast<BlueprintUnitFactReference>(),
            FeatureRefs.AnimalCompanionFeatureTriceratops_PreorderBonus.Cast<BlueprintUnitFactReference>(),
            FeatureRefs.AnimalCompanionFeatureVelociraptor.Cast<BlueprintUnitFactReference>(),
            FeatureRefs.AnimalCompanionFeatureWolf.Cast<BlueprintUnitFactReference>(),
            FeatureRefs.TriceratopsStatuetteCorrectFeature.Cast<BlueprintUnitFactReference>()
        };


        public static void Configure() {

            try {

                var abilityNameString = LocalizationTool.GetString(AbilityNameKey);
                var abilityDescriptionString = LocalizationTool.GetString(AbilityDescriptionKey);

                // Grant triceratops FIRST (ensures it's available for selection)
                var grantTriceratopsFeature = FeatureRefs.AnimalCompanionFeatureTriceratops.Cast<BlueprintFeatureReference>();

                // Then grant ranger selection system (provides progression framework, may trigger UI)
                var rangerCompanionSelection = FeatureSelectionRefs.AnimalCompanionSelectionRanger.Cast<BlueprintFeatureReference>();

                ActionsBuilder grantTriceratopsActions = ActionsBuilder.New()
                    .AddFeature(rangerCompanionSelection); // Framework for progression FIRST

                // Grant AnimalCompanionRank for levels 5-20 BEFORE granting companion
                for (int level = 5; level <= 20; level++) {
                    var animalCompanionRankFeature = FeatureRefs.AnimalCompanionRank.Cast<BlueprintFeatureReference>();
                    grantTriceratopsActions.AddFeature(animalCompanionRankFeature);
                }

                // Grant specific companion LAST (after ranks are in place)
                grantTriceratopsActions.AddFeature(grantTriceratopsFeature);

                // Force a level recalculation to update companion level immediately
                grantTriceratopsActions.ForceLevelup();

                AbilityConfigurator.New(abilityName, abilityGuid)
                    .SetDisplayName(abilityNameString)
                    .SetDescription(abilityDescriptionString)
                    .SetType(AbilityType.Supernatural)
                    .SetRange(AbilityRange.Personal)
                    .SetActionType(UnitCommand.CommandType.Standard)
                    .AddAbilityCasterHasNoFacts(new List<Blueprint<BlueprintUnitFactReference>>(CompanionFacts))
                    .AddAbilityEffectRunAction(
                        grantTriceratopsActions
                    )
                    .Configure();

                ItemEquipmentUsableConfigurator.For(itemGuid)
                    .SetAbility(abilityGuid)
                    .SetDescriptionText(LocalizationTool.GetString("TriceratopsStatuetteItem.Description"))
                    .SetSpendCharges(true)
                    .SetRestoreChargesOnRest(false)
                    .SetCharges(1)
                    .Configure();

                logger.Info("Triceratops Statuette item configured successfully.");

            } catch (Exception ex) {
                logger.Error(ex.ToString());
            }

        }
    }
}
