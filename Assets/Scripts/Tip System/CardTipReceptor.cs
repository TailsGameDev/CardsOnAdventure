using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTipReceptor : TipReceptor
{
    [Space(10)]

    [SerializeField]
    private Card cardToPullData = null;

    [SerializeField]
    private Sprite ignoreIconSprite = null;

    [SerializeField]
    private int textHeight = 0;

    private int offset = 50;

    void Start()
    {
        Card c = cardToPullData;

        title = c.GetColoredTitleForTip();

        TipSectionData cardArt = new TipSectionData(background: c.GetCardSprite());

        TipSectionData explanatoryText = new TipSectionData(c.GetExplanatoryText(), textHeight + offset);

        if (cardToPullData.IgnoreOpponentsBlock)
        {
            TipSectionData ignoreBlock = new TipSectionData(message: "    -> IGNORES PROTECTION.", background: ignoreIconSprite, height: 70);
            tipData = new[] { ignoreBlock, explanatoryText,  cardArt };
        }
        else
        {
            tipData = new[] { explanatoryText, cardArt };
        }
    }
}
