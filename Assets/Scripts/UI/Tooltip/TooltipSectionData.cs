using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TooltipSectionData
{
    public string message;
    public Sprite background;
    public float height;

    public TooltipSectionData(string message, Sprite background, float height)
    {
        this.message = message;
        this.background = background;
        this.height = height;
    }
}