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
using BlueprintCore.Blueprints.Configurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using Kingmaker.Blueprints;
using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Class.Kineticist;
using Kingmaker.Designers.Mechanics.Facts;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Alignments;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Utility;
using System;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
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
using BlueprintCore.Actions.Builder.BasicEx;
using Kingmaker.Enums.Damage;
using Kingmaker.Designers.EventConditionActionSystem.Evaluators;
using Kingmaker.Visual.HitSystem;



namespace AviaryClasses.Classes {

    public class LifeSensate {

        private static readonly LogWrapper Logger = LogWrapper.Get("LifeSensate");

        public static BlueprintArchetype archetypeRef;
        public static readonly string featName = "LifeSensate";
        public static readonly string featGuid = "9690f4ec-a4ad-4a9c-aaee-56251fbbea3c";


        public static void Configure() {

            try {

                SelectiveHealing.Configure();
                LifeBubble.Configure();
                LingeringEnergiesResource.Configure();
                LingeringEnergies.Configure();
                LingeringEnergiesBuff.Configure();
                CorrectedSupercharge.Configure();
                HealingWildTalentSelection.Configure();
                EmpoweredHealing.Configure();
                EmpoweredHealing2.Configure();
                LifeAttunement.Configure();
                LifeStudies.Configure();
                LifeFont.Configure();
                KineticRestoration2.Configure();
                KineticRestoration3.Configure();

                ArchetypeConfigurator archetype = ArchetypeConfigurator.New(LifeSensate.featName, LifeSensate.featGuid, CharacterClassRefs.KineticistClass);

                archetype.EditComponents<AddKineticistPart>(
                    c => c.MainStat = StatType.Intelligence,
                    x => true
                );

                archetype.SetBaseAttackBonus(StatProgressionRefs.BABMedium.ToString()); 
                archetype.SetFortitudeSave(StatProgressionRefs.SavesLow.ToString()); 
                archetype.SetReflexSave(StatProgressionRefs.SavesLow.ToString()); 
                archetype.SetWillSave(StatProgressionRefs.SavesHigh.ToString()); 

                archetype.SetLocalizedName(featName + ".Name");
                archetype.SetLocalizedDescription(featName + ".Description");
                archetype.AddPrerequisiteAlignment(AlignmentMaskType.TrueNeutral | AlignmentMaskType.Good, mergeBehavior: ComponentMerge.Merge);

                //level 1
                archetype.AddToAddFeatures(1, LifeAttunement.featGuid);
                archetype.AddToAddFeatures(1, LifeStudies.featGuid); 
                archetype.AddToAddFeatures(1, FeatureRefs.KineticHealerFeature.ToString());
                archetype.AddToAddFeatures(1, LingeringEnergies.featGuid);
                archetype.AddToRemoveFeatures(1, FeatureRefs.BurnFeature.ToString());
                archetype.AddToRemoveFeatures(1, ProgressionRefs.ElementalOverflowProgression.ToString());
                archetype.AddToRemoveFeatures(1, FeatureSelectionRefs.InfusionSelection.ToString());

                // level 2
                archetype.AddToAddFeatures(2, FeatureSelectionRefs.InfusionSelection.ToString());
                archetype.AddToRemoveFeatures(2, FeatureSelectionRefs.WildTalentSelection.ToString());
                archetype.AddToAddFeatures(2, HealingWildTalentSelection.featGuid);

                // level 3
                archetype.AddToAddFeatures(3, LifeFont.featGuid);
                archetype.AddToAddFeatures(3, FeatureRefs.HealingBUrstFeature.ToString());
                archetype.AddToRemoveFeatures(3, FeatureSelectionRefs.InfusionSelection.ToString());
                archetype.AddToRemoveFeatures(3, FeatureRefs.ElementalOverflowBonusFeature.ToString());

                //level 4
                archetype.AddToAddFeatures(4, FeatureSelectionRefs.InfusionSelection.ToString());
                archetype.AddToRemoveFeatures(4, FeatureSelectionRefs.WildTalentSelection.ToString());
                archetype.AddToAddFeatures(4, FeatureSelectionRefs.AnimalCompanionSelectionDruid.ToString());

                //level 5
                archetype.AddToAddFeatures(5, FeatureRefs.OverwhelmingSoulMentalProwessAdditionalUseFeature.ToString());
                archetype.AddToRemoveFeatures(5, FeatureSelectionRefs.InfusionSelection.ToString());
                archetype.AddToAddFeatures(5, HealingWildTalentSelection.featGuid);
                archetype.AddToAddFeatures(5, FeatureRefs.AnimalCompanionRank.ToString());

                //level 6
                archetype.AddToAddFeatures(6, LifeFont.featGuid);
                archetype.AddToAddFeatures(6, EmpoweredHealing.featGuid);
                archetype.AddToRemoveFeatures(6, FeatureSelectionRefs.WildTalentSelection.ToString());
                archetype.AddToAddFeatures(6, FeatureRefs.AnimalCompanionRank.ToString());

                //level 7
                archetype.AddToRemoveFeatures(7, FeatureSelectionRefs.SecondatyElementalFocusSelection.ToString());
                archetype.AddToAddFeatures(7, FeatureRefs.AnimalCompanionRank.ToString());

                //level 8
                archetype.AddToAddFeatures(8, FeatureSelectionRefs.InfusionSelection.ToString());
                archetype.AddToRemoveFeatures(8, FeatureSelectionRefs.WildTalentSelection.ToString());
                archetype.AddToAddFeatures(8, FeatureRefs.AnimalCompanionRank.ToString());

                //level 9
                archetype.AddToAddFeatures(9, LifeFont.featGuid);
                archetype.AddToRemoveFeatures(9, FeatureSelectionRefs.InfusionSelection.ToString());
                archetype.AddToAddFeatures(9, HealingWildTalentSelection.featGuid);
                archetype.AddToAddFeatures(9, FeatureRefs.AnimalCompanionRank.ToString());

                //level 10
                archetype.AddToAddFeatures(10, FeatureSelectionRefs.InfusionSelection.ToString());
                archetype.AddToAddFeatures(10, FeatureRefs.OverwhelmingSoulMentalProwessAdditionalUseFeature.ToString());
                archetype.AddToRemoveFeatures(10, FeatureSelectionRefs.WildTalentSelection.ToString());
                archetype.AddToAddFeatures(10, FeatureRefs.KineticRevificationFeature.ToString());
                archetype.AddToAddFeatures(10, FeatureRefs.AnimalCompanionRank.ToString());

                //level 11
                archetype.AddToAddFeatures(11, CorrectedSupercharge.featGuid);
                archetype.AddToRemoveFeatures(11, FeatureRefs.Supercharge.ToString());
                archetype.AddToRemoveFeatures(11, FeatureSelectionRefs.InfusionSelection.ToString());
                archetype.AddToAddFeatures(11, FeatureRefs.AnimalCompanionRank.ToString());
                archetype.AddToAddFeatures(11, EmpoweredHealing2.featGuid);

                //level 12
                archetype.AddToAddFeatures(12, FeatureSelectionRefs.InfusionSelection.ToString());
                archetype.AddToAddFeatures(12, LifeFont.featGuid);
                archetype.AddToRemoveFeatures(12, FeatureSelectionRefs.WildTalentSelection.ToString());
                archetype.AddToAddFeatures(12, FeatureRefs.AnimalCompanionRank.ToString());

                //level 13
                archetype.AddToRemoveFeatures(13, FeatureSelectionRefs.InfusionSelection.ToString());
                archetype.AddToAddFeatures(13, HealingWildTalentSelection.featGuid);
                archetype.AddToAddFeatures(13, FeatureRefs.AnimalCompanionRank.ToString());

                //level 14
                archetype.AddToAddFeatures(14, FeatureSelectionRefs.InfusionSelection.ToString());
                archetype.AddToAddFeatures(14, FeatureRefs.OverwhelmingSoulMentalProwessAdditionalUseFeature.ToString());
                archetype.AddToRemoveFeatures(14, FeatureSelectionRefs.WildTalentSelection.ToString());
                archetype.AddToAddFeatures(14, FeatureRefs.AnimalCompanionRank.ToString());

                //level 15
                archetype.AddToAddFeatures(15, LifeFont.featGuid);
                archetype.AddToAddFeatures(15, FeatureSelectionRefs.SecondatyElementalFocusSelection.ToString());
                archetype.AddToAddFeatures(15, FeatureSelectionRefs.WaterBlastSelection.ToString());
                archetype.AddToAddFeatures(15, FeatureRefs.AnimalCompanionRank.ToString());
                archetype.AddToRemoveFeatures(15, FeatureSelectionRefs.ThirdElementalFocusSelection.ToString());

                //level 16
                archetype.AddToAddFeatures(16, FeatureSelectionRefs.InfusionSelection.ToString());
                archetype.AddToRemoveFeatures(16, FeatureSelectionRefs.WildTalentSelection.ToString());
                archetype.AddToAddFeatures(16, FeatureRefs.AnimalCompanionRank.ToString());

                //level 17
                archetype.AddToRemoveFeatures(17, FeatureSelectionRefs.InfusionSelection.ToString());
                archetype.AddToAddFeatures(17, HealingWildTalentSelection.featGuid);
                archetype.AddToAddFeatures(17, FeatureRefs.AnimalCompanionRank.ToString());

                //level 18
                archetype.AddToAddFeatures(18, FeatureSelectionRefs.InfusionSelection.ToString());
                archetype.AddToAddFeatures(18, LifeFont.featGuid);
                archetype.AddToAddFeatures(18, FeatureRefs.OverwhelmingSoulMentalProwessAdditionalUseFeature.ToString());
                archetype.AddToRemoveFeatures(18, FeatureSelectionRefs.WildTalentSelection.ToString());
                archetype.AddToAddFeatures(18, FeatureRefs.AnimalCompanionRank.ToString());

                //level 19
                archetype.AddToRemoveFeatures(19, FeatureSelectionRefs.InfusionSelection.ToString());
                archetype.AddToAddFeatures(19, FeatureRefs.AnimalCompanionRank.ToString());

                //level 20
                archetype.AddToRemoveFeatures(20, FeatureSelectionRefs.WildTalentSelection.ToString());
                archetype.AddToAddFeatures(20, HealingWildTalentSelection.featGuid);
                archetype.AddToAddFeatures(20, FeatureRefs.AnimalCompanionRank.ToString());

                //Class Skills
                archetype.SetReplaceClassSkills(true);
                archetype.SetClassSkills(
                    StatType.SkillKnowledgeWorld,
                    StatType.SkillKnowledgeArcana,
                    StatType.SkillPerception,
                    StatType.SkillUseMagicDevice
                );

                archetype.SetOverrideAttributeRecommendations(true);
                archetype.SetRecommendedAttributes(
                    StatType.Intelligence,
                    StatType.Dexterity
                );

                LifeSensate.archetypeRef = archetype.Configure();

                FeatureConfigurator.For(ProgressionRefs.ElementalFocusAir_0.ToString()).AddPrerequisiteNoArchetype(LifeSensate.featGuid, CharacterClassRefs.KineticistClass.ToString()).Configure();
                FeatureConfigurator.For(ProgressionRefs.ElementalFocusEarth_0.ToString()).AddPrerequisiteNoArchetype(LifeSensate.featGuid, CharacterClassRefs.KineticistClass.ToString()).Configure();
                FeatureConfigurator.For(ProgressionRefs.ElementalFocusFire_0.ToString()).AddPrerequisiteNoArchetype(LifeSensate.featGuid, CharacterClassRefs.KineticistClass.ToString()).Configure();

                FeatureConfigurator.For(ProgressionRefs.SecondaryElementAir.ToString()).AddPrerequisiteNoArchetype(LifeSensate.featGuid, CharacterClassRefs.KineticistClass.ToString()).Configure();
                FeatureConfigurator.For(ProgressionRefs.SecondaryElementEarth.ToString()).AddPrerequisiteNoArchetype(LifeSensate.featGuid, CharacterClassRefs.KineticistClass.ToString()).Configure();
                FeatureConfigurator.For(ProgressionRefs.SecondaryElementFire.ToString()).AddPrerequisiteNoArchetype(LifeSensate.featGuid, CharacterClassRefs.KineticistClass.ToString()).Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }

        }
    }


    public class HealingWildTalentSelection {

        private static readonly LogWrapper Logger = LogWrapper.Get("HealingWildTalentSelection");

        public static readonly string featName = "HealingWildTalentSelection";
        public static readonly string featGuid = "2e71dd65-8606-4401-bc46-1e9708e71a18";


        public static void Configure() {

            try {

                FeatureSelectionConfigurator featureSelection = FeatureSelectionConfigurator.New(featName, featGuid)
                    .CopyFrom(FeatureSelectionRefs.WildTalentSelection);

                featureSelection.SetDisplayName("HealingWildTalentSelection.Name");

                featureSelection.ClearAllFeatures();
                featureSelection.AddToAllFeatures(FeatureRefs.KineticRestorationFeature.ToString());
                featureSelection.AddToAllFeatures(KineticRestoration2.featGuid);
                featureSelection.AddToAllFeatures(KineticRestoration3.featGuid);
                featureSelection.AddToAllFeatures(FeatureRefs.HealingBUrstFeature.ToString());
                featureSelection.AddToAllFeatures(FeatureRefs.KineticRevificationFeature.ToString());
                featureSelection.AddToAllFeatures(FeatureSelectionRefs.SkillFocusSelection.ToString());
                featureSelection.AddToAllFeatures(FeatureRefs.SkilledKineticistFeature.ToString());
                featureSelection.AddToAllFeatures(FeatureSelectionRefs.ElementalWhispersSelection.ToString());
                featureSelection.AddToAllFeatures(FeatureRefs.SlickFeature.ToString());
                featureSelection.AddToAllFeatures(LifeBubble.featGuid);
                //featureSelection.AddToAllFeatures(SelectiveHealing.featGuid);
                featureSelection.AddToAllFeatures(FeatureSelectionRefs.WildTalentBonusFeatWater.ToString());
                featureSelection.AddToAllFeatures(FeatureSelectionRefs.WildTalentBonusFeatWater1.ToString());
                featureSelection.AddToAllFeatures(FeatureSelectionRefs.WildTalentBonusFeatWater2.ToString());
                featureSelection.AddToAllFeatures(FeatureSelectionRefs.WildTalentBonusFeatWater3.ToString());
                featureSelection.AddToAllFeatures(FeatureSelectionRefs.WildTalentBonusFeatWater4.ToString());
                featureSelection.AddToAllFeatures(FeatureSelectionRefs.WildTalentBonusFeatWater5.ToString());

                featureSelection.Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }

        }

    }

    public class LifeStudies {

        private static readonly LogWrapper Logger = LogWrapper.Get("LifeStudies");

        public static readonly string featName = "LifeStudies";
        public static readonly string featGuid = "f05245c8-7233-4bad-ad10-9bd691f4e750";


        public static void Configure() {

            try {

                FeatureConfigurator feature = FeatureConfigurator.New(featName, featGuid);

                if (feature != null) {

                    feature.SetDescription(featName + ".Description");
                    feature.SetDisplayName(featName + ".Name");
                    feature.SetIsClassFeature(true);

                    feature.Configure();

                }

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }

        }

    }

    public class KineticRestoration2 {

        private static readonly LogWrapper Logger = LogWrapper.Get("KineticRestoration2");

        public static readonly string featName = "KineticRestoration2";
        public static readonly string featGuid = "6b767630-bf1d-4a8c-af3b-7100da75669f";
        public static readonly string abilityName = "KineticRestoration2Ability";
        public static readonly string abilityGuid = "de1dac68-6ffa-41ff-9c37-cfc3dfd7e95e";

        public static void Configure() {

            try {

                BlueprintAbility baseAbility = BlueprintTool.Get<BlueprintAbility>(AbilityRefs.RemoveSickness.ToString());

                ActionsBuilder healActions = ActionsBuilder.New()
                .RemoveBuffsByDescriptor(SpellDescriptor.Blindness)
                .RemoveBuff(BuffRefs.Blind.ToString())
                .RemoveBuff(BuffRefs.BlindnessBuff.ToString())
                .RemoveBuff(BuffRefs.DazeBuff.ToString())
                .RemoveBuff(BuffRefs.Stunned.ToString())
                .RemoveBuff(BuffRefs.GlitterdustBlindnessBuff.ToString())
                .RemoveBuff(BuffRefs.DazzledBuff.ToString())
                .RemoveBuff(BuffRefs.InsanityBuff.ToString())
                .RemoveBuff(BuffRefs.FeeblemindBuff.ToString())
                .RemoveBuff(BuffRefs.Ecorche_Buff_SeizeSkin.ToString())
                .RemoveBuff(BuffRefs.Confusion.ToString())
                .RemoveBuff(BuffRefs.Fatigued.ToString())
                .RemoveBuff(BuffRefs.Exhausted.ToString())
                .RemoveBuff(BuffRefs.Nauseated.ToString())
                .RemoveBuff(BuffRefs.PoisonBuff.ToString())
                .RemoveBuff(BuffRefs.Sickened.ToString())
                .HealStatDamage(
                    statClass: Kingmaker.UnitLogic.Mechanics.Actions.ContextActionHealStatDamage.StatClass.Any, 
                    healType: Kingmaker.UnitLogic.Mechanics.Actions.ContextActionHealStatDamage.StatDamageHealType.HealAllDamage
                )
                .Build();

                BlueprintAbility ability = AbilityConfigurator.New(abilityName, abilityGuid)
                .SetDisplayName(featName + ".Name")
                .SetDescription(featName + ".Description")
                .SetIcon(baseAbility.m_Icon)
                .SetCanTargetFriends(true)
                .SetCanTargetSelf(true)
                .AddAbilityEffectRunAction(healActions)
                .SetAnimation(Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell.CastAnimationStyle.Kineticist)
                .SetAvailableMetamagic(Metamagic.Quicken)
                .SetType(AbilityType.Supernatural)
                .AddComponent<AbilityKineticist>(c => c.WildTalentBurnCost=1)
                .Configure();

                FeatureConfigurator feature = FeatureConfigurator.New(featName, featGuid, FeatureGroup.KineticWildTalent);

                feature.SetDescription(featName + ".Description")
                .SetDisplayName(featName + ".Name")
                .SetIsClassFeature(true)
                .SetIcon(baseAbility.m_Icon)
                .AddKineticistAcceptBurnTrigger()
                .AddPrerequisiteArchetypeLevel(LifeSensate.featGuid, CharacterClassRefs.KineticistClass.ToString(), level: 13)
                .AddPrerequisiteFeature(FeatureRefs.KineticRestorationFeature.ToString())
                .AddFacts(new() { ability })
                .SetHideInCharacterSheetAndLevelUp(true)
                .Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }

        }

    }

    public class KineticRestoration3 {

        private static readonly LogWrapper Logger = LogWrapper.Get("KineticRestoration3");

        public static readonly string featName = "KineticRestoration3";
        public static readonly string featGuid = "13f31782-4bc5-4530-9994-094c404068a2";
        public static readonly string abilityName = "KineticRestoration3Ability";
        public static readonly string abilityGuid = "ed8d214d-274f-4e62-8115-659dff3d1826";

        public static void Configure() {

            try {

                BlueprintAbility baseAbility = BlueprintTool.Get<BlueprintAbility>(AbilityRefs.RemoveCurse.ToString());

                ActionsBuilder healActions = ActionsBuilder.New()
                .RemoveBuffsByDescriptor(SpellDescriptor.Blindness)
                .RemoveBuff(BuffRefs.Blind.ToString())
                .RemoveBuff(BuffRefs.BlindnessBuff.ToString())
                .RemoveBuff(BuffRefs.DazeBuff.ToString())
                .RemoveBuff(BuffRefs.Stunned.ToString())
                .RemoveBuff(BuffRefs.GlitterdustBlindnessBuff.ToString())
                .RemoveBuff(BuffRefs.DazzledBuff.ToString())
                .RemoveBuff(BuffRefs.InsanityBuff.ToString())
                .RemoveBuff(BuffRefs.FeeblemindBuff.ToString())
                .RemoveBuff(BuffRefs.Ecorche_Buff_SeizeSkin.ToString())
                .RemoveBuff(BuffRefs.Confusion.ToString())
                .RemoveBuff(BuffRefs.Fatigued.ToString())
                .RemoveBuff(BuffRefs.Exhausted.ToString())
                .RemoveBuff(BuffRefs.Nauseated.ToString())
                .RemoveBuff(BuffRefs.PoisonBuff.ToString())
                .RemoveBuff(BuffRefs.Sickened.ToString())
                .HealStatDamage(
                    statClass: Kingmaker.UnitLogic.Mechanics.Actions.ContextActionHealStatDamage.StatClass.Any, 
                    healType: Kingmaker.UnitLogic.Mechanics.Actions.ContextActionHealStatDamage.StatDamageHealType.HealAllDamage
                )
                .Build();

                BlueprintAbility ability = AbilityConfigurator.New(abilityName, abilityGuid)
                .SetDisplayName(featName + ".Name")
                .SetDescription(featName + ".Description")
                .SetIcon(baseAbility.m_Icon)
                .SetCanTargetPoint(true)
                .AddAbilityEffectRunAction(healActions)
                .SetAnimation(Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell.CastAnimationStyle.Kineticist)
                .SetAvailableMetamagic(Metamagic.Quicken)
                .SetType(AbilityType.Supernatural)
                .AddAbilityTargetsAround (
                    radius: 30.Feet(),
                    targetType: TargetType.Ally
                )
                .AddComponent<AbilityKineticist>(c => c.WildTalentBurnCost=2)
                .Configure();

                FeatureConfigurator feature = FeatureConfigurator.New(featName, featGuid, FeatureGroup.KineticWildTalent);

                feature.SetDescription(featName + ".Description")
                .SetDisplayName(featName + ".Name")
                .SetIsClassFeature(true)
                .SetIcon(baseAbility.m_Icon)
                .AddKineticistAcceptBurnTrigger()
                .AddPrerequisiteArchetypeLevel(LifeSensate.featGuid, CharacterClassRefs.KineticistClass.ToString(), level: 17)
                .AddPrerequisiteFeature(FeatureRefs.KineticRestorationFeature.ToString())
                .AddFacts(new() { ability })
                .SetHideInCharacterSheetAndLevelUp(true)
                .Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }

        }

    }


    public class LifeBubble {

        private static readonly LogWrapper Logger = LogWrapper.Get("LifeBubble");

        public static readonly string featName = "LifeBubble";
        public static readonly string featGuid = "a09e1d2b-88f4-4378-8f34-a030cf9483c7";
        public static readonly string abilityName = "LifeBubbleAbility";
        public static readonly string abilityGuid = "6094a64d-8773-4e39-bddd-8da375bd3fd7";


        public static void Configure() {

            try {

                BlueprintAbility baseAbility = BlueprintTool.Get<BlueprintAbility>(AbilityRefs.ResistEnergyCommunal.ToString());

                BlueprintAbility fireAbility = AbilityConfigurator.New("LifeBubbleResistFire", "6552f952-523b-42ea-a94a-0209ea08a821").CopyFrom(AbilityRefs.ResistFireCommunal.ToString(), x => true)
                .SetDisplayName(featName + "Fire.Name")
                .SetDescription(featName + ".Description")
                //.AddAbilityAcceptBurnOnCast(1)
                .AddComponent<AbilityKineticist>(c => c.WildTalentBurnCost=1)
                .SetType(AbilityType.SpellLike)
                .Configure();

                BlueprintAbility acidAbility = AbilityConfigurator.New("LifeBubbleResistAcid", "ec3408c9-fda9-40d6-aa6b-72e10d053616").CopyFrom(AbilityRefs.ResistAcidCommunal.ToString(), x => true)
                .SetDisplayName(featName + "Acid.Name")
                .SetDescription(featName + ".Description")
                //.AddAbilityAcceptBurnOnCast(1)
                .AddComponent<AbilityKineticist>(c => c.WildTalentBurnCost=1)
                .SetType(AbilityType.SpellLike)
                .Configure();

                BlueprintAbility coldAbility = AbilityConfigurator.New("LifeBubbleResistCold", "025f3549-00cf-41a5-b14f-782a3bd54ca9").CopyFrom(AbilityRefs.ResistColdCommunal.ToString(), x => true)
                .SetDisplayName(featName + "Cold.Name")
                .SetDescription(featName + ".Description")
                //.AddAbilityAcceptBurnOnCast(1)
                .AddComponent<AbilityKineticist>(c => c.WildTalentBurnCost=1)
                .SetType(AbilityType.SpellLike)
                .Configure();

                BlueprintAbility elecAbility = AbilityConfigurator.New("LifeBubbleResistElec", "2b66f785-a2d9-4c1f-b4d3-3744f4fdd981").CopyFrom(AbilityRefs.ResistElectricityCommunal.ToString(), x => true)
                .SetDisplayName(featName + "Elec.Name")
                .SetDescription(featName + ".Description")
                //.AddAbilityAcceptBurnOnCast(1)
                .AddComponent<AbilityKineticist>(c => c.WildTalentBurnCost=1)
                .SetType(AbilityType.SpellLike)
                .Configure();

                BlueprintAbility sonicAbility = AbilityConfigurator.New("LifeBubbleResistSonic", "221c7859-e10e-4603-9413-fd1da916ef4a").CopyFrom(AbilityRefs.ResistSonicCommunal.ToString(), x => true)
                .SetDisplayName(featName + "Sonic.Name")
                .SetDescription(featName + ".Description")
                .SetType(AbilityType.SpellLike)
                //.AddAbilityAcceptBurnOnCast(1)
                .AddComponent<AbilityKineticist>(c => c.WildTalentBurnCost=1)
                .Configure();
     
                BlueprintAbility ability = AbilityConfigurator.New(abilityName, abilityGuid)
                .SetDisplayName(featName + ".Name")
                .SetDescription(featName + ".Description")
                .AddAbilityVariants([fireAbility, acidAbility, coldAbility, elecAbility, sonicAbility])
                .SetIcon(baseAbility.m_Icon)
                .Configure();

                FeatureConfigurator feature = FeatureConfigurator.New(featName, featGuid, FeatureGroup.KineticWildTalent);

                feature.SetDescription(featName + ".Description")
                .SetDisplayName(featName + ".Name")
                .SetIsClassFeature(true)
                .SetIcon(baseAbility.m_Icon)
                .AddKineticistAcceptBurnTrigger()
                .AddPrerequisiteArchetypeLevel(LifeSensate.featGuid, CharacterClassRefs.KineticistClass.ToString(), level: 5)
                .AddFacts(new() { ability })
                .Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }

        }

    }


    public class CorrectedSupercharge {

        private static readonly LogWrapper Logger = LogWrapper.Get("CorrectedSupercharge");

        public static readonly string featName = "Supercharge";
        public static readonly string featGuid = "f238fee8-e3a6-44bf-a1cb-b8feab911c35";
        public static readonly string featureDescription = "CorrectedSupercharge.Description";
        public static readonly string featureName = "CorrectedSupercharge.Name";


        public static void Configure() {

            try {

                FeatureConfigurator feature = FeatureConfigurator.New(featName, featGuid).CopyFrom(FeatureRefs.Supercharge.ToString(), x => true);

                if (feature != null) {

                    feature.SetDescription(featureDescription);
                    feature.SetDisplayName(featureName);

                    feature.Configure();

                }

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }

        }

    }


    public class LifeFont {

        private static readonly LogWrapper Logger = LogWrapper.Get("LifeFont");

        public static readonly string featName = "LifeFont";
        public static readonly string featGuid = "6b05a12b-5dc9-4757-90bb-6dcd7a444cac";


        public static readonly string featureName = featName + ".Name";
        public static readonly string featureDescription = featName + ".Description";


        public static void Configure() {

            try {

                FeatureConfigurator feature = FeatureConfigurator.New(featName, featGuid).CopyFrom(FeatureRefs.OverwhelmingSoulOverwhelmingPowerFeature.ToString(), x => true);

                if (feature != null) {

                    feature.SetDescription(featureDescription);
                    feature.SetDisplayName(featureName);
                    feature.SetIsClassFeature(true);

                    feature.Configure();

                }

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }

        }

    }

    // Custom burn component based on Overwhelming Soul
    public class LifeAttunement {

        private static readonly LogWrapper Logger = LogWrapper.Get("LifeAttunement");

        public static readonly string featName = "LifeAttunement";
        public static readonly string featGuid = "f3503d52-5fa9-45dc-b0e0-1e8d7583ee41";


        public static readonly string featureName = featName + ".Name";
        public static readonly string featureDescription = featName + ".Description";


        public static void Configure() {

            try {

                FeatureConfigurator feature = FeatureConfigurator.New(featName, featGuid).CopyFrom(FeatureRefs.BurnFeature.ToString(), x => true);

                feature.SetDescription(featureDescription);
                feature.SetDisplayName(featureName);
                feature.SetIsClassFeature(true);

                feature.EditComponents<RecalculateOnStatChange>(
                    c => c.UseKineticistMainStat = false,
                    x => true
                );

                feature.EditComponents<RecalculateOnStatChange>(
                    c => c.Stat = StatType.Intelligence,
                    x => true
                );

                feature.EditComponents<AddKineticistPart>(
                    c => c.MainStat = StatType.Intelligence,
                    x => true
                );

                feature.Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }

        }

    }

    public class EmpoweredHealing {

        private static readonly LogWrapper Logger = LogWrapper.Get("EmpoweredHealing");

        public static readonly string featName = "EmpoweredHealing";
        public static readonly string featGuid = "53905737-9454-4e3a-bc42-38e21e4f5e52";

        public static void Configure() {

            try {

                FeatureConfigurator feature = FeatureConfigurator.New(featName, featGuid);

                feature.SetDescription(featName + ".Description")
                .SetDisplayName(featName + ".Name")
                .SetIsClassFeature(true)
                .AddAutoMetamagic(
                    abilities: [AbilityRefs.KineticHealerAbility.ToString(), AbilityRefs.HealingBurstAbility.ToString()],
                    allowedAbilities: AutoMetamagic.AllowedType.Any,
                    checkSpellbook: false,
                    metamagic: Metamagic.Empower
                )
                .Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }

        }

    }

    public class EmpoweredHealing2 {

        private static readonly LogWrapper Logger = LogWrapper.Get("EmpoweredHealing2");

        public static readonly string featName = "EmpoweredHealing2";
        public static readonly string featGuid = "715cd431-e4b5-4eee-b78a-8c7b043399d1";

        public static void Configure() {

            try {

                FeatureConfigurator feature = FeatureConfigurator.New(featName, featGuid);

                feature.SetDescription(featName + ".Description")
                .SetDisplayName(featName + ".Name")
                .SetIsClassFeature(true)
                .AddAutoMetamagic(
                    abilities: [AbilityRefs.HealingBurstAbility.ToString()],
                    allowedAbilities: AutoMetamagic.AllowedType.Any,
                    checkSpellbook: false,
                    metamagic: Metamagic.Empower
                )
                .Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }

        }

    }

    public class SelectiveHealing {

        private static readonly LogWrapper Logger = LogWrapper.Get("SelectiveHealing");

        public static readonly string featName = "SelectiveHealing";
        public static readonly string featGuid = "400c95eb-cee3-4137-b334-43ea64ba3556";

        public static readonly string abilityName = "SelectiveHealingAbility";
        public static readonly string abilityGuid = "007be405-824d-44cd-8593-9f21bda60170";


        public static void Configure() {

            try {

                BlueprintFeature baseFeature = BlueprintTool.Get<BlueprintFeature>(FeatureRefs.SelectiveSpellFeat.ToString());

                BlueprintActivatableAbility ability = ActivatableAbilityConfigurator.New(abilityName, abilityGuid)
                .SetDisplayName(featName + ".Name")
                .SetDescription(featName + ".Description")
                .SetIcon(baseFeature.m_Icon)
                .AddAutoMetamagic(
                    [
                        AbilityRefs.HealingBurstAbility.ToString()
                    ]
                    ,null, null, null, null, null, 20, Metamagic.Selective
                )
                .Configure();


                FeatureConfigurator feature = FeatureConfigurator.New(featName, featGuid);

                feature.SetDescription(featName + ".Description");
                feature.SetDisplayName(featName + ".Name");
                feature.SetIcon (baseFeature.m_Icon);
                feature.SetIsClassFeature(true);
                feature.AddFacts(new() { ability });
                feature.Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }

        }

    }


    public static class LingeringEnergiesResource {

        private static readonly LogWrapper Logger = LogWrapper.Get("LingeringEnergiesResource");

        public static BlueprintAbilityResource ResourcePool { get; private set; }

        // Resource
        public static readonly string featName = "LingeringEnergiesResource";
        public static readonly string featGuid = "94b6e209-0297-4d2e-892d-c131a11a1dab";

        public static void Configure() {

            try {

                LingeringEnergiesResource.ResourcePool = AbilityResourceConfigurator.New(
                    featName, // Unique internal name
                    featGuid // Unique GUID for the resource
                )
                .SetMaxAmount(
                    ResourceAmountBuilder.New(0).IncreaseByStat(StatType.Intelligence)
                )
                .SetUseMax(true) // Enforce max size
                .Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }


        }
    }


    public static class LingeringEnergies {

        private static readonly LogWrapper Logger = LogWrapper.Get("LingeringEnergies");

        
        public static BlueprintFeature Feature { get; private set; }

        //Feature
        public static readonly string featName = "LingeringEnergies";
        public static readonly string featGuid = "b72fba98-2aa8-4113-bbc2-600601c3e890";


        public static void Configure() {

            try {

                //Resource Assignment
                LingeringEnergies.Feature = FeatureConfigurator.New(
                    featName,
                    featGuid
                )
                .SetDisplayName(featName + ".Name")
                .SetDescription(featName + ".Description")
                .AddAbilityResources(
                    resource: LingeringEnergiesResource.ResourcePool.ToReference<BlueprintAbilityResourceReference>(),
                    restoreAmount: true // Restore the pool when resting
                ).AddRestTrigger (ActionsBuilder.New().HealBurn())
                .SetIsClassFeature(true)
                .Configure();

            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }
        }

    }

}
