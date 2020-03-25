using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateLogger : MonoBehaviour
{
    private static int logCounter = 0;

    void Update()
    {
        logCounter++;
        if (logCounter > 60)
        {
            logCounter = 0;
        }
    }

    public static void Log(string message)
    {
        if (logCounter == 60)
        {
            Debug.Log(message);
        }
    }
}
