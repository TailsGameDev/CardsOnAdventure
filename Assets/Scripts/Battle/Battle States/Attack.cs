using System.Collections.Generic;

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
        if (attackTokens.Count == attackerBattlefield.GetAmountOfOccupiedSlots())
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
                if (!possibleAttacker.Freezing)
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
        if (!clickedEndTurnBtn)
        {
            opponentBattleField.DisplayProtectionVFXOnlyofCardsInBackline();

            attackerBattlefield.MakeOnlySelectedCardBigger();

            MakeSureAttackerCardIsClickedFirst();

            if (ReceivedValidInput())
            {
                bool attackerIgnoresBlock = attackerBattlefield.GetSelectedCard().IgnoreOpponentsBlock;
                opponentBattleField.MakeProtectionEvidentOnSelectedIfNeeded(attackerIgnoresBlock);

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
