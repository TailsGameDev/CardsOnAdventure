using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [SerializeField]
    private string acronym = "";

    [SerializeField]
    private string fullName = "";

    [SerializeField]
    protected GameObject attackVFX = null;

    [SerializeField]
    protected GameObject buffVFX = null;

    [SerializeField]
    private GameObject defenseVFX = null;

    public string Acronym { get => acronym; }

    protected bool canMakeDoubleAttack = false;

    public bool CanMakeDoubleAttack { set => canMakeDoubleAttack = value; }

    public string FullName { get => fullName; }
    public GameObject DefenseVFX { get => defenseVFX; }

    public abstract void ApplyEffectsConsideringSelectedTarget
    (
        Battlefield opponentBattlefield,
        Battlefield attackerBattlefield
    );

    public abstract bool HasBlockEffect();

    public abstract bool HasHeavyArmorEffect();

    public abstract bool HasReflectEffect();
}
