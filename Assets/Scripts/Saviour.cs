using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saviour : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        new SaveFacade().SaveEverything();
    }
}
