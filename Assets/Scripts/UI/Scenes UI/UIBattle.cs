using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBattle : PopUpOpener
{
    [SerializeField]
    private RectTransform UIDamageTextParent = null;
    public static RectTransform parentOfDynamicUIThatMustAppear;

    [SerializeField]
    private Battlefield enemyBattlefield = null;

    [SerializeField]
    private Battlefield playerBattlefield = null;

    [SerializeField]
    private Hand playerHand = null;

    private void Awake()
    {
        parentOfDynamicUIThatMustAppear = UIDamageTextParent;
    }

    public void OnEnemyBattleFieldSlotClicked(int index)
    {
        WhenAnyBattleFieldIsClicked(enemyBattlefield, index);
    }

    public void OnPlayerBattleFieldSlotClicked(int index)
    {
        WhenAnyBattleFieldIsClicked(playerBattlefield, index);
    }

    void WhenAnyBattleFieldIsClicked(Battlefield battlefield, int index)
    {
        battlefield.SetSelectedIndex(index);
    }

    public void OnPlayerHandSlotClicked(int index)
    {
        playerHand.SetSelectedIndex(index);
    }

    public void OnPauseBtnClicked()
    {
        popUpOpener.OpenPausePopUp();
    }
}
