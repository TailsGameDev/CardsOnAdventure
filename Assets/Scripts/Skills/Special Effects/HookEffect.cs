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
                Transform vfx = Instantiate(specialVFX).transform;
                vfx.position = target.GetRectTransform().position;
                vfx.SetParent(UIBattle.parentOfDynamicUIThatMustAppear);
                // If the card is used agains the player, the VFX should display upside down.
                if (vfx.localPosition.y < 0)
                {
                    vfx.eulerAngles = new Vector3(0, 0, 180);
                }

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
