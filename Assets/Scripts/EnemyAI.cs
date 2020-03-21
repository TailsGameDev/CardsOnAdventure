using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Hand enemyHand = null;

    private Battlefield enemyBattlefield = null;

    private Battlefield playerBattlefield = null;

    private UnityEngine.UI.Button endRepositionBtn;

    public void PlaceCard(Hand enemyHand, Battlefield enemyBattlefield)
    {
        this.enemyHand = enemyHand;
        this.enemyBattlefield = enemyBattlefield;

        if ( ! enemyHand.IsEmpty() )
        {
            enemyHand.SelectFirstOccupiedIndex();
        }

        if ( ! enemyBattlefield.IsFull())
        {
            enemyBattlefield.SelectFirstFreeIndex();
        }
    }

    public void Reposition(Battlefield enemyBattlefield, UnityEngine.UI.Button endRepositionBtn)
    {
        this.enemyBattlefield = enemyBattlefield;
        this.endRepositionBtn = endRepositionBtn;
        StartCoroutine(RepositionCoroutine());
    }

    IEnumerator RepositionCoroutine()
    {
        bool change0With2 = ChangeCardInFrontWithCardBehind(0, 2);
        bool change1With3 = ChangeCardInFrontWithCardBehind(1, 3);
        yield return null;

        if (change0With2)
        {
            enemyBattlefield.SetSelectedIndex(0);
        }
        yield return null;

        if (change0With2)
        {
            enemyBattlefield.SetSelectedIndex(2);
        }
        yield return null;

        if (change1With3)
        {
            enemyBattlefield.SetSelectedIndex(1);
        }
        yield return null;

        if (change1With3)
        {
            enemyBattlefield.SetSelectedIndex(3);
        }
        yield return null;

        endRepositionBtn.onClick.Invoke();
    }

    private bool ChangeCardInFrontWithCardBehind(int inFrontIndex, int behindIndex)
    {
        bool change;

        if (enemyBattlefield.IsSlotIndexFree(inFrontIndex) || enemyBattlefield.IsSlotIndexFree(behindIndex))
        {
            change = false;
        }
        else
        {
            Card cardInFront = enemyBattlefield.GetReferenceToCardAt(inFrontIndex);
            Card cardBehind = enemyBattlefield.GetReferenceToCardAt(behindIndex);
            change = cardInFront.GetVitality() < cardBehind.GetVitality();
        }

        return change;
    }
}