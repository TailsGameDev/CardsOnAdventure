using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentHandlers : MonoBehaviour
{
    private PersistentHandlers instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
