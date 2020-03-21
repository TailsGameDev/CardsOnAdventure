using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{

    [SerializeField]
    private bool doubleAttack = false;

    public void DoEffect()
    {


        if (doubleAttack)
        {
            DoEffect();
        } 
    }

}
