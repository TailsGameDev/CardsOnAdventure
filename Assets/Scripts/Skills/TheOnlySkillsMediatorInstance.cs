using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheOnlySkillsMediatorInstance : SkillsMediatorUser
{
    [SerializeField]
    private OldSkill basicAttack = null;

    [SerializeField]
    private OldSkill skillNamedStrong = null;

    [SerializeField]
    protected AudioRequisitor audioRequisitor = null;

    [SerializeField]
    private DefenseEffect regularDefenseEffect = null;

    [SerializeField]
    private DefenseEffect blockDefenseEffect = null;

    public DefenseEffect RegularDefenseEffect { get => regularDefenseEffect; }
    public DefenseEffect BlockDefenseEffect { get => blockDefenseEffect; }

    private void Awake()
    {
        if (skillsMediator == null)
        {
            skillsMediator = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public OldSkill GetBasicAttackSkill()
    {
        return basicAttack;
    }

    public bool IsTheSkillNamedStrong (OldSkill skill)
    {
        return skillNamedStrong.IsAnotherInstanceOf(skill);
    }

    public void PlaySFX(AudioClip audioClip)
    {
        audioRequisitor.RequestSFX(audioClip);
    }
}
