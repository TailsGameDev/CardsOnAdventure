using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpotPrototype : OpenersSuperclass
{
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
    private AudioClip spotBGM = null;
    [SerializeField]
    private AudioClip winSound = null;
    [SerializeField]
    private AudioClip celebrateSFX = null;

    private bool isTraining = false;

    public void OnBattleSpotBtnClicked()
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
        if (isTraining)
        {
            CurrentBattleInfo.PrepareBattle(backgroundColor, bgmParam: spotBGM);
            DeckPrototypeFactory.PrepareTrainingDummyDeckForTheEnemy();
        }
        else if (deckClass == Classes.NOT_A_CLASS)
        {
            CurrentBattleInfo.PrepareBattle(backgroundColor, bgmParam: spotBGM);
            DeckPrototypeFactory.PrepareModifiedSizeRandomDeckForTheEnemy(deckSizeMultiplier);
        }
        else
        {
            CurrentBattleInfo.PrepareBattle(deckClass, spotBGM);
            DeckPrototypeFactory.PrepareClassDeckForTheEnemy(deckSizeMultiplier, deckClass);
        }
        MarkSpotToBeCleared();
        sceneOpener.OpenBattle();
    }

    public void OnTrainingSpotBtnClicked()
    {
        isTraining = true;
        OnBattleSpotBtnClicked();
    }

    private void MarkSpotToBeCleared()
    {
        Spot spot = transform.parent.GetComponent<Spot>();
        uiMap.SetSpotInfoToClearIfPlayerSucceed(spot);
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
        MarkSpotToBeCleared();
        sceneOpener.OpenDeckBuildingScene();
    }
    #endregion
}
