using UnityEngine;

public class EndTurn : TurnBattleState
{
    public EndTurn(Duelist duelist, Battlefield battlefield, Deck deck, Hand hand, GameObject toDeactivate)
    {
        this.battlefield = battlefield;
        this.hand = hand;
        this.duelist = duelist;

        toDeactivate.SetActive(false);

        // It might be active if there was a Bonus Reposition State
        TipDragAndDrop.HideBonusRepositioningTip();
    }

    public override void ExecuteAction()
    {
        battlefield.RemoveFreezingStateFromAllCards();
        battlefield.RemoveObfuscateFromAllCards();

        // TODO: Review if needed
        hand.RemoveFreezingStateFromAllCards();
        hand.RemoveObfuscateFromAllCards();

        currentBattleStatesFactory = GetTheOtherFactory();
    }

    public override BattleState GetNextState()
    {
        BattleState nextState;

        if (HasPlayerLost())
        {
            // Create end game with the other player as the winner
            nextState = currentBattleStatesFactory.CreateEndGameState(winnerFactory: currentBattleStatesFactory);
        }
        else if (DeathCounter.AreCardsDyingTooFew())
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
