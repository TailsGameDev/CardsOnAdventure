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
    private CurrentBattleInfo.BattleReward rewardType = CurrentBattleInfo.BattleReward.NONE;
    [SerializeField]
    private string rewardDeckName = null;

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

    [SerializeField]
    private string story = null;

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
        PlayerAndEnemyDeckHolder.PrepareEditorMadeDeckForTheEnemy(editorMadeEnemyDeckName);

        if (isTraining || deckClass == Classes.NOT_A_CLASS)
        {
            CurrentBattleInfo.PrepareBattle(battleIcon, 
                backgroundColor, rewardType, rewardDeckName, bgmParam: spotBGM);
        }
        else
        {
            CurrentBattleInfo.PrepareBattle(battleIcon, rewardType, rewardDeckName, deckClass, spotBGM);
        }
    }

    // Referenced in Editor
    public void OnTrainingSpotBtnClicked()
    {
        isTraining = true;
        PlayerAndEnemyDeckHolder.PrepareFirstDeckIfNeededForThePlayerAndGetReadyForSaving();
        OnBattleSpotBtnClicked();
    }

    // Referenced in Editor
    public void OnIncidentSpotClicked()
    {
        MarkSpotToBeClearedIfBelongsToMap();
        incidentPopUps[Random.Range(0, incidentPopUps.Length)].Open(incidentPopUpOpener);
    }

    public void MarkSpotToBeClearedIfBelongsToMap()
    {
        // Commented out as there was a bug about it and currently all spots belong to the map
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
    public void OnStorySpotClicked()
    {
        MarkSpotToBeClearedIfBelongsToMap();
        storyPopUpOpener.Open(story);
    }
    #endregion
}
