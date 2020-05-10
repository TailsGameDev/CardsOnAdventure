using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

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
    private AudioClip battleBGM = null;
    [SerializeField]
    private AudioClip winSound = null;
    [SerializeField]
    private AudioClip celebrateSFX = null;

    public void OnBattleSpotBtnClicked()
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
        if (deckClass == Classes.NOT_A_CLASS)
        {
            CurrentBattleInfo.PrepareBattle(backgroundColor, bgmParam: battleBGM);
            DeckPrototypeFactory.PrepareModifiedSizeRandomDeckForTheEnemy(deckSizeMultiplier);
        }
        else
        {
            CurrentBattleInfo.PrepareBattle(deckClass, battleBGM);
            DeckPrototypeFactory.PrepareClassDeckForTheEnemy(deckSizeMultiplier, deckClass);
        }
        Spot spot = transform.parent.GetComponent<Spot>();
        uiMap.SetSpotInfoToClearIfPlayerSucceed(spot);
        sceneOpener.OpenBattle();
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
        Spot spot = transform.parent.GetComponent<Spot>();
        uiMap.SetSpotInfoToClearIfPlayerSucceed(spot);
        sceneOpener.OpenDeckBuildingScene();
    }
    #endregion
}
