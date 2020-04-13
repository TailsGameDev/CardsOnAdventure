using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTipHolder : TipsDragAndDropReceptor
{
    [SerializeField]
    private Card cardToPullData = null;

    void Start()
    {
        Card c = cardToPullData;

        title = c.GetColoredTitleForTooltip();

        TipSectionData section1 = new TipSectionData(background: c.GetCardSprite());

        TipSectionData section2 = new TipSectionData(c.GetExplanatoryText());

        tipData = new[] { section1, section2 };
    }

}
