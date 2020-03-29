using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsMadiator : MonoBehaviour
{

    [SerializeField]
    private OldSkill basicAttack = null;

    [SerializeField]
    protected AudioRequisitor audioRequisitor = null;

    public OldSkill GetBasicAttackSkill()
    {
        return basicAttack;
    }

    public void PlaySFX(AudioClip audioClip)
    {
        audioRequisitor.RequestSFX(audioClip);
    }
}
