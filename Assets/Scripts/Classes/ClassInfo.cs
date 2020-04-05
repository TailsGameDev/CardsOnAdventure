using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassInfo : MonoBehaviour
{
    private static Dictionary<Classes, ClassInfo> classesInfo = new Dictionary<Classes, ClassInfo>();

    [SerializeField]
    private Classes classe = Classes.NOT_A_CLASS;

    [SerializeField]
    private string colorHexCode = "code";

    [SerializeField]
    private Color color = Color.white;

    [SerializeField]
    private Card[] cards = null;

    private int vitalityBonus = 0;
    private int attackPowerBonus = 0;

    public string ColorHexCode { get => colorHexCode; }
    public Card[] Cards { get => cards; }
    public int VitalityBonus { get => vitalityBonus; }
    public int AttackPowerBonus { get => attackPowerBonus; }

    private void Awake()
    {
        if (!classesInfo.ContainsKey(classe))
        {
            classesInfo.Add(classe, this);
        }
    }

    public static Color GetColorOfClass(Classes classe)
    {
        return classesInfo[classe].color;
    }

    public static string GetColorHexCodeOfClass(Classes classe)
    {
        return classesInfo[classe].colorHexCode;
    }

    public static Card[] GetCardsOfClass(Classes classe)
    {
        return classesInfo[classe].cards;
    }

    public static void GiveVitalityBonusToClass(Classes classe)
    {
        classesInfo[classe].vitalityBonus++;
    }

    public static void GiveAttackPowerBonusToClass(Classes classe)
    {
        classesInfo[classe].attackPowerBonus++;
    }

    public static int GetVitalityBonusOfClass(Classes classe)
    {
        return classesInfo[classe].vitalityBonus;
    }

    public static int GetAttackPowerBonusOfClass(Classes classe)
    {
        return classesInfo[classe].attackPowerBonus;
    }

    public static void ResetBonusesToAllClasses()
    {
        foreach (Classes key in classesInfo.Keys)
        {
            classesInfo[key].attackPowerBonus = 0;
            classesInfo[key].vitalityBonus = 0;
        }
    }
}
