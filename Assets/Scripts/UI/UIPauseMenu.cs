﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIPauseMenu : OpenersSuperclass
{

    [SerializeField]
    private GameObject fleeFromBattleBtn = null;

    [SerializeField]
    protected RandomSprite cardBackground = null;

    protected virtual void OnEnable()
    {
        if (cardBackground != null)
        {
            cardBackground.ChangeSprite();
        }
        fleeFromBattleBtn.SetActive(SceneManager.GetActiveScene().name == "Battle");
    }

    public void OnBattleRulesBtnClicked()
    {
        openerOfPopUpsMadeInEditor.OpenBattleRulesPopUp();
    }

    public virtual void OnSettingsBtnClicked()
    {
        openerOfPopUpsMadeInEditor.OpenSettingsPopUp();
    }

    public void OnQuitToMainMenuBtnClicked()
    {
        string warningMessage = "All unsaved progress will be lost.";
        customPopUpOpener.OpenConfirmationRequestPopUp(warningMessage, sceneOpener.OpenMainMenu);
    }

    public void OnFleeFromBattleBtnClicked()
    {
        #if !UNITY_EDITOR
        MapsCache.SpotToClearAndLevelUpIfPlayerWins = null;
        #endif
        string warningMessage = "If You flee, the enemies will make fun out of You.";
        customPopUpOpener.OpenConfirmationRequestPopUp(warningMessage, sceneOpener.OpenMapScene);
    }

    public void OnGoImmediateToMainMenuBtnClicked()
    {
        sceneOpener.OpenMainMenu();
    }
}
