using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStatesFactory : MonoBehaviour
{
    [SerializeField]
    private BattleStatesFactory otherBattleStatesFactory = null;

    [SerializeField]
    private bool isThePlayersFactory = false;

    [SerializeField]
    private EnemyAI enemyAI = null;

    [SerializeField]
    private Deck deck = null;

    [SerializeField]
    private Hand hand = null;

    [SerializeField]
    private Battlefield battlefield = null;

    [SerializeField]
    private Button endRepositioningBtn = null;

    public Battlefield GetBattlefield()
    {
        return battlefield;
    }

    public BattleState CreateGameStartState()
    {
        if (isThePlayersFactory)
        {
            return new GameStart(firstToPlayStatesFactory: this, playerStatesFactory: this, enemyStatesFactory: otherBattleStatesFactory, enemyAI);
        }
        else
        {
            return new GameStart(firstToPlayStatesFactory: this, playerStatesFactory: otherBattleStatesFactory, enemyStatesFactory: this, enemyAI);
        }
        
    }

    public BattleState CreateDrawCardState()
    {
        return new DrawCard(deck, battlefield, hand);
    }

    public BattleState CreatePlaceCardState()
    {
        return new PlaceCard(hand, battlefield, deck);
    }

    public BattleState CreateRepositionState()
    {
        return new Reposition(battlefield, endRepositioningBtn);
    }

    public BattleState CreateAttackState()
    {
        return new Attack(battlefield, otherBattleStatesFactory.GetBattlefield());
    }

    public BattleState CreateEndTurnState()
    {
        return new EndTurn(battlefield, deck);
    }

    public BattleState CreateBeginTurnState()
    {
        return new BeginTurn(battlefield, deck);
    }

    public BattleState CreateEndGameState(BattleStatesFactory winnerFactory)
    {
        return new EndGame(winnerFactory);
    }
}
