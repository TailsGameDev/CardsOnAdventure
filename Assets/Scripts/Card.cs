using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class Card : SkillsMediatorUser
{
    [SerializeField]
    private Classes classe = Classes.NOT_A_CLASS;

    [SerializeField]
    private int attackPower = 99;
    [SerializeField]
    private int vitality = 99;

    [SerializeField]
    private Text vitalityText = null;

    [SerializeField]
    private Text attackPowerText = null;

    [SerializeField]
    private OldSkill skills = null;

    [SerializeField]
    private Text skillText = null;

    [SerializeField]
    private Image obfuscator = null;

    [SerializeField]
    private Image cardImage = null;

    private bool freezing = false;

    private GameObject freezingEffect = null;

    private Battlefield battlefield;

    [SerializeField]
    private Sprite horizontalSprite = null;

    public bool Freezing { get => freezing; }
    public int Vitality { get => vitality; }
    public int AttackPower { get => attackPower; set => attackPower = value; }
    public Battlefield Battlefield { get => battlefield; set => battlefield = value; }
    public Classes Classe { get => classe; }

    private void Start()
    {
        if (skills == null)
        {
            skills = skillsMediator.GetBasicAttackSkill();
        }
    }

    public OldSkill Skills {
        get => skills;
        set {
            skills = value;
            skillText.text = skills.Acronym;
        }
    }

    private void Awake()
    {
        attackPowerText.text = AttackPower.ToString();
        skillText.text = skills.Acronym;
        SetVitality(Vitality);
    }

    public int GetVitality()
    {
        return Vitality;
    }

    public void AttackSelectedCard(Battlefield opponentBattlefield, Battlefield attackerBattlefield)
    {
        Skills.ApplyEffectsConsideringSelectedTarget(opponentBattlefield, attackerBattlefield);
    }

    #region damage
    public void TakeDamage(int damage)
    {
        SetVitality(Vitality - damage);
    }

    public void AjustCardToDifficult(int difficultyLevel)
    {
        attackPower = attackPower * difficultyLevel;
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
            battlefield.Remove(this);
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

    public void ShowDefenseVFXandSFXIfHasBlockOrReflect(float attackerYPosition)
    {
        if(skills.HasReflectEffect() || skills.HasHeavyArmorEffect())
        {
            ShowDefenseVFXandSFX( attackerYPosition );
        }
    }

    public void ShowDefenseVFXandSFX(float attackerYPosition)
    {
        ShowDefenseVFX(attackerYPosition);
        ShowDefenseSFX();
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
            vfx.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            vfx.transform.eulerAngles = new Vector3(0, 0, 180);
        }
    }

    public void ChangeToHorizontalVersion()
    {
        cardImage.sprite = horizontalSprite;
        attackPowerText.transform.Rotate(new Vector3(0,0,-90));
        vitalityText.transform.Rotate(new Vector3(0, 0, -90));
        skillText.transform.parent.Rotate(new Vector3(0, 0, -90));

        vitalityText.transform.parent.position = attackPowerText.transform.position;
        attackPowerText.transform.parent.position = skillText.transform.position + (new Vector3(100, 0 , 0)/2);
        skillText.transform.parent.position = skillText.transform.position + new Vector3(0,cardImage.GetComponent<RectTransform>().sizeDelta.x,0);
    }

    private void ShowDefenseSFX()
    {
        skills.PlayDefenseSFX();
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

    public float GetDamageReductionPercentage()
    {
        return skills.DamageReductionPercentage;
    }
    public float GetDamageReflectionPercentage()
    {
        return skills.DamageReflectionPercentage;
    }

    public string GetColoredTitleForTooltip()
    {
        string colorHexCode = ClassInfo.GetColorHexCodeOfClass(classe);
        return "<color=#"+ colorHexCode + ">"+ classe+"</color>";
    }

    public Sprite GetCardSprite()
    {
        return cardImage.sprite;
    }

    public string GetExplanatoryText()
    {
        return skills.GetExplanatoryText() + "\n" +
               "<color=#FD7878>Attack Power: " + attackPower + "</color>\n" +
               "<color=#9EFA9D>Vitality: " + vitality + "</color>\n";
               
    }

    public void SumPlayerBonuses()
    {
        attackPower += ClassInfo.GetAttackPowerBonusOfClass(classe);
        attackPowerText.text = attackPower.ToString();

        vitality += ClassInfo.GetVitalityBonusOfClass(classe);
        vitalityText.text = vitality.ToString();
    }
}
