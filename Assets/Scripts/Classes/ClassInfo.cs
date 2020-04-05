using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassInfo : MonoBehaviour
{
    public static List<ClassInfo> classesInfo = new List<ClassInfo>();

    [SerializeField]
    Classes classe = Classes.NOT_A_CLASS;

    [SerializeField]
    protected string colorHexCode;

    [SerializeField]
    private Card[] cards = null;

    public string ColorHexCode { get => colorHexCode; }
    public Card[] Cards { get => cards; }

    private void Awake()
    {
        bool myCloneIsAlreadyInTheList = false;
        for (int i = 0; i < classesInfo.Count; i++)
        {
            if (classesInfo[i].classe == classe)
            {
                myCloneIsAlreadyInTheList = true;
                break;
            }
        }

        if (!myCloneIsAlreadyInTheList)
        {
            classesInfo.Add(this);
        }
    }

    public static ClassInfo GetInfoOfClass(Classes classe)
    {
        ClassInfo info = classesInfo[0];

        for (int i = 1; i < classesInfo.Count; i++)
        {
            if (classesInfo[i].classe == classe)
            {
                info = classesInfo[i];
            }
        }

        return info;
    }
}
