using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezingEffect : SpecialEffect
{
    public override void ExecuteEffect(Battlefield obf, Battlefield abf, ref int targetIndex, GameObject specialVFX)
    {
        obf.GetReferenceToCardAt(targetIndex).ApplyFreezing(Instantiate(specialVFX));
    }
}
