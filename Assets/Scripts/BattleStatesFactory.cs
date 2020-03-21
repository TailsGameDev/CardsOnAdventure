using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStatesFactory : MonoBehaviour
{
    [SerializeField]
    private BattleStatesFactory otherBattleStatesFactory = null;

    [SerializeField]
    private Deck deck = null;

    [SerializeField]
    private Hand hand = null;

    [SerializeField]
    private Battlefield battlefield = null;

    [SerializeField]
    private Button endRepositioningBtn = null;

    public BattleState CreateGameStartState()
    {
        return new GameStart(this, otherBattleStatesFactory);
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
        return new Attack();
    }
}
