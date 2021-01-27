using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class Card : SkillsMediatorUser
{
    #region Attributes
    // Card general attributes
    [SerializeField]
    private string nickname = null;

    [SerializeField]
    public ClassInfo classInfo = null;

    [SerializeField]
    private OldSkill skills = null;
    private bool freezing = false;
    private GameObject freezingEffect = null;

    [SerializeField]
    private int initialAmountOnCollection = 0;

    [SerializeField]
    private TipReceptor tipReceptor = null;

    public CardDragAndDrop cardDragAndDrop = null;
    
    private delegate void OnLevelUp();
    private static OnLevelUp onLevelUp;

    private RectTransform rectTransform;

    // Visual Related Attributes
    [SerializeField]
    private Text[] skillTexts = null;
    [SerializeField]
    private GameObject[] ignoreProtectionIcons = null;
    [SerializeField]
    private GameObject protectionIcon = null;
    [SerializeField]
    private Image obfuscator = null;
    [SerializeField]
    private GameObject lvlUpVFX = null;
    [SerializeField]
    private Image cardImage = null;
    [SerializeField]
    private GameObject DamageTextPrototype = null;
    [SerializeField]
    private Sprite horizontalSprite = null;
    private Sprite verticalSprite = null;
    [SerializeField]
    private Shakeable shakeable = null;
    [SerializeField]
    private float fadingDurationOnDeath = 0.5f;
    [SerializeField]
    private Color normalVitalityColor = Color.green;
    private Color overhealedColor = Color.yellow;
    private float increaseScaleValueInProtectionAnimation = 0.2f;
    private float increaseScaleSpeedMultiplier = 1.0f;

    // Attack and Vitality related Attributes
    [SerializeField]
    private Text[] vitalityTexts = null;
    [SerializeField]
    private Text[] attackPowerTexts = null;
    [SerializeField]
    private CardsLevel cardsLevel = null;
    [SerializeField]
    private int originalAttackPower = 0;
    private int attackPower;
    [SerializeField]
    private int originalVitality = 0;
    private int vitality;
    private int vitalityLimit;
    private int vitalityAtStartOfBattle;
    [SerializeField]
    private float attackBonusPerLevel = 0;
    [SerializeField]
    private float vitalityBonusPerLevel = 0;
    #endregion

    #region Properties
    public bool Freezing { get => freezing; }
    public int Vitality { get => vitality; }
    public int AttackPower { get => attackPower; set => attackPower = value; }
    public Classes Classe { get => classInfo.Classe; }
    public bool IgnoreOpponentsBlock { get => skills.IgnoreProtection; }
    public OldSkill Skill {
        get => skills;
        set 
        {
            skills = value;
            SetTextArray(skillTexts, skills.FullName);
            SetTextArrayColor(skillTexts, classInfo.Color);
            for (int i = 0; i < ignoreProtectionIcons.Length; i++)
            {
                bool appearInThisAlignment = skillTexts[i].transform.parent.gameObject.activeSelf;
                ignoreProtectionIcons[i].SetActive(appearInThisAlignment && value.IgnoreProtection);
            }
        }
    }
    public RectTransform RectTransform { get => rectTransform; }
    public int InitialAmountOnCollection { get => initialAmountOnCollection; }
    #endregion

    #region Initialization
    private void Awake()
    {
        // General
        classInfo.TryToRegisterCardInClass(this);
        onLevelUp += RefreshStatsForThePlayer;
        // Triggers update in skill text
        Skill = skills;
        rectTransform = GetComponent<RectTransform>();

        // Atk Vit
        attackPower = originalAttackPower;
        vitality = originalVitality;
        SetInitialAndLimitVitality();
        SetTextArray(attackPowerTexts, attackPower.ToString());

        // Visual
        verticalSprite = cardImage.sprite;
    }
    private void Start()
    {
        // General
        if (skills == null)
        {
            skills = skillsMediator.GetBasicAttackSkill();
        }

        // Atk and Vit related
        overhealedColor = ClassInfo.GetColorOfClass(Classes.CLERIC);
        SetVitalityAndUpdateTextLooks(Vitality);
    }
    #endregion
    private void OnDestroy()
    {
        onLevelUp -= RefreshStatsForThePlayer;
    }

    public void AttackSelectedCard(Battlefield opponentBattlefield, Battlefield attackerBattlefield)
    {
        Skill.ApplyEffectsConsideringSelectedTarget(opponentBattlefield, attackerBattlefield);
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
                DeathCounter.RegisterDeath();
            }
            else
            {
                DeathCounter.RegisterSurvived();
            }
        }
    }
    
    private void CreateDamageAnimatedText(int damage)
    {
        RectTransform damageTextTransform = Instantiate(DamageTextPrototype).GetComponent<RectTransform>();

        damageTextTransform.SetParent(UIBattle.parentOfDynamicUIThatMustAppear, false);
        damageTextTransform.position = cardImage.transform.position;
        damageTextTransform.Rotate(new UnityEngine.Vector3(0, 0, 90));

        damageTextTransform.GetComponentInChildren<Text>().text = damage.ToString();

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
        SetTextArray(vitalityTexts, vitality.ToString());

        if (vitality <= vitalityAtStartOfBattle)
        {
            SetTextArrayColor(vitalityTexts, normalVitalityColor);
        }
        else
        {
            SetTextArrayColor(vitalityTexts, overhealedColor);
        }
    }
    private void SetTextArray(Text[] array, string message)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i].text = message;
        }
    }
    private void SetTextArrayColor(Text[] array, Color color)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i].color = color;
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
        return Skill.HasBlockEffect();
    }
    public bool HasHeavyArmorSkill()
    {
        return Skill.HasHeavyArmorEffect();
    }
    public bool HasReflectSkill()
    {
        return Skill.HasReflectEffect();
    }
    #endregion

    #region VFX and Methods to Change Card Visually
    public bool CanAttack()
    {
        return !obfuscator.gameObject.activeSelf && !freezing;
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
        skills.PlayDefenseSFX();
    }
    public void ShowDefenseVFX(float attackerYPosition)
    {
        UnityEngine.Vector3 forwards = new UnityEngine.Vector3(0, 0, -rectTransform.position.y);
        UnityEngine.Vector3 upwards = new UnityEngine.Vector3(0, 0, -1);
        // UnityEngine.Quaternion lookRotation = UnityEngine.Quaternion.LookRotation(forwards, upwards);
        GameObject vfx = Instantiate(skills.DefenseVFX, rectTransform.position, UnityEngine.Quaternion.identity);
        vfx.GetComponent<RectTransform>().SetParent(rectTransform, false);
        vfx.GetComponent<RectTransform>().localPosition = UnityEngine.Vector3.zero;

        float y = attackerYPosition - rectTransform.position.y;

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
        attackPowerTexts[0].transform.parent.gameObject.SetActive(false);
        attackPowerTexts[1].transform.parent.gameObject.SetActive(true);

        vitalityTexts[0].transform.parent.gameObject.SetActive(false);
        vitalityTexts[1].transform.parent.gameObject.SetActive(true);

        skillTexts[0].transform.parent.gameObject.SetActive(false);
        skillTexts[1].transform.parent.gameObject.SetActive(true);

        if (skills.IgnoreProtection)
        {
            ignoreProtectionIcons[0].SetActive(false);
            ignoreProtectionIcons[1].SetActive(true);
        }

        cardImage.sprite = horizontalSprite;
    }
    public void ChangeToVerticalVersion()
    {
        attackPowerTexts[1].transform.parent.gameObject.SetActive(false);
        attackPowerTexts[0].transform.parent.gameObject.SetActive(true);

        vitalityTexts[1].transform.parent.gameObject.SetActive(false);
        vitalityTexts[0].transform.parent.gameObject.SetActive(true);

        skillTexts[1].transform.parent.gameObject.SetActive(false);
        skillTexts[0].transform.parent.gameObject.SetActive(true);

        if (skills.IgnoreProtection)
        {
            ignoreProtectionIcons[0].SetActive(true);
            ignoreProtectionIcons[1].SetActive(false);
        }

        cardImage.sprite = verticalSprite;
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
    public void SetProtectionIconActive(bool active)
    {
        protectionIcon.SetActive(active);
    }
    public void MakeProtectionEvident()
    {
        StartCoroutine(MakeProtectionEvident(protectionIcon.transform));
    }
    private IEnumerator MakeProtectionEvident(Transform toScale)
    {
        Transform protection = toScale;
        UnityEngine.Vector3 originalScale = protection.localScale;
        UnityEngine.Vector3 targetScale = originalScale + new UnityEngine.Vector3(increaseScaleValueInProtectionAnimation,
                                                        increaseScaleValueInProtectionAnimation, 0.0f);
        while (protection.localScale.x < targetScale.x)
        {
            float t = TimeFacade.DeltaTime * increaseScaleSpeedMultiplier;
            protection.localScale += new UnityEngine.Vector3(t, t, t);
            yield return null;
        }

        while (protection.localScale.x > originalScale.x)
        {
            float t = TimeFacade.DeltaTime * increaseScaleSpeedMultiplier;
            protection.localScale -= new UnityEngine.Vector3(t, t, t);
            yield return null;
        }
    }
    #endregion

    #region Get Some Sprite Or Text
    public Sprite GetCardVerticalSprite()
    {
        return verticalSprite;
    }
    public string GetColoredTitleForTip()
    {
        return ColorHexCodes.Paint(nickname, classInfo.Color);
    }

    public string GetExplanatoryText()
    {
        return 
                "Class: "+ ColorHexCodes.Paint(classInfo.name.ToUpperInvariant(), classInfo.Color)+
                "\nSkill: "+(GetSkillsExplanatoryText()) + "\n" +
                ColorHexCodes.BeginLightRed+"Attack Power: " + attackPower + ColorHexCodes.End+"\n" +
                ColorHexCodes.BeginLightGreen+"Vitality: " + vitality + ColorHexCodes.End+"\n"
                + ColorHexCodes.BeginWhite+"Level: "+ (CalculateLevelBasedOnCurrentAttackPower()+1) +ColorHexCodes.End
                ;
    }
    public string GetSkillsExplanatoryText()
    {
        return ColorHexCodes.Paint(skills.GetExplanatoryText(attackPower).ToUpper(), classInfo.Color);
    }
    #endregion

    #region Configure Attack and Vitality at the beggining of the battle
    public virtual void RefreshStatsForThePlayer()
    {
        attackPower = originalAttackPower;
        vitality = originalVitality;

        int level = GetLevel();
        SumLevelBonus(level);

        // AttackPowerBonus and VitalityBonus holds currently unnused buff to the cards of the class.
        attackPower += classInfo.AttackPowerBonus;
        vitality += classInfo.VitalityBonus;

        RefreshUIAndVitalityState();
    }
    private void RefreshUIAndVitalityState()
    {
        SetTextArray(attackPowerTexts, attackPower.ToString());
        SetInitialAndLimitVitality();
        UpdateVitalityTextAndItsColor();
    }
    private void SetInitialAndLimitVitality()
    {
        vitalityAtStartOfBattle = vitality;
        vitalityLimit = vitality + vitality;
    }
    public void SumLevelBonus(int level)
    {
        attackPower += (int)(level * attackBonusPerLevel);
        vitality += (int)(level * vitalityBonusPerLevel);

        RefreshUIAndVitalityState();
    }
    public void ApplyEditorMadeDeckBuffAndRefreshUI(int atkBuff, int vitBuff, int levelBuff)
    {
        // This method is currently used just by Enemy decks
        attackPower = originalAttackPower;
        vitality = originalVitality;

        attackPower += (int)(levelBuff * attackBonusPerLevel);
        vitality += (int)(levelBuff * vitalityBonusPerLevel);

        attackPower += atkBuff;
        vitality += vitBuff;

        RefreshUIAndVitalityState();
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

    public void ModifyAttackPowerForThisMatch(int valueToSum)
    {
        attackPower += valueToSum;
        SetTextArray(attackPowerTexts, attackPower.ToString());
    }

    public bool IsLocked()
    {
        return CardsCollection.IsCardLocked(this);
    }
    public bool IsAnotherInstanceOf(Card card)
    {
        return card.Skill == skills;
    }
    public Card GetClone()
    {
        return Instantiate(this);
    }

    public RectTransform GetRectTransform()
    {
        return GetComponent<RectTransform>();
    }
    public CardDragAndDrop GetCardDragAndDrop()
    {
        return GetComponent<CardDragAndDrop>();
    }

    public void OpenTip(TipPopUpOpener tipPopUpOpener)
    {
        tipReceptor.OpenTip(tipPopUpOpener);
    }

    public int GetLevel()
    {
        int level = cardsLevel.GetLevelOfCard(this);
        return level;
    }
    public void LevelUp()
    {
        cardsLevel.LevelUpCard(this);
        onLevelUp?.Invoke();

        GameObject vfx = Instantiate(lvlUpVFX);
        vfx.transform.position = transform.position;
        vfx.SetActive(gameObject.activeSelf);
        vfx.transform.SetParent(transform);
    }
    public int CalculateLevelBasedOnCurrentAttackPower()
    {
        float aux = attackPower;
        aux -= originalAttackPower;
        float level = aux / attackBonusPerLevel;
        return (int)level;
    }
}