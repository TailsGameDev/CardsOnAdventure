using System.Collections.Generic;

public class Attack : BattleState
{
    private Battlefield attackerBattlefield;
    private Battlefield opponentBattleField;

    private List<int> attackTokens = new List<int>();

    private UICustomBtn endTurnBtn;
    private bool clickedEndTurnBtn = false;

    private CustomPopUp popUpOpener;

    private bool obfWasFullAtBeggining = false;

    private UICustomBtn repositionAgainBtn;
    private bool clickedRepositionAgainBtn = false;

    private PreMadeSoundRequest confirmOnUselessAtackSFXRequisitor;
    private PreMadeSoundRequest onCancelUselessAtackSFXRequisitor;

    public static bool shouldAskForTip = true;

    public Attack(
                    Battlefield myBattlefield,
                    Battlefield opponentBattleField,
                    UICustomBtn endTurnBtn,
                    UICustomBtn repositionAgainBtn,
                    CustomPopUp popUpOpener,
                    PreMadeSoundRequest confirmOnUselessAtackSFXRequisitor,
                    PreMadeSoundRequest onCancelUselessAtackSFXRequisitor
                 )
    {
        this.attackerBattlefield = myBattlefield;
        this.opponentBattleField = opponentBattleField;

        obfWasFullAtBeggining = opponentBattleField.IsFull();

        this.repositionAgainBtn = repositionAgainBtn;

        this.confirmOnUselessAtackSFXRequisitor = confirmOnUselessAtackSFXRequisitor;
        this.onCancelUselessAtackSFXRequisitor = onCancelUselessAtackSFXRequisitor;

        ClearSelections();

        if (currentBattleStatesFactory == enemyBattleStatesFactory)
        {
            new EnemyAI().Attack(enemyBattlefield: myBattlefield, playerBattlefield: opponentBattleField);
        }
        else
        {
            if (shouldAskForTip)
            {
                TipDragAndDrop.AskToUseTips();
                shouldAskForTip = false;
            }
        }

        attackTokens = ListCardsThatShouldAttackDuringThisState();

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

    private void OnClickedEndTurnBtn()
    {
        if (attackTokens.Count == attackerBattlefield.GetAmountOfOccupiedSlots())
        {
            popUpOpener.OpenAndMakeUncloseable
                (
                    title: "Attack",
                    warningMessage: "<color=#FFFFFF>You should attack before end your turn.</color> " +
                        "<color=#1DEFC7>DRAG AND DROP YOUR CARDS ABOVE ENEMY'S CARDS</color>",
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

    private List<int> ListCardsThatShouldAttackDuringThisState()
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

            attackerBattlefield.MakeOnlySelectedCardBigger();

            MakeSureAttackerCardIsClickedFirst();

            if (ReceivedValidInput())
            {
                bool attackerIgnoresBlock = attackerBattlefield.GetSelectedCard().IgnoreOpponentsBlock;
                opponentBattleField.MakeProtectionEvidentOnSelectedIfNeeded(attackerIgnoresBlock);

                HandleUselessAttacks();

                Card myCard = attackerBattlefield.GetSelectedCard();
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

    private bool HandleUselessAttacks()
    {
        Card myCard = attackerBattlefield.GetSelectedCard();
        bool isUseless = myCard.AttackPower == 1 && opponentBattleField.IsThereACardInFrontOf(opponentBattleField.GetSelectedIndex());

        if (currentBattleStatesFactory == playerBattleStatesFactory)
        {
            if (isUseless)
            {
                popUpOpener.Open
                    (
                        title: "Protected",
                        warningMessage: "<color=#FFFFFF>Your <color=#FD7878>Attack Power</color> of <color=#FD7878>1</color> was not enough to deal damage because cards behind others are 'Protected'</color>" +
                        "\n<color=#1DEFC7>DAMAGE = 1/2 = 0.5 = 0 (integer)</color>",
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
        bool invalidClickInMyBattlefield = (myIndex != -1) && (!attackTokens.Contains(myIndex));

        int opponentIndex = opponentBattleField.GetSelectedIndex();
        bool invalidClickInOpponentsBattlefield = (opponentIndex != -1) && opponentBattleField.IsSlotIndexFree(opponentIndex);

        return invalidClickInMyBattlefield || invalidClickInOpponentsBattlefield;
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

        if (clickedRepositionAgainBtn)
        {
            nextState = currentBattleStatesFactory.CreateRepositionState();
        }
        if (obfWasFullAtBeggining && opponentBattleField.IsEmpty() && attackerBattlefield.IsFull())
        {
            nextState = currentBattleStatesFactory.CreateBonusRepositionState(); 
        }
        else if (attackerBattlefield.IsEmpty() || opponentBattleField.IsEmpty() || attackTokens.Count == 0 || clickedEndTurnBtn)
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
        endTurnBtn.gameObject.SetActive(false);
        repositionAgainBtn.gameObject.SetActive(false);
        opponentBattleField.HideAllProtectionVFX();
    }
}
