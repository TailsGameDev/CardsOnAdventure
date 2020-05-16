using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTipReceptor : TipReceptor
{
    [Space(10)]

    [SerializeField]
    private Card cardToPullData = null;

    [SerializeField]
    private int textHeight = 0;

    void Start()
    {
        Card c = cardToPullData;

        title = c.GetColoredTitleForTip();

        TipSectionData cardArt = new TipSectionData(background: c.GetCardSprite());

        //TipSectionData section2 = new TipSectionData("<color=#9EFA9D> * SCROLL DOWN FOR MORE TEACHING</color>", 62.5f);

        TipSectionData explanatoryText = new TipSectionData(c.GetExplanatoryText(), textHeight);

        /*
        TipSectionData explanatoryText = new TipSectionData("Note: click a card also opens this pop-up.\n"+
            c.GetExplanatoryText(), textHeight+50);
        */

        tipData = new[] { explanatoryText, cardArt };
    }
}
