using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TipSectionData
{
    public string message;
    public Sprite background;
    public float height;
    private const float DEFAULT_HEIGHT = 972.0f;

    public TipSectionData(string message)
    {
        this.message = message;
        this.background = null;
        this.height = DEFAULT_HEIGHT;
    }

    public TipSectionData(Sprite background)
    {
        this.message = "";
        this.background = background;
        this.height = DEFAULT_HEIGHT;
    }

    public TipSectionData(Sprite background, float height)
    {
        this.message = "";
        this.background = background;
        this.height = height;
    }

    public TipSectionData(string message, float height)
    {
        this.message = message;
        this.background = null;
        this.height = height;
    }

    public TipSectionData(string message, Sprite background, float height)
    {
        this.message = message;
        this.background = background;
        this.height = height;
    }
}