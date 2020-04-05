using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Its just for not destroying it's children, ant not permit duplicates of them
public class CardManagers : MonoBehaviour
{
    private CardManagers instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
