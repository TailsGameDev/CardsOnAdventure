using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField]
    private int attackPower = 0;
    [SerializeField]
    private int vitality = 999;

    [SerializeField]
    private Text vitalityText = null;

    [SerializeField]
    private Text attackPowerText = null;

    [SerializeField]
    private Skill skills = null;

    [SerializeField]
    private Text skillText = null;

    private bool freezing = false;

    public Skill Skills {
        get => skills;
        set {
            skills = value;
            skillText.text = skills.Acronym;
        }
    }
    public bool Freezing { get => freezing; set => freezing = value; }
    public int Vitality { get => vitality; }

    private void Awake()
    {
        attackPowerText.text = attackPower.ToString();
        skillText.text = skills.Acronym;
        SetVitality(Vitality);
    }

    public int GetAttackPower()
    {
        return attackPower;
    }

    public int GetVitality()
    {
        return Vitality;
    }

    public void AttackSelectedCard(Battlefield opponentBattlefield, Battlefield attackerBattlefield)
    {
        Skills.ApplyEffectsConsideringSelectedTarget(opponentBattlefield, attackerBattlefield);
    }

    public void TakeDamage(int damage)
    {
        SetVitality(Vitality - damage);
    }

    public void Heal(int healAmount)
    {
        SetVitality(Vitality + healAmount);
    }

    private void SetVitality(int value)
    {
        if (Vitality != value)
        {
            Debug.Log(gameObject.name + " set vitality to: " + value, this);
        }
        
        vitality = value;
        vitalityText.text = value.ToString();
        if (Vitality <= 0)
        {
            Destroy(gameObject);
        }
    }

    public bool HasBlockSkill()
    {
        return Skills.HasBlockEffect();
    }

    public bool HasHeavyArmorSkill()
    {
        return Skills.HasHeavyArmorEffect();
    }

    public bool HasReflectSkill()
    {
        return Skills.HasReflectEffect();
    }
}
