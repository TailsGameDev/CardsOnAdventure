using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BattleState : BattleInfo
{
    public abstract void ExecuteAction();
    public abstract BattleState GetNextState();

    protected static BattleStatesFactory currentBattleStatesFactory;

    protected static BattleStatesFactory playerBattleStatesFactory;
    protected static BattleStatesFactory enemyBattleStatesFactory;

    public bool IsPlayerTurn()
    {
        return currentBattleStatesFactory == playerBattleStatesFactory;
    }
}

public class GameStart : BattleState
{

    public GameStart(BattleStatesFactory firstToPlayStatesFactory, BattleStatesFactory playerStatesFactory, BattleStatesFactory enemyStatesFactory)
    {
        currentBattleStatesFactory = firstToPlayStatesFactory;
        playerBattleStatesFactory = playerStatesFactory;
        enemyBattleStatesFactory = enemyStatesFactory;
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
            if (deck.IsEmpty())
            {
                nextState = currentBattleStatesFactory.CreatePlaceCardState();
            } else
            {
                // Draw more cards!
                nextState = this;
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

        if (currentBattleStatesFactory == enemyBattleStatesFactory)
        {
            new EnemyAI().PlaceCard(hand, battlefield);
        }
    }

    public override void ExecuteAction()
    {
        if ( ReceivedValidInput() )
        {
            Card card = hand.RemoveCardFromSelectedIndex();
            battlefield.PlaceCardInSelectedIndex(card);
            cardWasSuccessfullyPlaced = true;
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

        if (cardWasSuccessfullyPlaced)
        {
            if (deck.ContainCards())
            {
                nextState = currentBattleStatesFactory.CreateDrawCardState();
            }
            else
            {
                if ( hand.HasCards() && battlefield.HasEmptySlot())
                {
                    if (currentBattleStatesFactory == enemyBattleStatesFactory)
                    {
                        // Create the state again triggers EnemyAI
                        nextState = currentBattleStatesFactory.CreatePlaceCardState();
                    }
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

        ClearSelection();

        endRepositioningBtn.onClick.AddListener(OnEndRepositioning);

        if (currentBattleStatesFactory == enemyBattleStatesFactory)
        {
            new EnemyAI().Reposition(battlefield, endRepositioningBtn);
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
            Card oldSelectedCard = battlefield.GetReferenceToCardAt(oldIndex);
            Card currentSelectedCard = battlefield.GetReferenceToCardAt(currentIndex);
            if ( ! oldSelectedCard.Freezing && ! currentSelectedCard.Freezing )
            {
                SwapCards();
            }
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
        return nextState;
    }
}

public class Attack : BattleState
{
    private Battlefield attackerBattlefield;
    private Battlefield opponentBattleField;

    private List<int> attackTokens = new List<int>();

    public Attack(Battlefield myBattlefield, Battlefield opponentBattleField)
    {
        this.attackerBattlefield = myBattlefield;
        this.opponentBattleField = opponentBattleField;

        ClearSelections();

        if (currentBattleStatesFactory == enemyBattleStatesFactory)
        {
            new EnemyAI().Attack(enemyBattlefield: myBattlefield, playerBattlefield: opponentBattleField);
        }

        attackTokens = ListCardsThatShouldAttackDuringThisState();
    }

    private void ClearSelections()
    {
        attackerBattlefield.ClearSelection();
        opponentBattleField.ClearSelection();
    }

    private List<int> ListCardsThatShouldAttackDuringThisState()
    {
        List<int> cards = new List<int>();
        for (int i = 0; i < attackerBattlefield.GetSize(); i++)
        {
            if (attackerBattlefield.ContainsCardInIndex(i))
            {
                Card possibleAttacker = attackerBattlefield.GetReferenceToCardAt(i);
                if ( ! possibleAttacker.Freezing )
                {
                    cards.Add(i);
                }
            }
        }
        return cards;
    }

    public override void ExecuteAction()
    {
        if (ReceivedValidInput())
        {
            Card myCard = attackerBattlefield.GetReferenceToSelectedCard();
            myCard.AttackSelectedCard(opponentBattleField, attackerBattlefield);

            attackTokens.Remove(attackerBattlefield.GetSelectedIndex());

            ClearSelections();
        }

        if (ClickedInvalidCard())
        {
            ClearSelections();
            Debug.Log("Clicked invalid card");
        }
    }

    private bool ClickedInvalidCard()
    {
        int myIndex = attackerBattlefield.GetSelectedIndex();
        bool invalidClickInMyBattlefield = (myIndex != -1) && (!attackTokens.Contains(myIndex));

        int opponentIndex = opponentBattleField.GetSelectedIndex();
        bool invalidClickInOpponentsBattlefield = (opponentIndex != -1) && opponentBattleField.IsSlotIndexFree(opponentIndex);

        return invalidClickInMyBattlefield || invalidClickInOpponentsBattlefield;
    }

    private bool ReceivedValidInput()
    {
        bool receivedInput = ReceivedInputInBothBattlefields();

        bool receivedInputIsValid = false;

        if (receivedInput)
        {
            receivedInputIsValid = ReceivedInputIsValid();
        }

        bool cardHasAnAttackToken = attackTokens.Contains(attackerBattlefield.GetSelectedIndex());

        return receivedInput && receivedInputIsValid && cardHasAnAttackToken;
    }

    private bool ReceivedInputInBothBattlefields()
    {
        return attackerBattlefield.SomeIndexWasSelectedSinceLastClear() && opponentBattleField.SomeIndexWasSelectedSinceLastClear(); ;
    }

    private bool ReceivedInputIsValid()
    {
        int myIndex = attackerBattlefield.GetSelectedIndex();
        int opponentIndex = opponentBattleField.GetSelectedIndex();

        return attackerBattlefield.ContainsCardInIndex(myIndex) && opponentBattleField.ContainsCardInIndex(opponentIndex); ;
    }

    public override BattleState GetNextState()
    {
        BattleState nextState = this;

        if (attackerBattlefield.IsEmpty() || opponentBattleField.IsEmpty() || attackTokens.Count == 0)
        {
            nextState = currentBattleStatesFactory.CreateEndTurnState();
        }

        return nextState;
    }
}

public abstract class TurnBattleState : BattleState
{
    protected Battlefield opponentBattlefield;
    protected Deck deck;
    protected Hand hand;

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
        return deck.IsEmpty() && opponentBattlefield.IsEmpty() && hand.IsEmpty();
    }
}

public class EndTurn : TurnBattleState
{
    public EndTurn(Battlefield battlefield, Deck deck, Hand hand)
    {
        this.opponentBattlefield = battlefield;
        this.deck = deck;
        this.hand = hand;
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
        return nextState;
    }
}

public class BeginTurn : TurnBattleState
{
    public BeginTurn(Battlefield opponentBattlefield, Deck deck, Hand hand)
    {
        this.opponentBattlefield = opponentBattlefield;
        this.deck = deck;
        this.hand = hand;
    }

    public override void ExecuteAction()
    {
        opponentBattlefield.RemoveFreezingStateFromAllCards();
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

        return nextState;
    }
}

public class EndGame : BattleState
{
    private BattleStatesFactory winnerFactory;

    public EndGame(BattleStatesFactory winnerFactory)
    {
        this.winnerFactory = winnerFactory;
    }

    public override void ExecuteAction()
    {
        if (winnerFactory == playerBattleStatesFactory)
        {
            Debug.LogWarning("You win!");
        }
        else
        {
            Debug.LogWarning("You loose!");
        }
    }

    public override BattleState GetNextState()
    {
        return this;
    }
}

