using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class Card : SkillsMediatorUser
{
    #region Attributes
    [SerializeField]
    public ClassInfo classInfo = null;

    public CardDragAndDrop cardDragAndDrop = null;

    [SerializeField]
    private int attackPower = 99;
    [SerializeField]
    private int vitality = 99;
    private int vitalityLimit;
    private int initialVitality;

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

    [SerializeField]
    private GameObject DamageTextPrototype = null;

    private bool freezing = false;

    private GameObject freezingEffect = null;

    [SerializeField]
    private Sprite horizontalSprite = null;

    [SerializeField]
    private RectTransform skillsHorizontalSpot = null;
    [SerializeField]
    private RectTransform attackPowerHorizontalSpot = null;
    [SerializeField]
    private RectTransform vitalityHorizontalSpot = null;

    [SerializeField]
    private Shakeable shakeable = null;

    [SerializeField]
    private float fadingDurationOnDeath = 0.5f;

    [SerializeField]
    private Color normalVitalityColor = Color.green;
    private Color overhealedColor = Color.yellow;

    private static DeathCounter deathCounter = new DeathCounter(13);
    #endregion

    #region Properties
    public bool Freezing { get => freezing; }
    public int Vitality { get => vitality; }
    public int AttackPower { get => attackPower; set => attackPower = value; }
    public Classes Classe { get => classInfo.Classe; }
    public bool IgnoreOpponentsBlock { get => skills.IgnoreOpponentsBlock; }
    public OldSkill Skills {
        get => skills;
        set {
            skills = value;
            skillText.text = skills.FullName;
            skillText.color = classInfo.Color;
        }
    }
    #endregion

    #region Initialization
    private void Awake()
    {
        classInfo.TryToRegisterCardInClass(this);

        attackPowerText.text = AttackPower.ToString();

        // Triggers update in skill text
        Skills = skills;

        SetInitialAndLimitVitality();
    }
    private void Start()
    {
        if (skills == null)
        {
            skills = skillsMediator.GetBasicAttackSkill();
        }

        overhealedColor = ClassInfo.GetColorOfClass(Classes.CLERIC);
        SetVitalityAndUpdateTextLooks(Vitality);
    }
    private void SetInitialAndLimitVitality()
    {
        initialVitality = vitality;
        vitalityLimit = vitality + vitality;
    }
    #endregion

    public void AttackSelectedCard(Battlefield opponentBattlefield, Battlefield attackerBattlefield)
    {
        Skills.ApplyEffectsConsideringSelectedTarget(opponentBattlefield, attackerBattlefield);
    }

    #region Damage and Healing
    public void TakeDamageAndManageCardState(int damage, Battlefield battlefieldToRemoveCardInCaseOfDeath)
    {
        // Actions conditioned to damage amount
        { 
            if (damage > 0)
            {
                CreateDamageAnimatedText(damage);
                shakeable.Shake();
            }
            else if (damage < 0)
            {
                L.ogError(this, "[Card] tryed to apply negative damage. That's wrong! Use Heal method instead");
            }
        }

        // Actions conditioned to the Vitality
        { 
            int newVit = SetVitalityAndUpdateTextLooks(Vitality - damage);
            if (newVit <= 0)
            {
                RemoveFreezing();
                battlefieldToRemoveCardInCaseOfDeath.Remove(this);
                StartCoroutine(DieWithAnimation());
                deathCounter.RegisterDeath();
            }
            else
            {
                deathCounter.RegisterSurvived();
            }
        }
    }
    private void CreateDamageAnimatedText(int damage)
    {
        RectTransform damageTextTransform = Instantiate(DamageTextPrototype).GetComponent<RectTransform>();

        damageTextTransform.SetParent(UIBattle.parentOfDynamicUIThatMustAppear, false);
        damageTextTransform.position = cardImage.transform.position;
        damageTextTransform.Rotate(new UnityEngine.Vector3(0, 0, 90));

        damageTextTransform.GetComponent<Text>().text = damage.ToString();

        damageTextTransform.gameObject.SetActive(true);
    }

    private int SetVitalityAndUpdateTextLooks(int value)
    {   
        vitality = value;
        UpdateVitalityTextAndItsColor();
        return vitality;
    }
    private void UpdateVitalityTextAndItsColor()
    {
        vitalityText.text = vitality.ToString();

        if (vitality <= initialVitality)
        {
            vitalityText.color = normalVitalityColor;
        }
        else
        {
            vitalityText.color = overhealedColor;
        }
    }

    private IEnumerator DieWithAnimation()
    {
        Fader fader = new Fader();
        yield return fader.DieWithAnimation(
                fadingDuration: fadingDurationOnDeath,
                allChildrenImages: GetComponentsInChildren<Image>(),
                allChildrenTexts: GetComponentsInChildren<Text>()
            );

        Destroy(gameObject);
    }

    public bool CanBeHealed()
    {
        return vitality < vitalityLimit;
    }
    public void HealNotExceedingDoubleVitalityLimit(int healAmount)
    {
        int vit = Mathf.Min(vitalityLimit, vitality + healAmount);

        SetVitalityAndUpdateTextLooks(vit);
    }
    public void InconditionalHealing(int healAmount)
    {
        SetVitalityAndUpdateTextLooks(Vitality + healAmount);
    }
    #endregion

    #region Has XXXX Skill
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

    #region VFX and Methods to Change Card Visually
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
        skills.PlayDefenseSFX();
    }
    public void ShowDefenseVFX(float attackerYPosition)
    {
        UnityEngine.Vector3 forwards = new UnityEngine.Vector3(0, 0, -transform.position.y);
        UnityEngine.Vector3 upwards = new UnityEngine.Vector3(0, 0, -1);
        UnityEngine.Quaternion lookRotation = UnityEngine.Quaternion.LookRotation(forwards, upwards);
        GameObject vfx = Instantiate(skills.DefenseVFX, transform.position, UnityEngine.Quaternion.identity);
        vfx.GetComponent<RectTransform>().SetParent(transform, false);
        vfx.GetComponent<RectTransform>().localPosition = UnityEngine.Vector3.zero;

        float y = attackerYPosition - transform.position.y;

        if (y > 0)
        {
            vfx.transform.eulerAngles = new UnityEngine.Vector3(0, 0, 0);
        }
        else
        {
            vfx.transform.eulerAngles = new UnityEngine.Vector3(0, 0, 180);
        }
    }
    public void ChangeToHorizontalVersion()
    {
        cardImage.sprite = horizontalSprite;

        attackPowerText.transform.Rotate(new UnityEngine.Vector3(0,0,-90));
        vitalityText.transform.Rotate(new UnityEngine.Vector3(0, 0, -90));
        skillText.transform.parent.Rotate(new UnityEngine.Vector3(0, 0, -90));

        vitalityText.transform.parent.position = vitalityHorizontalSpot.position;
        attackPowerText.transform.parent.position = attackPowerHorizontalSpot.position;
        skillText.transform.parent.position = skillsHorizontalSpot.position;
    }
    public void MakeColorGray()
    {
        const float GRAY_INTENSITY = 0.5f;
        const float ALPHA = 1.0f;
        cardImage.color = new Color(GRAY_INTENSITY, GRAY_INTENSITY, GRAY_INTENSITY, ALPHA);
    }
    public void MakeColorDefault()
    {
        cardImage.color = Color.white;
    }
    #endregion


    #region Get Some Sprite Or Text
    public Sprite GetCardSprite()
    {
        return cardImage.sprite;
    }
    public string GetSkillFullName()
    {
        return skills.FullName;
    }
    public string GetColoredTitleForTooltip()
    {
        return "<color=#"+ classInfo.ColorHexCode + ">"+ Classe+"</color>";
    }
    public string GetExplanatoryText()
    {
        return GetSkillsExplanatoryText() + "\n" +
               "<color=#FD7878>Attack Power: " + attackPower + "</color>\n" +
               "<color=#9EFA9D>Vitality: " + vitality + "</color>\n";
    }
    public string GetSkillsExplanatoryText()
    {
        return skills.GetExplanatoryText(attackPower);
    }
    #endregion

    public void ApplyFreezing(GameObject freezingEffect)
    {
        RemoveFreezing();
        freezing = true;
        this.freezingEffect = freezingEffect;
        ChildMaker.AdoptAndTeleport(transform, freezingEffect.GetComponent<RectTransform>());
        freezingEffect.transform.localScale = UnityEngine.Vector3.one;
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

    public void ApplyPlayerBonuses()
    {
        attackPower += classInfo.AttackPowerBonus;
        attackPowerText.text = attackPower.ToString();

        vitality += classInfo.VitalityBonus;
        SetInitialAndLimitVitality();
        UpdateVitalityTextAndItsColor();
    }
    public void BuffAttackPowerForThisMatch()
    {
        attackPower++;
        attackPowerText.text = attackPower.ToString();
    }

    public bool IsAnotherInstanceOf(Card card)
    {
        return card.Skills == skills;
    }
    public Card GetClone()
    {
        return Instantiate(this);
    }

    public static void ResetDeathCount()
    {
        deathCounter.ResetDeathCount();
    }
    public static int GetDeathCount()
    {
        return deathCounter.GetDeathCount();
    }
}