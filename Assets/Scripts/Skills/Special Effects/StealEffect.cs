using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealEffect : SpecialEffect
{
    public override void ExecuteEffect(Battlefield obf, Battlefield abf, ref int targetIndex, GameObject specialVFX)
    {
        Card attacker = abf.GetSelectedCard();
        Card target = obf.GetReferenceToCardAt(targetIndex);
        attacker.Skills = target.Skills;
        target.Skills = skillsMediator.GetComponent<TheOnlySkillsMediatorInstance>().GetBasicAttackSkill();
    }
}
