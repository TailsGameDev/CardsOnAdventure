using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseEffect : MonoBehaviour
{
    private int damage;
    private Card toBeDamaged;

    public virtual void ExecuteEffect(int damage, Battlefield obf, Battlefield abf, int toBeDamagedIndex, GameObject attackVFX)
    {
        // References
        this.damage = damage;
        toBeDamaged = obf.GetReferenceToCardAt(toBeDamagedIndex);

        // VFX and SFX
        ShowAttackVFXInFrontOf(toBeDamaged.TransformWrapper, attackVFX);
        toBeDamaged.ShowDefenseVFXandSFXIfHasBlockOrReflect(abf.transform.position.y);

        // Damage
        toBeDamaged.TakeDamageAndManageCardState(CalculateDamageWithReductionPercentage(), obf);

        // Damage reflection
        Card attacker = abf.GetSelectedCard();
        attacker.TakeDamageAndManageCardState(CalculateReflectedDamage(), abf);
    }

    public static bool IsSuccessfulAttack(int damage, Battlefield obf, Battlefield abf, int toBeDamagedIndex)
    {
        // Attacker might have already died with reflection
        Card attacker = abf.GetSelectedCard();
        return damage > 0 && obf.ContainsCardInIndex(toBeDamagedIndex) && attacker != null;
    }

    // The parent of the VFX must be the slot that holds the car, because the car can be destroyed on attack.
    private void ShowAttackVFXInFrontOf(TransformWrapper target, GameObject attackVFX)
    {
        if (attackVFX != null)
        {
            InstantiateObjAsSonOf(attackVFX, target.Parent);
        }
    }

    private void InstantiateObjAsSonOf(GameObject toInstantiate, TransformWrapper parent)
    {
        TransformWrapper transformWrapper = new (Instantiate(toInstantiate).transform);
        ChildMaker.AdoptAndTeleport(parent, transformWrapper);
    }

    private int CalculateDamageWithReductionPercentage()
    {
        return (int)(damage * (1.0f - toBeDamaged.GetDamageReductionPercentage()));
    }

    private int CalculateReflectedDamage()
    {
        return (int)(damage * toBeDamaged.GetDamageReflectionPercentage());
    }
}
