using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class ClassesSerializable
{
    private List<Classes> classes;
    private List<int> attackPowerBonuses;
    private List<int> vitalityBonuses;

    public ClassesSerializable(Dictionary<Classes, ClassInfo> classesInfo)
    {
        classes = classesInfo.Keys.ToList<Classes>();

        attackPowerBonuses = new List<int>();
        vitalityBonuses = new List<int>();

        for (int i = 0; i < classes.Count; i++)
        {
            attackPowerBonuses.Add(classesInfo[classes[i]].AttackPowerBonus);
            vitalityBonuses.Add(classesInfo[classes[i]].VitalityBonus);
        }
    }

    public void SetBonusesInAllClasses(Dictionary<Classes, ClassInfo> classesInfo)
    {
        for (int i = 0; i < classes.Count; i++)
        {
            classesInfo[classes[i]].AttackPowerBonus = attackPowerBonuses[i];
            classesInfo[classes[i]].VitalityBonus = vitalityBonuses[i];
        }
    }
}
