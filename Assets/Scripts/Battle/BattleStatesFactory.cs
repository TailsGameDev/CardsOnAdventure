using UnityEngine;

public class BattleStatesFactory : OpenersSuperclass
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
    private GameObject btnsBackground = null;
    [SerializeField]
    private UICustomBtn endRepositioningBtn = null;
    [SerializeField]
    private UICustomBtn repositionAgainBtn = null;
    [SerializeField]
    private UICustomBtn endTurnBtn = null;

    [SerializeField]
    private PreMadeAudioFactory preMadeAudioFactory = null;

    [SerializeField]
    private AudioRequisitor audioRequisitor = null;

    [SerializeField]
    private GameObject sceneCanvasGameObject = null;

    [SerializeField]
    private GameObject activateOnRepositionState = null;

    [SerializeField]
    private GameObject activateOnAttackState = null;

    public BattleState CreateGameStartState()
    {
        if (isThePlayersFactory)
        {
            return new GameStart(firstToPlayStatesFactory: this, playerStatesFactory: this, enemyStatesFactory: otherBattleStatesFactory, audioRequisitor);
        }
        else
        {
            return new GameStart(firstToPlayStatesFactory: this, playerStatesFactory: otherBattleStatesFactory, enemyStatesFactory: this, audioRequisitor);
        }
    }
    public BattleState CreateDrawCardState()
    {
        return new DrawCard(deck, battlefield, hand);
    }
    // If fully ordered by flow order, the Place Card state would be here, but it is at the end of the class.
    public BattleState CreateRepositionState()
    {
        return new Reposition(battlefield, otherBattleStatesFactory.battlefield, endRepositioningBtn, activateOnRepositionState);
    }
    public BattleState CreateAttackState()
    {
        return new Attack
            (
                battlefield,
                otherBattleStatesFactory.battlefield,
                endTurnBtn,
                repositionAgainBtn,
                activateOnAttackState,
                customPopUpOpener,
                preMadeAudioFactory.CreateFacepalmAudioRequest(assignor: gameObject),
                preMadeAudioFactory.CreateOffendAudioRequest(assignor: gameObject)
            );
    }
    public BattleState CreateEndTurnState()
    {
        return new EndTurn(battlefield, deck, hand, btnsBackground);
    }
    public BattleState CreateIsGameTiedState()
    {
        return new OfferBuff(customPopUpOpener, whateverBF: battlefield, theOtherBF: otherBattleStatesFactory.battlefield);
    }
    public BattleState CreateBeginTurnState()
    {
        return new BeginTurn(battlefield, deck, hand);
    }
    public BattleState CreateEndGameState(BattleStatesFactory winnerFactory)
    {
        return new EndGame (
                                winnerFactory, 
                                sceneCanvasGameObject,
                                openerOfPopUpsMadeInEditor,
                                customPopUpOpener,
                                sceneOpener,
                                preMadeAudioFactory.CreateVictoryAudioRequest(gameObject),
                                preMadeAudioFactory.CreateDefeatAudioRequest(gameObject),
                                preMadeAudioFactory.CreateStopAllSFXRequest(gameObject),
                                preMadeAudioFactory.CreateCricketsAudioRequest(gameObject),
                                preMadeAudioFactory.CreateCryingAudioRequest(gameObject)
                            );
    }

    public BattleState CreatePlaceCardState()
    {
        return new PlaceCard
            (
                hand,
                battlefield,
                deck,
                placeCardSFX: GetRandomPlaceCardPreMadeSoundRequest(),
                btnsBackground,
                customPopUpOpener
            );
    }
    public BattleState CreateBonusRepositionState()
    {
        return new BonusReposition
            (
                hand,
                battlefield,
                deck,
                placeCardSFX: GetRandomPlaceCardPreMadeSoundRequest(),
                btnsBackground,
                customPopUpOpener,
                otherBattleStatesFactory.hand,
                otherBattleStatesFactory.deck,
                endRepositioningBtn.GetTextComponent()
            );
    }
    private PreMadeAudioRequest GetRandomPlaceCardPreMadeSoundRequest()
    {
        return preMadeAudioFactory.CreateRandomPlaceCardAudioRequest(gameObject);
    }
}
