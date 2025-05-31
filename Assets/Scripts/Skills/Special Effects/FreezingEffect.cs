using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezingEffect : SpecialEffect
{
    public override void ExecuteEffect(Battlefield obf, Battlefield abf, ref int targetIndex, GameObject specialVFX)
    {
        int toBeFreezedIndex;

        if ( BlockEffect.IsThereABlockerInFrontOfTarget(obf, targetIndex) )
        {
            toBeFreezedIndex = obf.GetIndexInFrontOf(targetIndex);
        }
        else
        {
            toBeFreezedIndex = targetIndex;
        }

        obf.GetReferenceToCardAt(toBeFreezedIndex).ApplyFreezing(Instantiate(specialVFX));
    }
}
