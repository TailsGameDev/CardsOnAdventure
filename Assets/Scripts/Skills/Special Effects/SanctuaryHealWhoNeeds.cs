using UnityEngine;

public class SanctuaryHealWhoNeeds : SpecialEffect
{
    private Battlefield abf;
    private GameObject specialVFX;

    public override void ExecuteEffect(Battlefield obf, Battlefield abf, ref int targetIndex, GameObject specialVFX)
    {
        this.abf = abf;
        this.specialVFX = specialVFX;

        int healerIndex = abf.GetSelectedIndex();

        Card healerCard = abf.GetReferenceToCardAt(healerIndex);
        int healAmount = healerCard.AttackPower;

        int whoNeeds = abf.GetIndexWithLowestVitalityThatCanBeHealed();
        HealingAlgorithm(whoNeeds, healAmount);
        whoNeeds = abf.GetIndexWithLowestVitalityThatCanBeHealed();
        HealingAlgorithm(whoNeeds, healAmount);
    }

    private void HealingAlgorithm(int index, int healAmount)
    {
        if (abf.ContainsCardInIndex(index))
        {
            Card toBeHealed = abf.GetReferenceToCardAt(index);

            int vitalityBefore = toBeHealed.Vitality;
            toBeHealed.HealNotExceedingDoubleVitalityLimit(healAmount);
            int vitalityAfter = toBeHealed.Vitality;

            if (vitalityAfter > vitalityBefore)
            {
                InstantiateObjAsSonOf(specialVFX, toBeHealed.TransformWrapper.Parent);
            }
        }
    }
}
