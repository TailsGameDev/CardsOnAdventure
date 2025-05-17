using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockEffect : DefenseEffect
{
    public override void ExecuteEffect(int damage, Battlefield obf, Battlefield abf, int toBeDamagedIndex, GameObject attackVFX)
    {
        int tobeDamagedIndexConsideringBlock = obf.GetIndexInFrontOf(toBeDamagedIndex);
        int damageConsideringBlock = damage + damage;

        Card toBeDamaged = obf.GetReferenceToCardAt(tobeDamagedIndexConsideringBlock);

        toBeDamaged.ShowDefenseVFXandSFX(abf.transform.position.y);

        base.ExecuteEffect(damageConsideringBlock, obf, abf, tobeDamagedIndexConsideringBlock, attackVFX);
    }

    public static bool IsThereABlockerInFrontOfTarget(Battlefield obf, int toBeDamagedIndex)
    {
        return obf.IsThereACardInFrontOf(toBeDamagedIndex) && obf.GetCardInFrontOf(toBeDamagedIndex).HasBlockSkill();
    }
}
