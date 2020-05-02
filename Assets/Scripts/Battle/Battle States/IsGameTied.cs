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
                            warningMessage: "If you wish, get a +1 Attack Power buff to all cards currently on the battlefield (your enemy's ones included)",
                            confirmBtnMessage: "Buff all Attacks!",
                            cancelBtnMessage: "No need for buffs.",
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