using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadCanvas : MonoBehaviour
{
    static DontDestroyOnLoadCanvas instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }
}
