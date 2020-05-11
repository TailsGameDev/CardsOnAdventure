using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField]
    private Slider aiDelaySlider = null;

    [SerializeField]
    private Toggle fullscreenToggle = null;

    [SerializeField]
    private Toggle customCursorToggle = null;
    [SerializeField]
    private Texture2D customCursor = null;

    [SerializeField]
    private Toggle shutSpiritsMouth = null;

    private const string AI_DELAY_KEY = "AI_DELAY_KEY";

    public static readonly int TRUE = 1;
    public static readonly int FALSE = 0;

    private const string SHUT_SPIRIT_MOUTH_KEY = "ShutSpiritsMouth";

    private static bool shouldDisplayTipOnEnable = true;

    private void Awake()
    {
        // EnemyAI class has a default value for the delay: 0.5f
        if (PlayerPrefs.HasKey(AI_DELAY_KEY))
        {
            aiDelaySlider.value = PlayerPrefs.GetFloat(AI_DELAY_KEY);
        }

        shutSpiritsMouth.isOn = IsTrue(PlayerPrefs.GetInt(SHUT_SPIRIT_MOUTH_KEY, FALSE));

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

    private void OnEnable()
    {
        if (shouldDisplayTipOnEnable)
        {
            TipDragAndDrop.AskToUseTips();
            shouldDisplayTipOnEnable = false;
        }
    }

    private static bool IsTrue(int c)
    {
        return c == TRUE;
    }

    private static bool IsFalse(int c)
    {
        return c == FALSE;
    }

    private int ToInt(bool b)
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

    public void ToggleSpiritsMouthShut()
    {
        PlayerPrefs.SetInt(SHUT_SPIRIT_MOUTH_KEY, ToInt(shutSpiritsMouth.isOn) );
    }

    public void ToggleFullscreenMode()
    {
        Screen.fullScreen = fullscreenToggle.isOn;

        if (fullscreenToggle.isOn)
        {
            Screen.SetResolution(576, 1024, true);
        }

        PlayerPrefs.SetInt("Fullscreen", ToInt( fullscreenToggle.isOn ) );
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
        PlayerPrefs.SetInt("CustomCursor", ToInt(customCursorToggle.isOn));
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

    public static bool ShouldAskForTips()
    {
        return IsFalse(PlayerPrefs.GetInt(SHUT_SPIRIT_MOUTH_KEY, defaultValue: FALSE));
    }
}
