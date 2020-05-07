using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    [SerializeField]
    private Slider aiDelaySlider = null;

    [SerializeField]
    private Toggle fullscreenToggle = null;

    [SerializeField]
    private Toggle customCursorToggle = null;
    [SerializeField]
    private Texture2D customCursor = null;

    private const string AI_DELAY_KEY = "AI_DELAY_KEY";

    private const int TRUE = 1;
    private const int FALSE = 0;

    private void Awake()
    {
        // EnemyAI class has a default value for the delay: 0.5f
        if (PlayerPrefs.HasKey(AI_DELAY_KEY))
        {
            aiDelaySlider.value = PlayerPrefs.GetFloat(AI_DELAY_KEY);
        }

        fullscreenToggle.isOn = IsTrue( PlayerPrefs.GetInt("Fullscreen", FALSE) ) ;

        customCursorToggle.isOn = IsTrue(PlayerPrefs.GetInt("CustomCursor", FALSE));
        if (customCursorToggle.isOn)
        {
            Cursor.SetCursor(customCursor, Vector2.zero, CursorMode.Auto);
        }

#if UNITY_ANDROID
        fullscreenToggle.gameObject.SetActive(false);
        customCursorToggle.gameObject.SetActive(false);
#endif
    }

    private static bool IsTrue(int c)
    {
        return c == 1;
    }

    private int BoolToInt(bool b)
    {
        if (b)
        {
            return TRUE;
        }
        else
        {
            return FALSE;
        }
    }

    public void OnAIDelaySliderValueChanged()
    {
        EnemyAI.AIDelay = aiDelaySlider.value;
        PlayerPrefs.SetFloat(AI_DELAY_KEY, aiDelaySlider.value);
    }

    public static float GetAIDelayFromPlayerPrefs()
    {
        return PlayerPrefs.GetFloat(AI_DELAY_KEY);
    }

    public void ToggleFullscreenMode()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
        PlayerPrefs.SetInt("Fullscreen", BoolToInt( fullscreenToggle.isOn ) );
    }

    public void ToggleCustomCursor()
    {
        if (customCursorToggle.isOn)
        {
            Cursor.SetCursor(customCursor, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        PlayerPrefs.SetInt("CustomCursor", BoolToInt(customCursorToggle.isOn));
    }

    public static void RefreshCursor(Texture2D customCursor)
    {
        if (IsTrue(PlayerPrefs.GetInt("CustomCursor", FALSE)))
        {
            Cursor.SetCursor(customCursor, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
