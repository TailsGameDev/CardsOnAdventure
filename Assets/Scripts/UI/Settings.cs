using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField]
    private Slider aiDelaySlider = null;
    [SerializeField]
    private Text aiDelayCurrentValue = null;

    [SerializeField]
    private Toggle fullscreenToggle = null;

    [SerializeField]
    private Toggle customCursorToggle = null;
    [SerializeField]
    private Texture2D customCursor = null;

    [SerializeField]
    private Toggle shutSpiritsMouth = null;

    private const string AI_DELAY_KEY = "AI_DELAY_KEY";
    private const string CUSTOM_CURSOR_KEY = "CUSTOM_CURSOR"; 

    public static readonly int TRUE = 1;
    public static readonly int FALSE = 0;

    private const string SHUT_SPIRIT_MOUTH_KEY = "ShutSpiritsMouth";
    
    private const string FULLSCREEN_KEY = "FULLSCREEN";
    private const int RESOLUTION_WIDTH = 360;
    private const int RESOLUTION_HEIGHT = 640;

    private static bool shouldDisplayTipOnEnable = true;

    private void Awake()
    {
        // EnemyAI class has a default value for the delay: 0.5f
        if (PlayerPrefs.HasKey(AI_DELAY_KEY))
        {
            aiDelaySlider.value = PlayerPrefs.GetFloat(AI_DELAY_KEY);
        }

        shutSpiritsMouth.isOn = IsTrue(PlayerPrefs.GetInt(SHUT_SPIRIT_MOUTH_KEY, FALSE));

        fullscreenToggle.isOn = IsTrue( PlayerPrefs.GetInt(FULLSCREEN_KEY, FALSE) ) ;

        customCursorToggle.isOn = IsTrue(PlayerPrefs.GetInt(CUSTOM_CURSOR_KEY, FALSE));
        if (customCursorToggle.isOn)
        {
            Cursor.SetCursor(customCursor, Vector2.zero, CursorMode.Auto);
        }

#if UNITY_ANDROID
        fullscreenToggle.gameObject.SetActive(false);
        customCursorToggle.gameObject.SetActive(false);
#endif
    }

    public static void InitializeFullScreen()
    {
        #if !UNITY_WEBGL
        Screen.SetResolution(RESOLUTION_WIDTH, RESOLUTION_HEIGHT, Settings.IsTrue(PlayerPrefs.GetInt(FULLSCREEN_KEY, Settings.FALSE)));
        #endif
    }

    private void OnEnable()
    {
        if (shouldDisplayTipOnEnable)
        {
            TipDragAndDrop.AskToUseTips();
            shouldDisplayTipOnEnable = false;
        }

        StartCoroutine(OnAFrameAfterEnabled());
    }

    private IEnumerator OnAFrameAfterEnabled()
    {
        yield return null;
        UpdateCurrentAIValueText();
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
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

        UpdateCurrentAIValueText();
    }

    private void UpdateCurrentAIValueText()
    {
        int intermediate = (int)(aiDelaySlider.value * 100);
        float v = ((float)intermediate) / 100;//((int)(aiDelaySlider.value * 10)) / 10;
        aiDelayCurrentValue.text = v.ToString();
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
            Screen.SetResolution(RESOLUTION_WIDTH, RESOLUTION_HEIGHT, true);
        }

        PlayerPrefs.SetInt(FULLSCREEN_KEY, ToInt( fullscreenToggle.isOn ) );
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
        PlayerPrefs.SetInt(CUSTOM_CURSOR_KEY, ToInt(customCursorToggle.isOn));
    }

    public void RefreshCursor()
    {
        if (IsTrue(PlayerPrefs.GetInt(CUSTOM_CURSOR_KEY, FALSE)))
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

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
