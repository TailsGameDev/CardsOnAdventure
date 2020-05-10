using UnityEngine;

public class BonusReposition : PlaceCard
{
    private Hand opponentHand;
    private Deck opponentDeck;

    public BonusReposition
        (
            Hand playerHand,
            Battlefield battlefield,
            Deck deck,
            PreMadeSoundRequest placeCardSFX,
            GameObject btnsBackground,
            CustomPopUp customPopUpOpener,
            PreMadeSoundRequest offendDeveloperAudioRequisitor,
            Hand opponentHand,
            Deck opponentDeck
        )
        : base
        (
            playerHand,
            battlefield,
            deck,
            placeCardSFX,
            btnsBackground,
            customPopUpOpener,
            offendDeveloperAudioRequisitor
        )
    {
        this.opponentHand = opponentHand;
        this.opponentDeck = opponentDeck;

        if ( ! EnemyIsCompletelyDefeated() )
        GiveHandBackToDeck();
        GiveBattlefieldBackToHand();
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
                ChildMaker.AdoptAndScaleAndSmoothlyMoveToParent(deck.transform, card.GetComponent<RectTransform>(), TIME_GOING_TO_HAND);
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
