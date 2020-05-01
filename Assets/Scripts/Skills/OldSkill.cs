using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class OldSkill : Skill
{
    //inherits xxxxVFX

    // Tank
    [SerializeField]
    private bool block = false;

    [SerializeField]
    private bool ignoreOpponentBlock = false;

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
    [SerializeField]
    private int healItselfBeforeAttack = 0;

    private Battlefield obf;
    private Battlefield abf;

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
        this.abf = attackerBattlefield;

        targetIndex = obf.GetSelectedIndex();

        int baseAttack = attacker.AttackPower;

        int damageToTarget;
        if (IsAttackingBackline(targetIndex) && obf.IsThereACardInFrontOf(targetIndex))
        {
            damageToTarget = (int)(baseAttack * backlineTargetDamageMultiplier);
        }
        else
        {
            damageToTarget = (int)(baseAttack * frontlineTargetDamageMultiplier);
        }

        int horizontalSpreadDamage = (int)(baseAttack * horizontalSpreadDamageMultiplier);
        int verticalSpreadDamage = (int)(baseAttack * verticalSpreadDamageMultiplier);

        if (specialEffect != null)
        {
            specialEffect.ExecuteEffect(obf, abf, ref targetIndex, specialVFX);
        }

        attacker.HealNotExceedingDoubleVitalityLimit(healItselfBeforeAttack);

        skillsMediator.PlaySFX(attackSFX);

        if (spawnAboveItselfWhenAttackVFX != null)
        {
            GameObject vfx = Instantiate(spawnAboveItselfWhenAttackVFX);
            ChildMaker.AdoptTeleportAndScale(attacker.transform, vfx.GetComponent<RectTransform>());
        }

        // Damage
        DamageCard(targetIndex, damageToTarget);

        DamageCard(obf.GetCardIndexBeside(targetIndex), horizontalSpreadDamage);

        DamageCard(obf.GetVerticalNeighborIndex(targetIndex), verticalSpreadDamage);
    }

    private bool IsAttackingBackline(int targetIndex)
    {
        return targetIndex != obf.GetIndexInFrontOf(targetIndex);
    }

    private void DamageCard(int toBeDamagedIndex, int damage)
    {
        if (DefenseEffect.IsSuccessfulAttack(damage, obf, abf, toBeDamagedIndex))
        {
            DefenseEffect defenseEffectToExecute;

            if (ShouldDoBlock(toBeDamagedIndex))
            {
                defenseEffectToExecute = skillsMediator.BlockDefenseEffect;
            } 
            else
            {
                defenseEffectToExecute = skillsMediator.RegularDefenseEffect;
            }

            bool isDoubleAttack = frontlineTargetDamageMultiplier > 1.9f;
            
            if (isDoubleAttack)
            {
                defenseEffectToExecute.ExecuteEffect(damage/2, obf, abf, toBeDamagedIndex, attackVFX);
                if (obf.ContainsCardInIndex(toBeDamagedIndex) && abf.ContainsCardInIndex(abf.GetSelectedIndex()))
                {
                    defenseEffectToExecute.ExecuteEffect(damage/2, obf, abf, toBeDamagedIndex, attackVFX);
                }
            } 
            else
            {
                defenseEffectToExecute.ExecuteEffect(damage, obf, abf, toBeDamagedIndex, attackVFX);
            }
        }
    }

    private bool ShouldDoBlock(int toBeDamagedIndex)
    {
        return IsAttackingBackline(targetIndex) &&
            BlockEffect.IsThereABlockerInTheFrontOfTarget(obf, toBeDamagedIndex)
            && !ignoreOpponentBlock;
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
