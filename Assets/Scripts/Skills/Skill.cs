using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : SkillsMediatorUser
{
    [SerializeField]
    private string acronym = "";

    [SerializeField]
    private string fullName = "";

    [SerializeField]
    private string description = "";

    [SerializeField]
    protected GameObject attackVFX = null;

    [SerializeField]
    private GameObject defenseVFX = null;

    [SerializeField]
    protected GameObject specialVFX = null;

    [SerializeField]
    protected AudioClip attackSFX = null;

    [SerializeField]
    protected AudioClip defenseSFX = null;

    public string Acronym { get => acronym; }
    public string FullName { get => fullName; }
    public GameObject DefenseVFX { get => defenseVFX; }
    public string Description { get => description; }

    public abstract void ApplyEffectsConsideringSelectedTarget
    (
        Battlefield opponentBattlefield,
        Battlefield attackerBattlefield
    );

    public abstract bool HasBlockEffect();

    public abstract bool HasHeavyArmorEffect();

    public abstract bool HasReflectEffect();

    public void PlayDefenseSFX()
    {
        skillsMediator.PlaySFX(defenseSFX);
    }
    public string GetExplanatoryText()
    {
        return "-[" + Acronym + "] " + FullName + ": " + Description;
    }
}
