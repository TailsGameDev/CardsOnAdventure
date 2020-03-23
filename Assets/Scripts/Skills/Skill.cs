using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField]
    private string acronym = "";

    // Warrior
    [SerializeField]
    private bool pierce = false;

    [SerializeField]
    private bool doubleAttack = false;

    // Mage
    [SerializeField]
    private bool area = false;

    [SerializeField]
    private bool freezing = false;

    // Rougue
    [SerializeField]
    private bool hook = false;

    [SerializeField]
    private bool steal = false;

    // Tank
    [SerializeField]
    private bool block = false;

    [SerializeField]
    private bool heavyArmor = false;

    [SerializeField]
    private bool reflect = false;

    [SerializeField]
    private bool sanctuary = false;

    private Battlefield obf;
    private Battlefield attackerBattleField;

    private Card attacker;

    public string Acronym { get => acronym; }

    private bool canMakeDoubleAttack = false;

    public void ApplyEffectsConsideringSelectedTarget(Battlefield opponentBattlefield, Battlefield attackerBattlefield)
    {
        this.attacker = attackerBattlefield.GetSelectedCard();
        this.obf = opponentBattlefield;
        this.attackerBattleField = attackerBattlefield;

        int baseAttackPower = attacker.GetAttackPower();

        int damageToTarget = baseAttackPower;
        int damageToBeside = 0;
        int damageToVerticalNeighbor = 0;

        int targetIndex = obf.GetSelectedIndex();

        Card target = obf.GetReferenceToCardAt(targetIndex);

        // Normal Attack
        if (IsAttackingBackline())
        {
            if (obf.IsThereACardInFrontOf(targetIndex))
            {
                damageToTarget = baseAttackPower / 2;
                damageToVerticalNeighbor = 0;
                damageToBeside = 0;

                if (obf.GetCardInFrontOf(targetIndex).HasBlockSkill())
                {
                    damageToTarget = 0;
                    damageToVerticalNeighbor = baseAttackPower;
                    damageToBeside = 0;
                }
            }
            else
            {
                // No card in front of target
                damageToTarget = baseAttackPower;
                damageToVerticalNeighbor = 0;
                damageToBeside = 0;
            }
        }


        // Warrior
        if (pierce)
        {
            if (IsAttackingFrontline())
            {
                damageToTarget = baseAttackPower;
                damageToVerticalNeighbor = baseAttackPower / 2;
                damageToBeside = 0;
            }
            else
            {
                damageToTarget = baseAttackPower / 2;
                damageToVerticalNeighbor = baseAttackPower;
                damageToBeside = 0;
            }
        }

        // Mage
        if (area)
        {
            damageToTarget = baseAttackPower / 2;
            damageToVerticalNeighbor = baseAttackPower / 2;
            damageToBeside = baseAttackPower / 2;
        }

        if (freezing)
        {
            target.Freezing = true;
        }

        // Rougue
        if (hook)
        {
            if (IsAttackingBackline())
            {
                int frontlineCardIndex = obf.GetIndexInFrontOf(targetIndex);

                if (obf.ContainsCardInIndex(frontlineCardIndex))
                {
                    obf.SwapCards(targetIndex, obf.GetVerticalNeighborIndex(targetIndex));

                    damageToTarget = 0;
                    damageToVerticalNeighbor = baseAttackPower / 2;
                    damageToBeside = 0;
                }
            }
        }

        if (steal)
        {
            attacker.Skills = target.Skills;
            target.Skills = SkillReferences.GetBasicAttackSkill();
        }

        // Healer
        if (sanctuary)
        {
            Battlefield abf = attackerBattlefield;
            int healer = abf.GetSelectedIndex();
            HealCard(healer, 2);
            HealCard(abf.GetCardIndexBeside(healer),2);
            HealCard(abf.GetVerticalNeighborIndex(healer),2);
        }

        // Damage
        DamageCard(targetIndex, damageToTarget);

        DamageCard(obf.GetCardIndexBeside(targetIndex), damageToBeside);

        DamageCard(obf.GetVerticalNeighborIndex(targetIndex), damageToVerticalNeighbor);


        // Warrior again
        if (doubleAttack)
        {
            if (canMakeDoubleAttack)
            {
                canMakeDoubleAttack = false;
                ApplyEffectsConsideringSelectedTarget(obf, attackerBattleField);
            }
            else
            {
                canMakeDoubleAttack = true;
            }
            
        } 
    }

    private bool IsAttackingFrontline()
    {
        int target = obf.GetSelectedIndex();
        return target == obf.GetIndexInFrontOf(target);
    }

    private bool IsAttackingBackline()
    {
        int target = obf.GetSelectedIndex();
        return target != obf.GetIndexInFrontOf(target);
    }

    private void DamageCard(int index, int damage)
    {
        if (obf.IsSlotIndexOccupied(index))
        {
            Card target = obf.GetReferenceToCardAt(index);

            if (target.HasHeavyArmorSkill())
            {
                target.TakeDamage( damage / 2 );
            }
            else
            {
                target.TakeDamage(damage);
            }

            if (target.HasReflectSkill())
            {
                attacker.TakeDamage( damage / 2 );
            }
        }
    }

    private void HealCard(int index, int healAmount)
    {
        if (attackerBattleField.IsSlotIndexOccupied(index))
        {
            Card toBeHealed = attackerBattleField.GetReferenceToCardAt(index);
            toBeHealed.Heal(healAmount);
        }
    }

    public bool HasBlockEffect()
    {
        return block;
    }

    public bool HasHeavyArmorEffect()
    {
        return heavyArmor;
    }

    public bool HasReflectEffect()
    {
        return reflect;
    }
}
