using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldSkill : Skill
{
    //inherits xxxxVFX

    // Tank
    [SerializeField]
    private bool block = false;

    [SerializeField]
    private SpecialEffect specialEffect = null;

    [SerializeField]
    private float frontlineTargetDamageMultiplier = 1.0f;
    [SerializeField]
    private float backlineTargetDamageMultiplier = 0.5f;
    [SerializeField]
    private float horizontalSpreadDamageMultiplier = 0.0f;
    [SerializeField]
    private float verticalSpreadDamageMultiplier = 0.0f;

    [SerializeField]
    private float damageReductionPercentage = 0.0f;
    [SerializeField]
    private float damageReflectionPercentage = 0.0f;

    private Battlefield obf;
    private Battlefield attackerBattleField;

    private Card attacker;
    private int targetIndex;

    public float DamageReductionPercentage { get => damageReductionPercentage; }
    public float DamageReflectionPercentage { get => damageReflectionPercentage; }

    public override void ApplyEffectsConsideringSelectedTarget(
                                                                Battlefield opponentBattlefield,
                                                                Battlefield attackerBattlefield)
    {
        this.attacker = attackerBattlefield.GetSelectedCard();
        this.obf = opponentBattlefield;
        this.attackerBattleField = attackerBattlefield;

        targetIndex = obf.GetSelectedIndex();

        int baseAttack = attacker.AttackPower;

        // Normal
        int damageToTarget;
        if (IsAttackingBackline(targetIndex) && obf.IsThereACardInFrontOf(targetIndex))
        {
            damageToTarget = (int)(baseAttack * backlineTargetDamageMultiplier);

            // Guardian [Block]
            if (obf.GetCardInFrontOf(targetIndex).HasBlockSkill())
            {
                targetIndex = obf.GetIndexInFrontOf(targetIndex);
                obf.GetReferenceToCardAt(targetIndex).ShowDefenseVFXandSFX(attackerBattleField.transform.position.y);
            }
        }
        else
        {
            damageToTarget = (int)(baseAttack * frontlineTargetDamageMultiplier);
        }

        int horizontalSpreadDamage = (int)(baseAttack * horizontalSpreadDamageMultiplier);
        int verticalSpreadDamage = (int)(baseAttack * verticalSpreadDamageMultiplier);

        if (specialEffect != null)
        {
            specialEffect.ExecuteEffect(obf, attackerBattleField, ref targetIndex, specialVFX);
        }

        skillsMediator.PlaySFX(attackSFX);

        // Damage
        DamageCard(targetIndex, damageToTarget);

        DamageCard(obf.GetCardIndexBeside(targetIndex), horizontalSpreadDamage);

        DamageCard(obf.GetVerticalNeighborIndex(targetIndex), verticalSpreadDamage);
    }

    private bool IsAttackingBackline(int targetIndex)
    {
        return targetIndex != obf.GetIndexInFrontOf(targetIndex);
    }

    private void DamageCard(int index, int damage)
    {
        if (damage > 0 && obf.IsSlotIndexOccupied(index))
        {
            Card target = obf.GetReferenceToCardAt(index);

            ShowAttackVFXInFrontOf(target.transform);

            target.ShowDefenseVFXandSFXIfHasBlockOrReflect(attackerBattleField.transform.position.y);

            target.TakeDamage((int)(damage * ( 1.0f - target.GetDamageReductionPercentage() ) ));

            attacker.TakeDamage((int)(damage * target.GetDamageReflectionPercentage()));
        }
    }

    private void ShowAttackVFXInFrontOf(Transform target)
    {
        if (attackVFX != null)
        {
            InstantiateObjAsSonOf(attackVFX, target.transform.parent);
        }
    }

    // The parent of the VFX must be the slot that holds the car, because the car can be destroyed on attack.
    void InstantiateObjAsSonOf(GameObject toInstantiate, Transform parent)
    {
        RectTransform instantiated = Instantiate(toInstantiate).GetComponent<RectTransform>();
        ChildMaker.AdoptAndTeleport(parent, instantiated);
    }

    public override bool HasBlockEffect()
    {
        return block;
    }

    public override bool HasHeavyArmorEffect()
    {
        return DamageReductionPercentage > 0.1f;
    }

    public override bool HasReflectEffect()
    {
        return DamageReflectionPercentage > 0.1f;
    }
}
