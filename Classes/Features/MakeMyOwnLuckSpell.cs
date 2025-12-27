using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Spells;
using BlueprintCore.Blueprints.Configurators.Classes.Spells;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Commands.Base;
using System;

namespace AviaryClasses.Classes.Features {
    public class MakeMyOwnLuckSpell {
        private static readonly LogWrapper Logger = LogWrapper.Get("MakeMyOwnLuckSpell");

        public static readonly string spellName = "IMakeMyOwnLuck";
        public static readonly string spellGuid = "0b033073-0f00-44a9-a755-db59952c3c85";
        public static readonly string spellListName = "KeenEyedAdventurerSpellList";
        public static readonly string spellListGuid = "0fd8d36c-cf8e-4d82-853f-08f3eb130b1f";
        public static readonly string spellbookName = "KeenEyedAdventurerSpellbook";
        public static readonly string spellbookGuid = "8405e7b6-af68-4453-a4cd-4cf94f99f407";

        public static void Configure() {
            try {
                var baseAbility = BlueprintTool.Get<BlueprintAbility>(AbilityRefs.WitchHexFortuneAbility.ToString());

                SpellListConfigurator.New(spellListName, spellListGuid)
                    .CopyFrom(SpellListRefs.WitchSpellList)
                    .Configure();

                SpellbookConfigurator.New(spellbookName, spellbookGuid)
                    .CopyFrom(SpellbookRefs.WitchSpellbook)
                    .SetSpellList(spellListGuid)
                    .Configure();

                AbilityConfigurator.NewSpell(spellName, spellGuid, SpellSchool.Divination, canSpecialize: true)
                    .SetDisplayName(spellName + ".Name")
                    .SetDescription(spellName + ".Description")
                    .SetIcon(baseAbility.m_Icon)
                    .SetRange(AbilityRange.Personal)
                    .SetCanTargetEnemies(false)
                    .SetCanTargetFriends(false)
                    .SetCanTargetSelf(true)
                    .SetSpellResistance(false)
                    .SetEffectOnAlly(AbilityEffectOnUnit.Helpful)
                    .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
                    .SetActionType(UnitCommand.CommandType.Standard)
                    .SetAvailableMetamagic(
                        Metamagic.Extend,
                        Metamagic.Heighten,
                        Metamagic.Quicken,
                        Metamagic.Reach
                    )
                    .SetLocalizedDuration(Duration.MinutePerLevel)
                    .AddAbilityEffectRunAction(
                        actions: ActionsBuilder.New()
                            .ApplyBuff(
                                BuffRefs.WitchHexFortuneBuff.ToString(),
                                durationValue: ContextDuration.Variable(ContextValues.Rank(), DurationRate.Minutes)
                            )
                    )
                    .AddContextRankConfig(ContextRankConfigs.CasterLevel())
                    .AddToSpellList(8, spellListGuid)
                    .Configure();
            } catch (Exception ex) {
                Logger.Error(ex.ToString());
            }
        }
    }
}
