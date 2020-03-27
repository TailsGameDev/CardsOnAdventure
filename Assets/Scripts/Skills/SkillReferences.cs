using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillReferences : MonoBehaviour
{
    private static SkillReferences instance;

    [SerializeField]
    private OldSkill basicAttack = null;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    public static OldSkill GetBasicAttackSkill()
    {
        return instance.basicAttack;
    }
}
