using UnityEngine;

public class ColorHexCodes
{
    public static string BeginWhite = "<color=#FFFFFF>";
    public static string BeginLightGreen = "<color=#9EFA9D>";
    public static string BeginLightRed = "<color=#FD7878>";
    public static string End = "</color>";

    public static string Paint(string message, Color color)
    {
        return "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + message + "</color>";

    }

    public static string GetHexCode(Color color)
    {
        return ColorUtility.ToHtmlStringRGB(color);
    }
}
