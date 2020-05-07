public class PlaceCard : BattleState
{
    protected Hand hand;
    protected Battlefield battlefield;
    protected Deck deck;
    protected PreMadeSoundRequest placeCardSFXRequest;

    bool cardWasSuccessfullyPlaced = false;

    public PlaceCard(Hand playerHand, Battlefield battlefield, Deck deck, PreMadeSoundRequest placeCardSFX)
    {
        this.hand = playerHand;
        this.battlefield = battlefield;
        this.deck = deck;
        this.placeCardSFXRequest = placeCardSFX;


        playerHand.ClearSelection();
        battlefield.ClearSelection();

        if (currentBattleStatesFactory == enemyBattleStatesFactory)
        {
            new EnemyAI().PlaceCard(hand, battlefield);
        }
    }

    public override void ExecuteAction()
    {
        hand.MakeOnlySelectedCardBigger();

        if (ReceivedValidInput())
        {
            hand.MakeSelectedCardNormalSize();

            Card card = hand.RemoveCardFromSelectedIndex();

            battlefield.PutCardInSelectedIndex(card, smooth: currentBattleStatesFactory == enemyBattleStatesFactory);
            cardWasSuccessfullyPlaced = true;

            placeCardSFXRequest.RequestPlaying();

            card.ChangeToHorizontalVersion();
        }
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

        if (IsPlayerTryingToReposition())
        {
            // This will trigger the Dynamic text in the middle of the screen to expand, remembering the player to "Place Card"
            nextState = currentBattleStatesFactory.CreateDrawCardState();
        }
        else if (cardWasSuccessfullyPlaced)
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
                    nextState = currentBattleStatesFactory.CreateRepositionState();
                }
            }
        }
        
        if (hand.IsEmpty() || battlefield.IsFull())
        {
            nextState = currentBattleStatesFactory.CreateRepositionState();
        }

        return nextState;
    }
    protected bool IsPlayerTryingToReposition()
    {
        return battlefield.GetSelectedIndex() >= 0 && hand.GetSelectedIndex() == -1;
    }
}
