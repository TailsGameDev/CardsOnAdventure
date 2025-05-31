using System.Collections.Generic;
using UnityEngine;

public class Attack : BattleState
{


    private Battlefield attackerBattlefield;
    private Battlefield opponentBattleField;

    private Duelist playerAttacker;
    private Duelist targetPlayer;

    private List<int> attackersThatHaveNotAttacked = new List<int>();

    private UICustomBtn endTurnBtn;
    private bool clickedEndTurnBtn = false;

    private CustomPopUp popUpOpener;

    private bool obfWasFullAtBeggining = false;

    private UICustomBtn repositionAgainBtn;
    private bool clickedRepositionAgainBtn = false;

    private GameObject toActivate;

    private PreMadeAudioRequest confirmOnUselessAtackSFXRequisitor;
    private PreMadeAudioRequest onCancelUselessAtackSFXRequisitor;

    public static bool shouldAskForTip = true;

    private readonly int TOTAL_OF_ATTACKERS;
    const int MAX_AMOUNT_OF_ATTACKS = 1;
    const int DAMAGE_BY_ATTACK_CARD_DEATH = 1;

    public Attack(
                    Duelist playerAttacker,       
                    Duelist playerTarget,       
                    Battlefield attackerBattlefield,
                    Battlefield opponentBattleField,
                    UICustomBtn endTurnBtn,
                    UICustomBtn repositionAgainBtn,
                    GameObject toActivate,
                    CustomPopUp popUpOpener,
                    PreMadeAudioRequest confirmOnUselessAtackSFXRequisitor,
                    PreMadeAudioRequest onCancelUselessAtackSFXRequisitor
                 )
    {
        this.attackerBattlefield = attackerBattlefield;
        this.opponentBattleField = opponentBattleField;
        this.playerAttacker = playerAttacker;
        this.targetPlayer = playerTarget;

        obfWasFullAtBeggining = opponentBattleField.IsFull();

        this.repositionAgainBtn = repositionAgainBtn;

        this.toActivate = toActivate;

        this.confirmOnUselessAtackSFXRequisitor = confirmOnUselessAtackSFXRequisitor;
        this.onCancelUselessAtackSFXRequisitor = onCancelUselessAtackSFXRequisitor;

        toActivate.SetActive(true);

        ClearSelections();

        if (currentBattleStatesFactory == enemyBattleStatesFactory)
        {
            new EnemyAI().Attack(MAX_AMOUNT_OF_ATTACKS, enemyBattlefield: attackerBattlefield, playerBattlefield: opponentBattleField);
        }
        else
        {
            if (shouldAskForTip)
            {
                TipDragAndDrop.AskToUseTips();
                shouldAskForTip = false;
            }
        }

        attackersThatHaveNotAttacked = ListCardsThatCanAttackDuringThisState();
        TOTAL_OF_ATTACKERS = attackersThatHaveNotAttacked.Count + GetAmountOfCardsThatAlreadyAttacked();

        this.endTurnBtn = endTurnBtn;
        this.popUpOpener = popUpOpener;

        if (currentBattleStatesFactory == playerBattleStatesFactory)
        {
            endTurnBtn.onClicked = OnClickedEndTurnBtn;
            endTurnBtn.gameObject.SetActive(true);

            repositionAgainBtn.onClicked = OnClickedRepositionAgainBtn;
            repositionAgainBtn.gameObject.SetActive(true);
        }

    }
    private void ClearSelections()
    {
        attackerBattlefield.ClearSelection();
        opponentBattleField.ClearSelection();
    }
    private List<int> ListCardsThatCanAttackDuringThisState()
    {
        List<int> cards = new List<int>();
        for (int i = 0; i < attackerBattlefield.GetSize(); i++)
        {
            if (attackerBattlefield.ContainsCardInIndex(i))
            {
                Card possibleAttacker = attackerBattlefield.GetReferenceToCardAt(i);
                if (possibleAttacker.CanAttack())
                {
                    cards.Add(i);
                }
            }
        }
        return cards;
    }
    private int GetAmountOfCardsThatAlreadyAttacked()
    {
        int amount = 0;
        for (int i = 0; i < attackerBattlefield.GetSize(); i++)
        {
            if (attackerBattlefield.ContainsCardInIndex(i))
            {
                Card possibleAttacker = attackerBattlefield.GetReferenceToCardAt(i);
                if (possibleAttacker.HasAlreadyAttacked())
                {
                    amount++;
                }
            }
        }
        return amount;
    }
    private void OnClickedEndTurnBtn()
    {
        if (attackersThatHaveNotAttacked.Count == attackerBattlefield.GetAmountOfCardsThatCanAttack())
        {
            popUpOpener.OpenAndMakeUncloseable
                (
                    title: "ATTACK",
                    warningMessage: ColorHexCodes.BeginLightGreen+"DRAG AND DROP YOUR CARDS ABOVE ENEMY'S CARDS"+ColorHexCodes.End,
                    confirmBtnMessage: "Ok, I'll attack",
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
    private void OnClickedRepositionAgainBtn()
    {
        clickedRepositionAgainBtn = true;
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
        if (!clickedEndTurnBtn && !clickedRepositionAgainBtn)
        {
            opponentBattleField.DisplayProtectionVFXOnlyofCardsInBackline();

            MakeSureAttackerCardIsClickedFirst();

            if (ReceivedValidInput())
            {
                bool attackerIgnoresProtection = attackerBattlefield.GetSelectedCard().IgnoresProtection;
                opponentBattleField.MakeProtectionEvidentOnSelectedIfNeeded(attackerIgnoresProtection);

                HandleUselessAttacks();

                int amountOfCardsAliveBeforeAttack = opponentBattleField.GetAmountOfOccupiedSlots();

                Card myCard = attackerBattlefield.GetSelectedCard();
                myCard.AttackSelectedCard(opponentBattleField, attackerBattlefield);

                attackersThatHaveNotAttacked.Remove(attackerBattlefield.GetSelectedIndex());

                if (myCard.IsAlive())
                {
                    attackerBattlefield.MakeSelectedCardEvident();
                    
                    // Obfuscate card to indicate it has already attacked
                    myCard.SetObfuscate(true);
                }
                else
                {
                    playerAttacker.TakeDamage(DAMAGE_BY_ATTACK_CARD_DEATH);
                }    

                // Deal damage to the opponent equal to the amount of his cards that just died
                int amountOfEnemyCardsThatJustDied = amountOfCardsAliveBeforeAttack - opponentBattleField.GetAmountOfOccupiedSlots();
                if (amountOfEnemyCardsThatJustDied > 0)
                {
                    targetPlayer.TakeDamage(amountOfEnemyCardsThatJustDied);
                }

                ClearSelections();
                opponentBattleField.ClearDeathMarks();
            }
            else if (attackerBattlefield.SomeIndexWasSelected())
            {
                Card selectedCard = attackerBattlefield.GetSelectedCard();
                opponentBattleField.DetachCardsThatWouldDie(selectedCard);
            }
            else
            {
                opponentBattleField.ClearDeathMarks();
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

    private bool ReceivedValidInput()
    {
        bool receivedInput = ReceivedInputInBothBattlefields();

        bool receivedInputIsValid = false;

        if (receivedInput)
        {
            receivedInputIsValid = ReceivedInputIsValid();
        }

        bool cardHasAnAttackToken = attackersThatHaveNotAttacked.Contains(attackerBattlefield.GetSelectedIndex());

        return receivedInput && receivedInputIsValid && cardHasAnAttackToken;
    }

    private bool HandleUselessAttacks()
    {
        Card myCard = attackerBattlefield.GetSelectedCard();
        bool isUseless = (!myCard.IgnoresProtection) && (myCard.AttackPower == 1) && opponentBattleField.IsThereACardInFrontOf(opponentBattleField.GetSelectedIndex());

        if (currentBattleStatesFactory == playerBattleStatesFactory)
        {
            if (isUseless)
            {
                popUpOpener.Open
                    (
                        title: "Protected",
                        warningMessage: ColorHexCodes.BeginWhite+"Your "+ColorHexCodes.BeginLightRed+"Attack Power"+ColorHexCodes.End+
                        " of "+ ColorHexCodes.BeginLightRed+"1"+ColorHexCodes.End+" was not enough to deal damage because cards behind others are 'Protected'</color>" +
                        "\n" +ColorHexCodes.BeginLightGreen+"DAMAGE = 1/2 = 0.5 = 0 (integer)"+ColorHexCodes.End,
                        confirmBtnMessage: "Facepalm",
                        cancelBtnMessage: "Offend enemy",
                        onConfirm: () => { confirmOnUselessAtackSFXRequisitor.RequestPlaying(); popUpOpener.ClosePopUpOnTop(); },
                        onCancel: () => { onCancelUselessAtackSFXRequisitor.RequestPlaying() ; popUpOpener.ClosePopUpOnTop(); }
                    );
            }
        }
        return isUseless;
    }

    private bool ClickedInvalidCard()
    {
        int myIndex = attackerBattlefield.GetSelectedIndex();
        bool invalidClickInMyBattlefield = (myIndex != -1) && (!attackersThatHaveNotAttacked.Contains(myIndex));

        int opponentIndex = opponentBattleField.GetSelectedIndex();
        bool invalidClickInOpponentsBattlefield = (opponentIndex != -1) && opponentBattleField.IsSlotIndexFree(opponentIndex);

        return invalidClickInMyBattlefield || invalidClickInOpponentsBattlefield;
    }

    private bool ReceivedInputInBothBattlefields()
    {
        return attackerBattlefield.SomeIndexWasSelectedSinceLastClear() && opponentBattleField.SomeIndexWasSelectedSinceLastClear();
    }

    private bool ReceivedInputIsValid()
    {
        int attackerIndex = attackerBattlefield.GetSelectedIndex();
        int opponentIndex = opponentBattleField.GetSelectedIndex();

        return attackerBattlefield.ContainsCardInIndex(attackerIndex) && opponentBattleField.ContainsCardInIndex(opponentIndex); ;
    }

    public override BattleState GetNextState()
    {
        BattleState nextState = this;

        if (clickedRepositionAgainBtn)
        {
            nextState = currentBattleStatesFactory.CreateRepositionState();
        }
        if (obfWasFullAtBeggining && opponentBattleField.IsEmpty() && attackerBattlefield.IsFull())
        {
            nextState = currentBattleStatesFactory.CreateBonusRepositionState(); 
        }
        else if ( attackerBattlefield.IsEmpty() || opponentBattleField.IsEmpty() || clickedEndTurnBtn ||
                  // Or if there is nobody to attack      
                  TOTAL_OF_ATTACKERS == 0 || attackersThatHaveNotAttacked.Count == 0 ||
                  // Or if too much cards have already attacked
                ( (TOTAL_OF_ATTACKERS - attackersThatHaveNotAttacked.Count) >= MAX_AMOUNT_OF_ATTACKS) )
        {
            nextState = currentBattleStatesFactory.CreateEndTurnState();
        }

        if (nextState != this)
        {
            OnEndingAttackState();
        }

        return nextState;
    }

    private void OnEndingAttackState()
    {
        toActivate.SetActive(false);
        endTurnBtn.gameObject.SetActive(false);
        repositionAgainBtn.gameObject.SetActive(false);
        opponentBattleField.HideAllProtectionVFX();
    }
}
