using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStatesFactory : PopUpOpener
{
    [SerializeField]
    private BattleStatesFactory otherBattleStatesFactory = null;

    [SerializeField]
    private bool isThePlayersFactory = false;

    [SerializeField]
    private Deck deck = null;

    [SerializeField]
    private Hand hand = null;

    [SerializeField]
    private Battlefield battlefield = null;

    [SerializeField]
    private Button endRepositioningBtn = null;

    [SerializeField]
    private AudioClip victoryBGM = null;

    [SerializeField]
    private AudioClip defeatBGM = null;

    public BattleState CreateGameStartState()
    {
        if (isThePlayersFactory)
        {
            return new GameStart(firstToPlayStatesFactory: this, playerStatesFactory: this, enemyStatesFactory: otherBattleStatesFactory);
        }
        else
        {
            return new GameStart(firstToPlayStatesFactory: this, playerStatesFactory: otherBattleStatesFactory, enemyStatesFactory: this);
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
        return new Attack(battlefield, otherBattleStatesFactory.battlefield);
    }

    public BattleState CreateEndTurnState()
    {
        return new EndTurn(battlefield, deck, hand);
    }

    public BattleState CreateBeginTurnState()
    {
        return new BeginTurn(otherBattleStatesFactory.battlefield, deck, hand);
    }

    public BattleState CreateEndGameState(BattleStatesFactory winnerFactory)
    {
        return new EndGame(winnerFactory, popUpOpener, victoryBGM, defeatBGM);
    }
}
