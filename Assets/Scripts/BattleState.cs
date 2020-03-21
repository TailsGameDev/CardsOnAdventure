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

    protected static EnemyAI enemyAI;
}

public class GameStart : BattleState
{

    public GameStart(BattleStatesFactory firstToPlayStatesFactory, BattleStatesFactory playerStatesFactory, BattleStatesFactory enemyStatesFactory, EnemyAI AI)
    {
        currentBattleStatesFactory = firstToPlayStatesFactory;
        playerBattleStatesFactory = playerStatesFactory;
        enemyBattleStatesFactory = enemyStatesFactory;
        enemyAI = AI;
    }

    public override void ExecuteAction()
    {
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
        if (deck.ContainCards() && ! hand.IsFull())
        {
            Card firstOfDeck = deck.DrawCard();
            hand.AddCard(firstOfDeck);
        }
    }

    public override BattleState GetNextState()
    {
        BattleState nextState;

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
        else
        {
            nextState = this;
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

        if (currentBattleStatesFactory == enemyBattleStatesFactory)
        {
            enemyAI.PlaceCard(hand, battlefield);
        }
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

        if (hand.IsEmpty())
        {
            nextState = currentBattleStatesFactory.CreateAttackState();
        }

        Debug.Log("[" + GetType().Name + "] nextState = " + nextState.GetType().Name);
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

        if (currentBattleStatesFactory == enemyBattleStatesFactory)
        {
            enemyAI.Reposition(battlefield, endRepositioningBtn);
        }
        else
        {
            endRepositioningBtn.gameObject.SetActive(true);
        }
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
            endRepositioningBtn.onClick.RemoveListener(OnEndRepositioning);
            nextState = currentBattleStatesFactory.CreateAttackState();
        }
        Debug.Log("[" + GetType().Name + "] nextState = " + nextState.GetType().Name);
        return nextState;
    }
}

public class Attack : BattleState
{
    private Battlefield myBattlefield;
    private Battlefield opponentBattleField;

    public Attack(Battlefield myBattlefield, Battlefield opponentBattleField)
    {
        this.myBattlefield = myBattlefield;
        this.opponentBattleField = opponentBattleField;
    }

    public override void ExecuteAction()
    {
        
    }

    public override BattleState GetNextState()
    {
        BattleState nextState = this;

        Debug.Log("[Attack] myBattlefield.IsEmpty(): " + myBattlefield.IsEmpty()+ "; opponentBattleField.IsEmpty(): "+ opponentBattleField.IsEmpty());

        if (myBattlefield.IsEmpty() || opponentBattleField.IsEmpty())
        {
            nextState = currentBattleStatesFactory.CreateEndTurnState();
        }

        return nextState;
    }
}

public abstract class TurnBattleState : BattleState
{
    protected Battlefield battlefield;
    protected Deck deck;

    protected BattleStatesFactory GetTheOtherFactory()
    {
        BattleStatesFactory otherFactory;
        if (currentBattleStatesFactory == playerBattleStatesFactory)
        {
            otherFactory = enemyBattleStatesFactory;
        }
        else
        {
            otherFactory = playerBattleStatesFactory;
        }
        return otherFactory;
    }

    protected bool IveLost()
    {
        return deck.IsEmpty() && battlefield.IsEmpty();
    }
}

public class EndTurn : TurnBattleState
{
    public EndTurn(Battlefield battlefield, Deck deck)
    {
        this.battlefield = battlefield;
        this.deck = deck;
    }

    public override void ExecuteAction()
    {
        currentBattleStatesFactory = GetTheOtherFactory();
    }

    public override BattleState GetNextState()
    {
        BattleState nextState;

        if(IveLost())
        {
            nextState = currentBattleStatesFactory.CreateEndGameState(winnerFactory: currentBattleStatesFactory);
        } else
        {
            nextState = currentBattleStatesFactory.CreateBeginTurnState();
        }
        Debug.Log("[" + GetType().Name + "] nextState = " + nextState.GetType().Name);
        return nextState;
    }
}

public class BeginTurn : TurnBattleState
{
    public BeginTurn(Battlefield battlefield, Deck deck)
    {
        this.battlefield = battlefield;
        this.deck = deck;
    }

    public override void ExecuteAction()
    {
        
    }

    public override BattleState GetNextState()
    {
        BattleState nextState;

        if (IveLost())
        {
            nextState = currentBattleStatesFactory.CreateEndGameState(winnerFactory: GetTheOtherFactory());
        }
        else
        {
            nextState = currentBattleStatesFactory.CreateDrawCardState();
        }

        Debug.Log("["+GetType().Name+"] nextState = " + nextState.GetType().Name);
        return nextState;
    }
}

public class EndGame : BattleState
{
    private BattleStatesFactory looserFactory;

    public EndGame(BattleStatesFactory looserFactory)
    {
        this.looserFactory = looserFactory;
    }

    public override void ExecuteAction()
    {
        if (looserFactory == playerBattleStatesFactory)
        {
            Debug.LogWarning("You lose!");
        }
        else
        {
            Debug.LogWarning("You win!");
        }
    }

    public override BattleState GetNextState()
    {
        return this;
    }
}

