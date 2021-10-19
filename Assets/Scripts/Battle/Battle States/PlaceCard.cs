using UnityEngine;
using System.Collections.Generic;

public class PlaceCard : BattleState
{
    protected Hand hand;
    protected Battlefield battlefield;
    protected Deck deck;
    protected PreMadeAudioRequest placeCardSFXRequest;
    protected GameObject btnsBackground;
    protected CustomPopUp customPopUpOpener;

    private bool cardWasSuccessfullyPlaced = false;
    private bool shouldDisplayPlaceCardsNowTip;

    public PlaceCard
        (
            Hand hand,
            Battlefield battlefield,
            Deck deck,
            PreMadeAudioRequest placeCardSFX,
            GameObject btnsBackground,
            CustomPopUp customPopUpOpener
        )
    {
        this.hand = hand;
        this.battlefield = battlefield;
        this.deck = deck;
        this.placeCardSFXRequest = placeCardSFX;
        this.btnsBackground = btnsBackground;
        this.customPopUpOpener = customPopUpOpener;
        
        btnsBackground.SetActive(false);

        ClearSelections();

        if (!IsPlayerTurn())
        {
            new EnemyAI().PlaceCard(this.hand, battlefield);
        }
    }

    private void ClearSelections()
    {
        hand.ClearSelection();
        battlefield.ClearSelection();
    }

    public override void ExecuteAction()
    {
        hand.MakeOnlySelectedCardBigger();

        if (shouldDisplayPlaceCardsNowTip)
        {
            shouldDisplayPlaceCardsNowTip = false;
            TipDragAndDrop.ShowPlaceCardsNowTip();
        }
        else if (PlayerIsTryingToReposition())
        {
            ClearSelections();
            shouldDisplayPlaceCardsNowTip = true;
            // I suspect the lag of bringing this UI to the screen was the cause of a bug in which the card couldn't be dropped.
            // That's why the PlaceCardsTip is not being displayed immediatelly.
        }
        else if (ReceivedValidInput())
        {
            hand.MakeSelectedCardNormalSize();

            Card card = hand.RemoveCardFromSelectedIndex();

            bool smooth = currentBattleStatesFactory == enemyBattleStatesFactory;
            if (smooth)
            {
                battlefield.PutCardInSelectedIndexWithSmoothMovement(card);
            }
            else
            {
                battlefield.PutCardInSelectedIndexThenTeleportToSlot(card);
            }
            cardWasSuccessfullyPlaced = true;

            placeCardSFXRequest.RequestPlaying();

            card.ChangeToHorizontalVersion();
        }
    }

    protected bool PlayerIsTryingToReposition()
    {
        return battlefield.GetSelectedIndex() >= 0 && hand.GetSelectedIndex() == -1;
    }

    public bool ReceivedValidInput()
    {
        bool receivedInput = hand.SomeIndexWasSelectedSinceLastClear() && battlefield.SomeIndexWasSelectedSinceLastClear();
        int handIndex = hand.GetSelectedIndex();
        int battlefieldIndex = battlefield.GetSelectedIndex();

        bool receivedInputIsValid = false;

        if (receivedInput)
        {
            receivedInputIsValid = hand.ContainsCardInIndex(handIndex) && battlefield.IsSlotIndexFree(battlefieldIndex);
        }

        return receivedInput && receivedInputIsValid;
    }

    public override BattleState GetNextState()
    {
        BattleState nextState = this;

        if (cardWasSuccessfullyPlaced)
        {
            if (deck.ContainCards())
            {
                nextState = currentBattleStatesFactory.CreateDrawCardState();
            }
            else
            {
                if (hand.HasCards() && battlefield.HasEmptySlot())
                {
                    // Create the state again triggers EnemyAI
                    nextState = currentBattleStatesFactory.CreatePlaceCardState();
                }
                else
                {
                    OnGoToRepositionState();
                    nextState = currentBattleStatesFactory.CreateRepositionState();
                }
            }
        }
        
        if (hand.IsEmpty() || battlefield.IsFull())
        {
            OnGoToRepositionState();
            nextState = currentBattleStatesFactory.CreateRepositionState();
        }

        return nextState;
    }

    protected void OnGoToRepositionState()
    {
        TipDragAndDrop.HidePlaceCardsNowTip();
        btnsBackground.SetActive(currentBattleStatesFactory == playerBattleStatesFactory);
    }
}
