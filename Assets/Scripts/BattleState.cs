using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BattleState
{
    public abstract void ExecuteAction();
    public abstract BattleState GetNextState();

    protected static BattleStatesFactory currentBattleStatesFactory;

    protected static BattleStatesFactory playerBattleStatesFactory;
    protected static BattleStatesFactory enemyBattleStatesFactory;
}

public class GameStart : BattleState
{

    public GameStart(BattleStatesFactory playerStatesFactory, BattleStatesFactory enemyStatesFactory)
    {
        playerBattleStatesFactory = playerStatesFactory;
        enemyBattleStatesFactory = enemyStatesFactory;
    }

    public override void ExecuteAction()
    {
        currentBattleStatesFactory = playerBattleStatesFactory;
    }

    public override BattleState GetNextState()
    {
        return currentBattleStatesFactory.CreateDrawCardState();
    }
}

public class DrawCard : BattleState
{
    private Deck deck;
    private Battlefield battlefield;
    private Hand hand;

    public DrawCard(Deck deck, Battlefield battlefield, Hand hand)
    {
        this.deck = deck;
        this.battlefield = battlefield;
        this.hand = hand;
    }

    public override void ExecuteAction()
    {
        if (deck.ContainCards())
        {
            Card firstOfDeck = deck.DrawCard();
            hand.AddCard(firstOfDeck);
        }
    }

    public override BattleState GetNextState()
    {
        BattleState nextState = this;

        if (hand.IsFull())
        {
            if (battlefield.IsFull())
            {
                nextState = currentBattleStatesFactory.CreateRepositionState();
            } else
            {
                nextState = currentBattleStatesFactory.CreatePlaceCardState();
            }
        }

        return nextState;
    }
}

public class PlaceCard : BattleState
{
    private Hand hand;
    private Battlefield battlefield;
    private Deck deck;

    bool cardWasSuccessfullyPlaced = false;

    public PlaceCard(Hand playerHand, Battlefield battlefield, Deck deck)
    {
        this.hand = playerHand;
        this.battlefield = battlefield;
        this.deck = deck;

        playerHand.ClearSelection();
        battlefield.ClearSelection();
    }

    public override void ExecuteAction()
    {
        if ( CanPlaceCards() )
        {
            Card card = hand.RemoveCardFromSelectedIndex();
            battlefield.PlaceCardInSelectedIndex(card);
            cardWasSuccessfullyPlaced = true;
        }
    }

    public bool CanPlaceCards()
    {
        bool receivedInput = hand.SomeIndexWasSelectedSinceLastClear() && battlefield.SomeIndexWasSelectedSinceLastClear();
        int handIndex = hand.GetSelectedIndex();
        int battlefieldIndex = battlefield.GetSelectedIndex();

        bool inputReceivedIsValid = false;

        if (receivedInput)
        {
            inputReceivedIsValid = hand.ContainsCardInIndex(handIndex) && battlefield.IsSlotIndexFree(battlefieldIndex);
        }

        return receivedInput && inputReceivedIsValid;
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
                nextState = currentBattleStatesFactory.CreateRepositionState();
            }
        }

        return nextState;
    }
}

public class Reposition : BattleState
{
    private Battlefield battlefield;
    private UnityEngine.UI.Button endRepositioningBtn;

    private int oldIndex;
    private int currentIndex;

    private bool repositioningEnded = false;

    public Reposition(Battlefield battlefield, Button endRepositioningBtn)
    {
        this.battlefield = battlefield;
        this.endRepositioningBtn = endRepositioningBtn;

        oldIndex = battlefield.GetSelectedIndex();

        endRepositioningBtn.onClick.AddListener(OnEndRepositioning);
        endRepositioningBtn.gameObject.SetActive(true);
    }

    private void OnEndRepositioning()
    {
        repositioningEnded = true;
    }

    public override void ExecuteAction()
    {
        if (repositioningEnded)
        {
            endRepositioningBtn.gameObject.SetActive(false);
            return;
        }

        currentIndex = battlefield.GetSelectedIndex();

        if (currentIndex == -1)
        {
            return;
        }

        if (oldIndex == -1)
        {
            oldIndex = currentIndex;
            return;
        }

        if ( oldIndex != currentIndex )
        {
            SwapCards();
            ClearSelection();
        }
    }

    private bool ShouldSwap()
    {
        return oldIndex != currentIndex;
    }

    private void SwapCards()
    {
        Card firsCard = battlefield.RemoveCard(oldIndex);
        Card secondCard = battlefield.RemoveCard(currentIndex);

        battlefield.PutCardInIndex(firsCard, currentIndex);
        battlefield.PutCardInIndex(secondCard, oldIndex);
    }

    private void ClearSelection()
    {
        oldIndex = -1;
        currentIndex = -1;
        battlefield.ClearSelection();
    }

    public override BattleState GetNextState()
    {
        BattleState nextState = this;
        if (repositioningEnded)
        {
            return currentBattleStatesFactory.CreateAttackState();
        }
        return nextState;
    }
}

public class Attack : BattleState
{
    public override void ExecuteAction()
    {
        
    }

    public override BattleState GetNextState()
    {
        return this;
    }
}

