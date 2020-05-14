using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyItselfInTime : MonoBehaviour
{
    [SerializeField]
    private float timeToDestruction = 5.0f;

    public void SetDestructionTime(float destructionTime)
    {
        timeToDestruction = destructionTime;
    }

    void Update()
    {
        if (timeToDestruction > 0)
        {
            timeToDestruction -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
