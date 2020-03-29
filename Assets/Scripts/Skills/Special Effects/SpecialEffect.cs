using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialEffect : SkillsMediatorUser
{
    public abstract void ExecuteEffect(Battlefield obf, Battlefield abf, ref int targetIndex, GameObject specialVFX);
}