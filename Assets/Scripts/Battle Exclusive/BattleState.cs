using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    private PreMadeSoundRequest placeCardSFXRequest;

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

        if ( ReceivedValidInput() )
        {
            hand.MakeSelectedCardNormalSize();

            Card card = hand.RemoveCardFromSelectedIndex();

            battlefield.PlaceCardInSelectedIndex(card);
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
    private UIBtn endRepositioningBtn;

    private int oldIndex;
    private int currentIndex;

    private bool repositioningEnded = false;

    public Reposition(Battlefield battlefield, UIBtn endRepositioningBtn)
    {
        this.battlefield = battlefield;
        this.endRepositioningBtn = endRepositioningBtn;

        ClearSelection();

        endRepositioningBtn.onUIBtnClicked += OnEndRepositioning;

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

        battlefield.MakeCardAtIndexBigger(currentIndex);

        if (oldIndex == -1)
        {
            oldIndex = currentIndex;
            return;
        }

        battlefield.MakeCardAtIndexBigger(oldIndex);

        if ( oldIndex != currentIndex )
        {
            Card oldSelectedCard = battlefield.GetReferenceToCardAt(oldIndex);
            Card currentSelectedCard = battlefield.GetReferenceToCardAt(currentIndex);
            if ( ! oldSelectedCard.Freezing && ! currentSelectedCard.Freezing )
            {
                battlefield.MakeCardAtIndexNormalSize(currentIndex);
                battlefield.MakeCardAtIndexNormalSize(oldIndex);
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
            endRepositioningBtn.onUIBtnClicked = null;
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
        attackerBattlefield.MakeOnlySelectedCardBigger();

        if (ReceivedValidInput())
        {
            Card myCard = attackerBattlefield.GetReferenceToSelectedCard();
            myCard.AttackSelectedCard(opponentBattleField, attackerBattlefield);

            attackTokens.Remove(attackerBattlefield.GetSelectedIndex());

            attackerBattlefield.MakeSelectedCardNormalSize();

            ClearSelections();

            myCard.SetObfuscate(true);
        }

        if (ClickedInvalidCard())
        {
            ClearSelections();
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
    protected Battlefield battlefield;
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
        return deck.IsEmpty() && battlefield.IsEmpty() && hand.IsEmpty();
    }
}

public class EndTurn : TurnBattleState
{
    public EndTurn(Battlefield battlefield, Deck deck, Hand hand)
    {
        this.battlefield = battlefield;
        this.deck = deck;
        this.hand = hand;
    }

    public override void ExecuteAction()
    {
        battlefield.RemoveFreezingStateFromAllCards();
        battlefield.RemoveObfuscateFromAllCards();

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
    public BeginTurn(Battlefield battlefield, Deck deck, Hand hand)
    {
        this.battlefield = battlefield;
        this.deck = deck;
        this.hand = hand;
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

        return nextState;
    }
}

public class EndGame : BattleState
{
    private BattleStatesFactory winnerFactory;
    private ThePopUpOpenerInstance popUpOpener;
    private CustomPopUp customPopUpOpener;
    private PreMadeSoundRequest victoryBGMRequest;
    private PreMadeSoundRequest defeatBGMRequest;

    private float timer = 0;

    private bool quit = false;

    public EndGame(BattleStatesFactory winnerFactory, ThePopUpOpenerInstance popUpOpener, CustomPopUp customPopUpOpener, PreMadeSoundRequest victoryBGMRequest, PreMadeSoundRequest defeatBGMRequest)
    {
        this.winnerFactory = winnerFactory;
        this.popUpOpener = popUpOpener;
        this.customPopUpOpener = customPopUpOpener;
        this.victoryBGMRequest = victoryBGMRequest;
        this.defeatBGMRequest = defeatBGMRequest;
    }

    public override void ExecuteAction()
    {
        if ( ! quit )
        {
            if (timer < 1.5f)
            {
                timer += Time.deltaTime;
            } 
            else
            {
                if ( popUpOpener.AllPopUpsAreClosed() )
                {
                    timer = 0;

                    if (winnerFactory == playerBattleStatesFactory)
                    {
                        customPopUpOpener.Open(
                            title : "Congratulations!",
                            warningMessage: "You beat those guys. What are you going to do now?",
                            confirmBtnMessage: "Look the map!",
                            cancelBtnMessage: "Nothing",
                            QuitBattleAndGoToMap,
                            victoryBGMRequest
                        );
                    }
                    else
                    {
                        customPopUpOpener.Open(
                            title: "You've lost the battle",
                            warningMessage: "The enemy start to search you fallen card's pockets",
                            confirmBtnMessage: "Go to main menu",
                            cancelBtnMessage: "Sit and cry",
                            QuitBattleResetMapLoadMainMenu,
                            defeatBGMRequest
                        );
                    }
                } 
                else
                // Some pop up is oppened
                { 
                    timer = 0.0f;
                }
            }
        }

    }

    void QuitBattleAndGoToMap()
    {
        quit = true;
        popUpOpener.UpdateMap();
        popUpOpener.CloseAllPopUpsExceptLoading();
        popUpOpener.OpenMapPopUp();
    }

    private void QuitBattleResetMapLoadMainMenu()
    {
        quit = true;
        popUpOpener.CloseAllPopUpsExceptLoading();
        popUpOpener.SetLoadingPopUpActiveToTrue();
        // popUpOpener.ResetMap(); Map is being reset by the OnPlayBtnClicked method at the main menu, in MainMenuCanvas.cs
        SceneManager.LoadScene("Main Menu");
    }

    public override BattleState GetNextState()
    {
        BattleState nextState;

        if (quit)
        {
            nextState = new Quit();
        }
        else
        {
            nextState = this;
        }

        return nextState;
    }
}

public class Quit : BattleState
{
    public override void ExecuteAction()
    {
    }

    public override BattleState GetNextState()
    {
        return this;
    }
}

