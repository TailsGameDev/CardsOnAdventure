using UnityEngine;
using UnityEngine.UI;

public class TipPopUpSection : MonoBehaviour
{
    [SerializeField]
    private Image backgroundImage = null;

    [SerializeField]
    private Text text = null;

    public void PopulateSection(TipSectionData tipSectionData)
    {
        Color c = tipSectionData.color;
        Color color = c;
        // Change the default color from 'transparent' to white
        if (c.r + c.g + c.b + c.a < 0.01)
        {
            color = Color.white;
        }

        if (tipSectionData.background != null)
        {
            backgroundImage.sprite = tipSectionData.background;
            backgroundImage.color = color;
        }

        text.text = tipSectionData.message;
        text.color = color;
    }
}
