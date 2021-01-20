using UnityEngine;
using UnityEngine.UI;

public class SpotPrototype : OpenersSuperclass
{
    [SerializeField]
    private bool belongsToMap = true;
    [SerializeField]
    private Map uiMap = null;
    [SerializeField]
    private AudioRequisitor audioRequisitor = null;

    [SerializeField]
    private Color backgroundColor = Color.white;

    [SerializeField]
    private Classes deckClass = Classes.NOT_A_CLASS;

    [SerializeField]
    private bool isMasterBattle = false;

    [SerializeField]
    private AudioClip spotBGM = null;
    [SerializeField]
    private AudioClip winSound = null;
    [SerializeField]
    private AudioClip celebrateSFX = null;

    private bool isTraining = false;

    [SerializeField]
    private IncidentPopUp[] incidentPopUps = null;

    [SerializeField]
    private string editorMadeEnemyDeckName = null;

    private Sprite battleIcon;

    public bool BelongsToMap { set => belongsToMap = value; }

    public void Awake()
    {
        battleIcon = GetComponent<Image>().sprite;
    }

    public void OnBattleSpotBtnClicked()
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrueAndDeactivateTips();
        PrepareBattle();
        MarkSpotToBeClearedIfBelongsToMap();
        sceneOpener.OpenBattle();
    }
    public void PrepareBattle()
    {
        DeckPrototypeFactory.PrepareEditorMadeDeckForTheEnemy(editorMadeEnemyDeckName);

        if (isTraining || deckClass == Classes.NOT_A_CLASS)
        {
            CurrentBattleInfo.PrepareBattle(battleIcon, 
                backgroundColor, giveRewardToSameClassOfMasterDeckOnWin: false, bgmParam: spotBGM);
        }
        else
        {
            CurrentBattleInfo.PrepareBattle(battleIcon, isMasterBattle, deckClass, spotBGM);
        }
    }

    public void OnTrainingSpotBtnClicked()
    {
        isTraining = true;
        DeckPrototypeFactory.PrepareFirstDeckIfNeededForThePlayerAndGetReadyForSaving();
        OnBattleSpotBtnClicked();
    }

    public void OnIncidentSpotClicked()
    {
        MarkSpotToBeClearedIfBelongsToMap();
        incidentPopUps[Random.Range(0, incidentPopUps.Length)].Open(incidentPopUpOpener);
    }

    public void MarkSpotToBeClearedIfBelongsToMap()
    {
        if (belongsToMap)
        {
            Spot spot = transform.parent.GetComponent<Spot>();
            uiMap.SetSpotInfoToClearIfPlayerSucceed(spot);
        }
    }

    #region On "Some Not Battle Spot" Clicked
    public void OnEndOfGameClicked()
    {
        audioRequisitor.RequestBGMToPlayOneSingleTime(winSound);
        customPopUpOpener.Open(
                title: "You Beat the game",
                warningMessage: "You are Awesome",
                confirmBtnMessage: "Celebrate",
                cancelBtnMessage: "Go to Menu",
                onConfirm: () => { audioRequisitor.RequestSFX(celebrateSFX); },
                onCancel: sceneOpener.OpenMainMenu
            );
    }
    public void OnDeckBuildBtnClicked()
    {
        MarkSpotToBeClearedIfBelongsToMap();
        sceneOpener.OpenDeckBuildingScene();
    }
    #endregion
}
