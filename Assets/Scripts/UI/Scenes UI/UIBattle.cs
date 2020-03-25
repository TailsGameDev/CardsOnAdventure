using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBattle : PopUpOpener
{
    [SerializeField]
    Battlefield enemyBattlefield = null;

    [SerializeField]
    Battlefield playerBattlefield = null;

    [SerializeField]
    Hand playerHand = null;

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
