using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipSection : MonoBehaviour
{
    [SerializeField]
    private Image backgroundImage = null;

    [SerializeField]
    private Text text = null;

    public void PopulateSection(TooltipSectionData tooltipSectionData)
    {
        backgroundImage.sprite = tooltipSectionData.background;
        text.text = tooltipSectionData.message;

        if (tooltipSectionData.background != null)
        {
            backgroundImage.color = Color.white;
        }
    }
}
