using UnityEngine;

public class PlaceCard : BattleState
{
    protected Hand hand;
    protected Battlefield battlefield;
    protected Deck deck;
    protected PreMadeSoundRequest placeCardSFXRequest;
    protected GameObject btnsBackground;
    protected CustomPopUp customPopUpOpener;

    bool cardWasSuccessfullyPlaced = false;

    public PlaceCard
        (
            Hand hand,
            Battlefield battlefield,
            Deck deck,
            PreMadeSoundRequest placeCardSFX,
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

        if (currentBattleStatesFactory == enemyBattleStatesFactory)
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

        if (PlayerIsTryingToReposition())
        {
            ClearSelections();

            customPopUpOpener.OpenMessageIfNoCustomPopUpIsOpenned
                (
                    title: "Place Cards",
                        warningMessage: "<color=#FFFFFF> You must <color=#1DEFC7>PLACE ALL CARDS YOU CAN BEFORE REPOSITIONING</color>. Drag and Drop from your hand" +
                        " to the battlefield, please.</color>"
                );
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
        btnsBackground.SetActive(currentBattleStatesFactory == playerBattleStatesFactory);
    }
}
