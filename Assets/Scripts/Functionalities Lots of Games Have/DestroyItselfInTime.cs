using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyItselfInTime : MonoBehaviour
{
    [SerializeField]
    private float timeToDestruction = 5.0f;

    [SerializeField]
    private bool destroyIfDisabled = false;

    public void SetDestructionTime(float destructionTime)
    {
        timeToDestruction = destructionTime;
    }

    void Update()
    {
        if (timeToDestruction > 0)
        {
            timeToDestruction -= TimeFacade.DeltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        if (destroyIfDisabled)
        {
            Destroy(gameObject);
        }
    }
}
