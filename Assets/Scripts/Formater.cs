using UnityEngine;

public class Formater
{
    public static string Paint(string message, string colorHexCode)
    {
        return "<color=#" + colorHexCode + ">" + message + "</color>";
    }

    public static string Paint(string message, Color color)
    {
        return "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + message + "</color>";
        
    }
}
