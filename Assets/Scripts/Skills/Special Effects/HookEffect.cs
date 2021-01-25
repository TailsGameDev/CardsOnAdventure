using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookEffect : SpecialEffect
{
    private Battlefield obf;
    public override void ExecuteEffect(Battlefield obf, Battlefield abf, ref int targetIndex, GameObject specialVFX)
    {
        this.obf = obf;
        if (IsAttackingBackline(targetIndex))
        {
            int frontlineCardIndex = obf.GetIndexInFrontOf(targetIndex);

            if (obf.ContainsCardInIndex(frontlineCardIndex))
            {
                Card target = obf.GetReferenceToCardAt(targetIndex);
                ChildMaker.AdoptAndTeleport(target.transform, Instantiate(specialVFX).GetComponent<RectTransform>());

                obf.SwapCards(targetIndex, obf.GetVerticalNeighborIndex(targetIndex));
                targetIndex = obf.GetIndexInFrontOf(targetIndex);
            }
        }
    }

    private bool IsAttackingBackline(int targetIndex)
    {
        return targetIndex != obf.GetIndexInFrontOf(targetIndex);
    }
}
