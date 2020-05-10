using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTipReceptor : TipsDragAndDropReceptor
{
    [SerializeField]
    private Card cardToPullData = null;

    void Start()
    {
        Card c = cardToPullData;

        title = c.GetColoredTitleForTip();

        TipSectionData section1 = new TipSectionData(background: c.GetCardSprite());

        TipSectionData section2 = new TipSectionData("<color=#1DEFC7> * SCROLL DOWN FOR MORE TEACHING</color>", 62.5f);

        TipSectionData section3 = new TipSectionData(c.GetExplanatoryText());

        tipData = new[] { section1, section2, section3 };
    }
}
