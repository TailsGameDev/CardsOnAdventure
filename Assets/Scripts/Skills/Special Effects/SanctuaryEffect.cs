using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanctuaryEffect : SpecialEffect
{
    private Battlefield abf;
    private GameObject specialVFX;
    public override void ExecuteEffect(Battlefield obf, Battlefield abf, ref int targetIndex, GameObject specialVFX)
    {
        this.abf = abf;
        this.specialVFX = specialVFX;

        int healer = abf.GetSelectedIndex();
        HealCard(healer, 2);
        HealCard(abf.GetCardIndexBeside(healer), 2);
        HealCard(abf.GetVerticalNeighborIndex(healer), 2);
    }

    private void HealCard(int index, int healAmount)
    {
        if (abf.IsSlotIndexOccupied(index))
        {
            Card toBeHealed = abf.GetReferenceToCardAt(index);
            toBeHealed.InconditionalHealing(healAmount);

            InstantiateObjAsSonOf(specialVFX, toBeHealed.transform.parent);
        }
    }

    void InstantiateObjAsSonOf(GameObject toInstantiate, Transform parent)
    {
        RectTransform instantiated = Instantiate(toInstantiate).GetComponent<RectTransform>();
        ChildMaker.AdoptAndTeleport(parent, instantiated);
    }
}
