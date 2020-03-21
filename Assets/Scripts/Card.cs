using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField]
    private int attackPower;
    [SerializeField]
    private int vitality;

    [SerializeField]
    private List<Skill> skills = null;
}
