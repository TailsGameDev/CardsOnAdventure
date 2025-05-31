using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealEffect : SpecialEffect
{
    public override void ExecuteEffect(Battlefield obf, Battlefield abf, ref int targetIndex, GameObject specialVFX)
    {
        Card attacker = abf.GetSelectedCard();
        Card target;
        if (BlockEffect.IsThereABlockerInFrontOfTarget(obf, targetIndex))
        {
            target = obf.GetCardInFrontOf(targetIndex);
        } 
        else
        {
            target = obf.GetReferenceToCardAt(targetIndex);
        }
        attacker.Skill = target.Skill;
        target.Skill = skillsMediator.GetBasicAttackSkill();

        if (skillsMediator.IsTheSkillNamedStrong(attacker.Skill))
        {
            attacker.ModifyAttackPowerForThisMatch(valueToSum: +4+target.GetLevel() * 4);
            target.ModifyAttackPowerForThisMatch(valueToSum: -4-target.GetLevel() * 4);
        }
    }
}
