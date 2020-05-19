using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private float deckSizeMultiplier = 0;

    [SerializeField]
    private bool fillWholeDeckWithClass = false;

    [SerializeField]
    private AudioClip spotBGM = null;
    [SerializeField]
    private AudioClip winSound = null;
    [SerializeField]
    private AudioClip celebrateSFX = null;

    private bool isTraining = false;

    [SerializeField]
    private IncidentPopUp[] incidentPopUps = null;

    public bool BelongsToMap { set => belongsToMap = value; }

    public void OnBattleSpotBtnClicked()
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
        PrepareBattle();
        MarkSpotToBeClearedIfBelongsToMap();
        sceneOpener.OpenBattle();
    }
    public void PrepareBattle()
    {
        
        if (isTraining)
        {
            CurrentBattleInfo.PrepareBattle(backgroundColor, bgmParam: spotBGM);
            DeckPrototypeFactory.PrepareTrainingDummyDeckForTheEnemy();
        }
        else if (deckClass == Classes.NOT_A_CLASS)
        {
            CurrentBattleInfo.PrepareBattle(backgroundColor, bgmParam: spotBGM);
            DeckPrototypeFactory.PrepareModifiedSizeRandomDeckWithoutMonstersForTheEnemy(deckSizeMultiplier);
        }
        else
        {
            CurrentBattleInfo.PrepareBattle(deckClass, spotBGM);
            if (fillWholeDeckWithClass)
            {
                DeckPrototypeFactory.PrepareFullClassDeckForTheEnemy(deckSizeMultiplier, deckClass);
            }
            else
            {
                DeckPrototypeFactory.PrepareHalfClassDeckForTheEnemy(deckSizeMultiplier, deckClass);
            }
        }
    }

    public void OnTrainingSpotBtnClicked()
    {
        isTraining = true;
        DeckPrototypeFactory.PrepareTrainingDeckForThePlayer();
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
        audioRequisitor.RequestBGM(winSound);
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
