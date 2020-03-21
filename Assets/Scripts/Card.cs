using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField]
    private int attackPower = 0;
    [SerializeField]
    private int vitality = 0;

    [SerializeField]
    private List<Skill> skills = null;

    public int GetAttackPower()
    {
        return attackPower;
    }

    public int GetVitality()
    {
        return vitality;
    }
}
