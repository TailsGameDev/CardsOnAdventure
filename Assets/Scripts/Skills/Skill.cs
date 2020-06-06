using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : SkillsMediatorUser
{
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
    protected GameObject spawnAboveItselfWhenAttackVFX = null;

    [SerializeField]
    protected AudioClip attackSFX = null;

    [SerializeField]
    protected AudioClip defenseSFX = null;

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
    public string GetExplanatoryText(int attackPower)
    {
        return "["+ FullName + "] " + Description
            .Replace("<half>Attack Power", "<color=#FD7878>Attack Power (" + attackPower / 2 + ")</color>")
            .Replace("<full>Attack Power", "<color=#FD7878>Attack Power (" + attackPower + ")</color>");
    }

    public bool IsAnotherInstanceOf(Skill skill)
    {
        return skill.fullName.Equals( fullName ) && skill.description.Equals( description );
    }
}
