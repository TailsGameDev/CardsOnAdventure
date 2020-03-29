using System;
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

    [SerializeField]
    private Image obfuscator = null;

    private bool freezing = false;

    private GameObject freezingEffect = null;

    public Skill Skills {
        get => skills;
        set {
            skills = value;
            skillText.text = skills.Acronym;
        }
    }
    public bool Freezing { get => freezing; }
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
        Skills.CanMakeDoubleAttack = true;
        Skills.ApplyEffectsConsideringSelectedTarget(opponentBattlefield, attackerBattlefield);
    }

    #region damage
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
        vitality = value;
        vitalityText.text = value.ToString();
        if (Vitality <= 0)
        {
            RemoveFreezing();
            Destroy(gameObject);
        }
    }
    #endregion

    #region Has XXX Effect
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
    #endregion

    public string GetSkillFullName()
    {
        return skills.FullName;
    }

    public void SetObfuscate(bool obfuscate)
    {
        obfuscator.gameObject.SetActive(obfuscate);
    }

    public void ShowDefenseVFX(float attackerYPosition)
    {
        Vector3 forwards = new Vector3(0, 0, -transform.position.y);
        Vector3 upwards = new Vector3(0, 0, -1);
        Quaternion lookRotation = Quaternion.LookRotation(forwards, upwards);
        GameObject vfx = Instantiate(skills.DefenseVFX, transform.position, Quaternion.identity);
        vfx.GetComponent<RectTransform>().SetParent(transform, false);
        vfx.GetComponent<RectTransform>().localPosition = Vector3.zero;

        float y = attackerYPosition - transform.position.y;

        if (y > 0)
        {
            vfx.transform.eulerAngles = new Vector3(0,0, 0);
        }
        else
        {
            vfx.transform.eulerAngles = new Vector3(0, 0,180);
        }
    }

    public void ApplyFreezing(GameObject freezingEffect)
    {
        RemoveFreezing();
        freezing = true;
        this.freezingEffect = freezingEffect;
        ChildMaker.AdoptAndTeleport(transform, freezingEffect.GetComponent<RectTransform>());
    }

    public void RemoveFreezing()
    {
        freezing = false;
        if (freezingEffect != null)
        {
            Destroy(freezingEffect);
        }
        freezingEffect = null;
    }

    internal void ShowDefenseSFX()
    {
        skills.PlayDefenseSFX();
    }
}
