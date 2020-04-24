using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;

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
        Camera.main.backgroundColor = backgroundColor;
        Card.ResetDeathCount();
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

        if (IsPlayerTryingToReposition())
        {
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

    private bool IsPlayerTryingToReposition()
    {
        return battlefield.GetSelectedIndex() >= 0 && hand.GetSelectedIndex() == -1;
    }
}

public class Reposition : BattleState
{
    private Battlefield attackerBattlefield;
    private Battlefield opponentBattlefield;
    private UICustomBtn endRepositioningBtn;

    private int oldIndex;
    private int currentIndex;

    private bool repositioningBtnClicked = false;

    // If player tries to attack during the reposition state, let's finish reposition and attack.
    private bool triedToAttack;
    private BattleState nextState;
    private int abfIndex;
    private int obfIndex;

    public Reposition(Battlefield attackerBattlefield, Battlefield opponentBattlefield, UICustomBtn endRepositioningBtn)
    {
        this.attackerBattlefield = attackerBattlefield;
        this.opponentBattlefield = opponentBattlefield;
        this.endRepositioningBtn = endRepositioningBtn;

        ClearSelection();

        endRepositioningBtn.onClicked = OnEndRepositioningBtnClicked;

        if (currentBattleStatesFactory == enemyBattleStatesFactory)
        {
            new EnemyAI().Reposition(attackerBattlefield, endRepositioningBtn);
        }
        else
        {
            endRepositioningBtn.gameObject.SetActive(true);
        }
    }

    private void OnEndRepositioningBtnClicked()
    {
        repositioningBtnClicked = true;
    }

    public override void ExecuteAction()
    {
        if (opponentBattlefield.GetSelectedIndex() != -1)
        {
            triedToAttack = true;
        }

        if (repositioningBtnClicked || triedToAttack)
        {
            endRepositioningBtn.gameObject.SetActive(false);
            return;
        }

        // Stay here until player select a card from his battlefield
        currentIndex = attackerBattlefield.GetSelectedIndex();

        if (currentIndex == -1)
        {
            ClearSelection();
            return;
        }

        attackerBattlefield.MakeCardAtIndexBigger(currentIndex);

        // Cache the index
        if (oldIndex == -1)
        {
            oldIndex = currentIndex;
            return;
        }

        attackerBattlefield.MakeCardAtIndexBigger(oldIndex);

        if ( oldIndex != currentIndex )
        {
            Card oldSelectedCard = attackerBattlefield.GetReferenceToCardAtOrGetNull(oldIndex);
            Card currentSelectedCard = attackerBattlefield.GetReferenceToCardAtOrGetNull(currentIndex);

            if (oldSelectedCard == null && currentSelectedCard != null)
            {
                if ( ! currentSelectedCard.Freezing )
                {
                    attackerBattlefield.MakeCardAtIndexNormalSize(oldIndex);
                    SwapCards();
                }
            } 
            else if (oldSelectedCard != null && currentSelectedCard == null)
            {
                if (!oldSelectedCard.Freezing)
                {
                    attackerBattlefield.MakeCardAtIndexNormalSize(currentIndex);
                    SwapCards();
                }
            }
            else if (oldSelectedCard != null && currentSelectedCard != null)
            {
                if (!oldSelectedCard.Freezing && !currentSelectedCard.Freezing)
                {
                    if (attackerBattlefield.IsSlotIndexOccupied(currentIndex))
                    {
                        attackerBattlefield.MakeCardAtIndexNormalSize(currentIndex);
                    }

                    if (attackerBattlefield.IsSlotIndexOccupied(oldIndex))
                    {
                        attackerBattlefield.MakeCardAtIndexNormalSize(oldIndex);
                    }

                    SwapCards();
                }
            }

            ClearSelection();
        }
    }

    private void SwapCards()
    {
        Card firstCard = attackerBattlefield.RemoveCardOrGetNull(oldIndex);
        Card secondCard = attackerBattlefield.RemoveCardOrGetNull(currentIndex);
        
        if (firstCard != null)
        {
            attackerBattlefield.PutCardInIndex(firstCard, currentIndex);
        }
        if (secondCard != null)
        {
            attackerBattlefield.PutCardInIndex(secondCard, oldIndex);
        }
    }

    private void ClearSelection()
    {
        oldIndex = -1;
        currentIndex = -1;
        attackerBattlefield.ClearSelection();
        opponentBattlefield.ClearSelection();
    }

    public override BattleState GetNextState()
    {
        nextState = this;
        if (repositioningBtnClicked || triedToAttack)
        {
            endRepositioningBtn.onClicked = null;

            BeforeCreateAttackState();
            nextState = currentBattleStatesFactory.CreateAttackState();
            AfterCreateAttackState();
        }
        return nextState;
    }

    // By default, Attack State clear it's selections on the constructor, but if the player tried to
    // attack during the Reposition State, we want the attack to happen.
    private void BeforeCreateAttackState()
    {
        if (triedToAttack)
        {
            abfIndex = attackerBattlefield.GetSelectedIndex();
            obfIndex = opponentBattlefield.GetSelectedIndex();
        }
    }

    private void AfterCreateAttackState()
    {
        if (triedToAttack)
        {
            ((Attack)nextState).SetAttackerSelectedIndex(abfIndex);
            ((Attack)nextState).SetOpponentSelectedIndex(obfIndex);
        }
    }
}

public class Attack : BattleState
{
    private Battlefield attackerBattlefield;
    private Battlefield opponentBattleField;

    private List<int> attackTokens = new List<int>();

    private bool clickedEndTurnBtn = false;

    private UICustomBtn endTurnBtn;

    private CustomPopUp popUpOpener;

    public Attack(Battlefield myBattlefield, Battlefield opponentBattleField, UICustomBtn endTurnBtn, CustomPopUp popUpOpener)
    {
        this.attackerBattlefield = myBattlefield;
        this.opponentBattleField = opponentBattleField;

        ClearSelections();

        if (currentBattleStatesFactory == enemyBattleStatesFactory)
        {
            new EnemyAI().Attack(enemyBattlefield: myBattlefield, playerBattlefield: opponentBattleField);
        }

        attackTokens = ListCardsThatShouldAttackDuringThisState();

        this.endTurnBtn = endTurnBtn;
        this.popUpOpener = popUpOpener;

        if (currentBattleStatesFactory == playerBattleStatesFactory)
        {
            endTurnBtn.onClicked = OnClickedEndTurnBtn;
            endTurnBtn.gameObject.SetActive(true);
        }
    }

    private void ClearSelections()
    {
        attackerBattlefield.ClearSelection();
        opponentBattleField.ClearSelection();
    }

    private void OnClickedEndTurnBtn()
    {
        if (attackTokens.Count == attackerBattlefield.GetStandingAmount())
        {
            popUpOpener.OpenAndMakeUncloseable
                (
                    title: "Attack!",
                    warningMessage: "You should attack before end your turn. Strike that foes down!",
                    confirmBtnMessage: "Ok, I'll attack!",
                    cancelBtnMessage: "I'm a pacifist...",
                    onConfirm: () => { popUpOpener.ClosePopUpOnTop(); },
                    onCancel: () => { clickedEndTurnBtn = true; popUpOpener.ClosePopUpOnTop(); }
                );
        } 
        else
        {
            clickedEndTurnBtn = true;
        }
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

    public void SetOpponentSelectedIndex(int index)
    {
        opponentBattleField.SetSelectedIndex(index);
    }

    public void SetAttackerSelectedIndex(int index)
    {
        attackerBattlefield.SetSelectedIndex(index);
    }

    public override void ExecuteAction()
    {
        if ( ! clickedEndTurnBtn )
        {
            opponentBattleField.DisplayProtectionVFXOnlyofCardsInBackline();

            attackerBattlefield.MakeOnlySelectedCardBigger();

            MakeSureAttackerCardIsClickedFirst();

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
    }

    private void MakeSureAttackerCardIsClickedFirst()
    {
        if (opponentBattleField.GetSelectedIndex() != -1 && attackerBattlefield.GetSelectedIndex() == -1)
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

        if (attackerBattlefield.IsEmpty() || opponentBattleField.IsEmpty() || attackTokens.Count == 0 || clickedEndTurnBtn)
        {
            OnEndingAttackState();
            nextState = currentBattleStatesFactory.CreateEndTurnState();
        }

        return nextState;
    }

    private void OnEndingAttackState()
    {
        endTurnBtn.gameObject.SetActive(false);
        opponentBattleField.HideAllProtectionVFX();
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
        } 
        else if (Card.GetDeathCount() <= 0)
        {
            nextState = currentBattleStatesFactory.CreateIsGameTiedState();
        }
        else
        {
            nextState = currentBattleStatesFactory.CreateBeginTurnState();
        }
        return nextState;
    }
}

public class IsGameTied : BattleState
{
    private CustomPopUp customPopUp;
    private Battlefield whateverBF;
    private Battlefield theOtherBF;

    private bool answeredThePopUp;

    public IsGameTied(CustomPopUp customPopUp, Battlefield whateverBF, Battlefield theOtherBF)
    {
        this.customPopUp = customPopUp;
        this.whateverBF = whateverBF;
        this.theOtherBF = theOtherBF;

        customPopUp.OpenAndMakeUncloseable(
                            title: "Is the game tied?",
                            warningMessage: "If you wish, get a +1 Attack Power buff to all cards (your enemy's ones included)",
                            confirmBtnMessage: "Buff all Attacks!",
                            cancelBtnMessage: "I don't need buffs.",
                            onConfirm: () => { BuffCards(); Proceed(); },
                            onCancel: Proceed
                        );
    }

    private void BuffCards()
    {
        whateverBF.BuffAllCardsAttackPowerForThisMatch();
        theOtherBF.BuffAllCardsAttackPowerForThisMatch();
    }

    void Proceed()
    {
        answeredThePopUp = true;
        Card.ResetDeathCount();
        customPopUp.ClosePopUpOnTop();
    }

    public override void ExecuteAction()
    {
    }

    public override BattleState GetNextState()
    {
        BattleState nextState = this;

        if (answeredThePopUp)
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
    private PreMadeSoundRequest stopAllSFXRequest;

    private float timer = 0;

    private bool quit = false;

    public EndGame(BattleStatesFactory winnerFactory, ThePopUpOpenerInstance popUpOpener, CustomPopUp customPopUpOpener, PreMadeSoundRequest victoryBGMRequest, PreMadeSoundRequest defeatBGMRequest, PreMadeSoundRequest stopAllSFXRequest)
    {
        this.winnerFactory = winnerFactory;
        this.popUpOpener = popUpOpener;
        this.customPopUpOpener = customPopUpOpener;
        this.victoryBGMRequest = victoryBGMRequest;
        this.defeatBGMRequest = defeatBGMRequest;
        this.stopAllSFXRequest = stopAllSFXRequest;
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
                        if ( IsMasterBattle() )
                        {
                            customPopUpOpener.Open(
                                title: "You've beaten a Master!",
                                warningMessage: "All your cards of class "+masterClass+" level up!\nPlease, choose what to improve on them!",
                                confirmBtnMessage: "+1 Vitality",
                                cancelBtnMessage: "+1 Attack Power",
                                onConfirm: ImproveVitalityThenSeeMap,
                                onCancel: ImproveAttackPowerThenSeeMap,
                                victoryBGMRequest
                            );
                        }
                        else
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
                    }
                    else
                    {
                        MapsCache.SpotToClearIfPlayerWins = null;
                        customPopUpOpener.Open(
                            title: "You've lost the battle",
                            warningMessage: "The enemy start to search you fallen card's pockets",
                            confirmBtnMessage: "Go to main menu",
                            cancelBtnMessage: "Sit and cry",
                            QuitBattleLoadMainMenu,
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

    private void QuitBattleAndGoToMap()
    {
        quit = true;
        stopAllSFXRequest.RequestPlaying();
        popUpOpener.CloseAllPopUpsExceptLoading();
        popUpOpener.OpenMapPopUp();
    }

    private void ImproveAttackPowerThenSeeMap()
    {
        ClassInfo.GiveAttackPowerBonusToClass(masterClass);
        QuitBattleAndGoToMap();
    }

    private void ImproveVitalityThenSeeMap()
    {
        ClassInfo.GiveVitalityBonusToClass(masterClass);
        QuitBattleAndGoToMap();
    }

    private void QuitBattleLoadMainMenu()
    {
        popUpOpener.SetLoadingPopUpActiveToTrue();
        popUpOpener.CloseAllPopUpsExceptLoading();
        stopAllSFXRequest.RequestPlaying();
        quit = true;
        // popUpOpener.ResetMap(); Map is being reset by the OnPlayBtnClicked method at the main menu, in MainMenuCanvas.cs
        // Cards buffs being reset at play btn on main menu
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

