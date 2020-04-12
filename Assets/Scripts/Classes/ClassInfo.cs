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
    private List<Card> cards = new List<Card>();

    private int vitalityBonus = 0;
    private int attackPowerBonus = 0;

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
        }
    }

    // That must be called by cards during Awake
    public void registerCardInClass(Card card)
    {
        if ( ! cards.Contains(card) )
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
        return classesInfo[classe].cards.ToArray();
    }

    public static void GiveVitalityBonusToClass(Classes classe)
    {
        classesInfo[classe].vitalityBonus++;
    }

    public static void GiveAttackPowerBonusToClass(Classes classe)
    {
        classesInfo[classe].attackPowerBonus++;
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
