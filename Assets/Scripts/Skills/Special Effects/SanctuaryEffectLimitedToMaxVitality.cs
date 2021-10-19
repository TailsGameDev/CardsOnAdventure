using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanctuaryEffectLimitedToMaxVitality : SpecialEffect
{
    private Battlefield abf;
    private GameObject specialVFX;

    private const int HEAL_SUCCEED = 1;
    private const int COULD_NOT_HEAL = 0;

    private const int HOW_MANY_CAN_BE_HEALED = 2;

    public override void ExecuteEffect(Battlefield obf, Battlefield abf, ref int targetIndex, GameObject specialVFX)
    {
        this.abf = abf;
        this.specialVFX = specialVFX;

        int healerIndex = abf.GetSelectedIndex();
        Card healerCard = abf.GetReferenceToCardAt(healerIndex);

        int healAmount = healerCard.AttackPower / 2;

        int howManyWereHealed = 0;
        howManyWereHealed += HealingAlgorithm(healerIndex, healAmount);
        howManyWereHealed += HealingAlgorithm(abf.GetCardIndexBeside(healerIndex), healAmount);
        if (howManyWereHealed < HOW_MANY_CAN_BE_HEALED)
        {
            howManyWereHealed += HealingAlgorithm(abf.GetVerticalNeighborIndex(healerIndex), healAmount);
        }
        if (howManyWereHealed < HOW_MANY_CAN_BE_HEALED)
        {
            HealingAlgorithm(abf.GetCardIndexBeside(abf.GetVerticalNeighborIndex(healerIndex)), healAmount);
        }
    }

    private int HealingAlgorithm(int index, int healAmount)
    {
        int healed = COULD_NOT_HEAL;
        if (abf.ContainsCardInIndex(index))
        {
            Card toBeHealed = abf.GetReferenceToCardAt(index);

            int vitalityBefore = toBeHealed.Vitality;
            toBeHealed.HealNotExceedingDoubleVitalityLimit(healAmount);
            int vitalityAfter = toBeHealed.Vitality;

            if (vitalityAfter > vitalityBefore)
            {
                InstantiateObjAsSonOf(specialVFX, toBeHealed.TransformWrapper.Parent);
                healed = HEAL_SUCCEED;
            }
        }
        return healed;
    }
}
