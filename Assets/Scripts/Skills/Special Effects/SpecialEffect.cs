using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialEffect : SkillsMediatorUser
{
    public abstract void ExecuteEffect(Battlefield obf, Battlefield abf, ref int targetIndex, GameObject specialVFX);

    protected void InstantiateObjAsSonOf(GameObject toInstantiate, TransformWrapper parent)
    {
        TransformWrapper transformWrapper = new TransformWrapper(Instantiate(toInstantiate).transform);
        ChildMaker.AdoptAndTeleport(parent, transformWrapper);
    }
}