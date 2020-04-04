using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsPopUpSection : MonoBehaviour
{
    [SerializeField]
    private Image backgroundImage = null;

    [SerializeField]
    private Text text = null;

    public void PopulateSection(TipSectionData tooltipSectionData)
    {
        backgroundImage.sprite = tooltipSectionData.background;
        text.text = tooltipSectionData.message;

        // If there is an Image, make it's colors appear. Otherwise it should mantain the default color.
        // The default color should be set in editor to alpha == 0 or equals to the pop-up background
        if (tooltipSectionData.background != null)
        {
            backgroundImage.color = Color.white;
        } 
    }
}
