using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    [SerializeField]
    private Slider aiDelaySlider = null;

    private const string AI_DELAY_KEY = "AI_DELAY_KEY";

    private void Awake()
    {
        // EnemyAI class has a default value for the delay: 0.5f
        if (PlayerPrefs.HasKey(AI_DELAY_KEY))
        {
            aiDelaySlider.value = PlayerPrefs.GetFloat(AI_DELAY_KEY);
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
}
