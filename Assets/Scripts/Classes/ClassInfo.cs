using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassInfo : MonoBehaviour
{
    private static Dictionary<Classes, ClassInfo> classesInfo = new Dictionary<Classes, ClassInfo>();

    [SerializeField]
    private ClassesPersistence classesPersistence = null;

    private static ClassesPersistence staticClassesPersistence = null;

    [SerializeField]
    private Classes classe = Classes.NOT_A_CLASS;

    [SerializeField]
    private string colorHexCode = "code";

    [SerializeField]
    private Color color = Color.white;

    private List<Card> cards = new List<Card>();

    private int vitalityBonus = 0;
    private int attackPowerBonus = 0;

    private bool registersEnabled = true;

    public Classes Classe { get => classe; }
    public string ColorHexCode { get => colorHexCode; }
    public Color Color { get => color; }
    public int AttackPowerBonus { get => attackPowerBonus; set => attackPowerBonus = value; }
    public int VitalityBonus { get => vitalityBonus; set => vitalityBonus = value; }

    private void Awake()
    {
        if (!classesInfo.ContainsKey(classe))
        {
            classesInfo.Add(classe, this);
            // TODO: the next line don't need to be called moe than once
            staticClassesPersistence = classesPersistence;
        }
    }

    private void Start()
    {
        registersEnabled = false;
    }

    // That must be called by cards during Awake
    public void TryToRegisterCardInClass(Card card)
    {
        if ( registersEnabled && ! cards.Contains(card) )
        {
            cards.Add(card);
        }
    }

    public static Color GetColorOfClass(Classes classe)
    {
        return classesInfo[classe].color;
    }

    public static Card[] GetCardsOfClass(Classes classe)
    {
        if (classesInfo[classe].cards == null)
        {
            Debug.LogError("cards of class is null");
        }
        else if (classesInfo[classe].cards.Count == 0)
        {
            Debug.LogError("cards count of class is zero");
        }
        return classesInfo[classe].cards.ToArray();
    }

    public static void GiveVitalityBonusToClassAndSaveInDeviceStorage(Classes classe)
    {
        classesInfo[classe].vitalityBonus++;
        staticClassesPersistence.SaveClasses(new ClassesSerializable(classesInfo));
    }

    public static void GiveAttackPowerBonusToClassAndSaveInDeviceStorage(Classes classe)
    {
        classesInfo[classe].attackPowerBonus++;
        staticClassesPersistence.SaveClasses(new ClassesSerializable(classesInfo));
    }

    public static void ResetBonusesToAllClasses()
    {
        foreach (Classes key in classesInfo.Keys)
        {
            classesInfo[key].attackPowerBonus = 0;
            classesInfo[key].vitalityBonus = 0;
        }
    }

    public static void LoadBonuses(ClassesSerializable classesSerializable)
    {
        classesSerializable.SetBonusesInDictionary(classesInfo);
    }
}
