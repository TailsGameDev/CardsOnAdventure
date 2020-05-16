using UnityEngine;

public class EndTurn : TurnBattleState
{
    public EndTurn(Battlefield battlefield, Deck deck, Hand hand, GameObject toDeactivate)
    {
        this.battlefield = battlefield;
        this.deck = deck;
        this.hand = hand;

        toDeactivate.SetActive(false);

        // It might be active if there was a Bonus Reposition State
        TipDragAndDrop.HideBonusRepositioningTip();
    }

    public override void ExecuteAction()
    {
        battlefield.RemoveFreezingStateFromAllCards();
        battlefield.RemoveObfuscateFromAllCards();

        hand.RemoveFreezingStateFromAllCards();
        hand.RemoveObfuscateFromAllCards();

        currentBattleStatesFactory = GetTheOtherFactory();
    }

    public override BattleState GetNextState()
    {
        BattleState nextState;

        if (IveLost())
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
