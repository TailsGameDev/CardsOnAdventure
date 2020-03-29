using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyItselfInTime : MonoBehaviour
{
    [SerializeField]
    private float timeToDestruction = 2.0f;

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
