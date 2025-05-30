﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L
{
    public static void og(object obj, string message)
    {
        og(message, obj);
    }

    public static void og(string message, object obj)
    {
        MonoBehaviour o = obj as MonoBehaviour;
        if (o != null)
        {
            Debug.Log("[" + obj.GetType() + "]" + message, (MonoBehaviour)obj);
        }
        else
        {
            Debug.Log("[" + obj.GetType() + "]" + message);
        }
    }

    public static void ogWarning(object obj, string message)
    {
        ogWarning(message,obj);
    }

    public static void ogWarning(string message, object obj)
    {
        if (obj.GetType().IsAssignableFrom(typeof(MonoBehaviour)))
        {
            Debug.LogWarning("[" + obj.GetType() + "]" + message, (MonoBehaviour)obj);
        }
        else
        {
            Debug.LogWarning("[" + obj.GetType() + "]" + message);
        }
    }

    public static void ogError(object obj, string message)
    {
        ogError(message, obj);
    }
    public static void ogError(string message, object obj)
    {
        if (obj.GetType().IsAssignableFrom(typeof(MonoBehaviour)))
        {
            Debug.LogError("[" + obj.GetType() + "]" + message, (MonoBehaviour)obj);
        }
        else
        {
            Debug.LogError("[" + obj.GetType() + "]" + message);
        }
    }

    private string GetTypeInBrackets(object obj)
    {
        return "[" + obj.GetType() + "]";
    }

    public static void ogArrayInt(object obj, int[] a)
    {
        string s = "";
        int sum = 0;
        for (int i = 0; i < a.Length; i++)
        {
            s += a[i] + "; ";
            sum += a[i];
        }

        L.og(obj, "L.ogArrayInt. indexes: " + s + "; sum: " + sum);
    }
}
