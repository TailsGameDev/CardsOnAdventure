using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassInfo : MonoBehaviour
{
    [SerializeField]
    protected string colorHexCode;

    public string ColorHexCode { get => colorHexCode; }
}
