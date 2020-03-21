using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBattle : MonoBehaviour
{
    [SerializeField]
    Battlefield enemyBattlefield = null;

    [SerializeField]
    Battlefield playerBattlefield = null;

    [SerializeField]
    Hand playerHand = null;

    public void OnEnemyBattleFieldSlotClicked(int index)
    {
        WhenBattleFieldIsClicked(enemyBattlefield, index);
    }

    public void OnPlayerBattleFieldSlotClicked(int index)
    {
        WhenBattleFieldIsClicked(playerBattlefield, index);
    }

    void WhenBattleFieldIsClicked(Battlefield battlefield, int index)
    {
        battlefield.SetSelectedIndex(index);
    }

    public void OnPlayerHandSlotClicked(int index)
    {
        playerHand.SetSelectedIndex(index);
    }
}
