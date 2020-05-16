using UnityEngine;
using UnityEngine.UI;

public class BonusReposition : PlaceCard
{
    private Hand opponentHand;
    private Deck opponentDeck;

    public BonusReposition
        (
            Hand playerHand,
            Battlefield battlefield,
            Deck deck,
            PreMadeAudioRequest placeCardSFX,
            GameObject btnsBackground,
            CustomPopUp customPopUpOpener,
            Hand opponentHand,
            Deck opponentDeck,
            Text endRepositioningBtnText
        )
        : base
        (
            playerHand,
            battlefield,
            deck,
            placeCardSFX,
            btnsBackground,
            customPopUpOpener
        )
    {
        this.opponentHand = opponentHand;
        this.opponentDeck = opponentDeck;

        if (currentBattleStatesFactory == playerBattleStatesFactory)
        {
            endRepositioningBtnText.text = "Pass ->";
        }

        if ( ! EnemyIsCompletelyDefeated() )
        GiveHandBackToDeck();
        GiveBattlefieldBackToHand();

        TipDragAndDrop.ShowBonusRepositioningTip();
    }

    private bool EnemyIsCompletelyDefeated()
    {
        return opponentHand.IsEmpty() && opponentDeck.IsEmpty();
    }

    private void GiveHandBackToDeck()
    {
        for (int i = 0; i < hand.GetSize(); i++)
        {
            Card card = hand.RemoveCardOrGetNull(i);
            if (card != null)
            {
                deck.PutCardInTop(card);
                const float TIME_GOING_TO_HAND = 0.7f;
                ChildMaker.AdoptAndScaleAndSmoothlyMoveToParent(deck.transform, card.GetRectTransform(), TIME_GOING_TO_HAND);
            }
        }
    }

    private void GiveBattlefieldBackToHand()
    {
        for (int i = 0; i < battlefield.GetSize(); i++)
        {
            Card card = battlefield.RemoveCardOrGetNull(i);
            if (card != null)
            {
                card.ChangeToVerticalVersion();
                hand.AddCard(card);
            }
        }
    }

    public override BattleState GetNextState()
    {
        if (EnemyIsCompletelyDefeated())
        {
            return currentBattleStatesFactory.CreateEndTurnState();
        }
        else
        {
            return base.GetNextState();
        }
    }
}
