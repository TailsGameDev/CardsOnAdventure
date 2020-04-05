using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI
{
    private Hand enemyHand = null;

    private Battlefield enemyBattlefield = null;

    private Battlefield playerBattlefield = null;

    private UICustomBtn endRepositionBtn;

    CoroutineExecutorPrototype coroutineExecutor;

    public void PlaceCard(Hand enemyHand, Battlefield enemyBattlefield)
    {
        this.enemyHand = enemyHand;
        this.enemyBattlefield = enemyBattlefield;

        if ( enemyHand.HasCards() )
        {
            enemyHand.SelectFirstOccupiedIndex();
        }

        if ( enemyBattlefield.HasEmptySlot() )
        {
            enemyBattlefield.SelectFirstFreeIndex();
        }
    }

    #region Reposition

    public void Reposition(Battlefield enemyBattlefield, UICustomBtn endRepositionBtn)
    {
        this.enemyBattlefield = enemyBattlefield;
        this.endRepositionBtn = endRepositionBtn;

        coroutineExecutor = CoroutineExecutorPrototype.GetCopy();

        coroutineExecutor.ExecuteCoroutine(RepositionCoroutine());
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

        endRepositionBtn.onClicked();

        coroutineExecutor.SelfDestroy();
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

    /*
    private void ExecuteRepositionWithCustomUpdate()
    {
        if (coroutineExecutor == null)
        {
            coroutineExecutor = CoroutineExecutorPrototype.GetCopy();
        }
        RepositionAction RepositionAction = new RepositionAction(enemyBattlefield, endRepositionBtn);
        coroutineExecutor.ExecuteCustomUpdateUntillCountLimit(RepositionAction, 6);
    }
    */
    #endregion

    public void Attack(Battlefield enemyBattlefield, Battlefield playerBattlefield)
    {
        this.enemyBattlefield = enemyBattlefield;
        this.playerBattlefield = playerBattlefield;

        if ( ! enemyBattlefield.IsEmpty() && ! playerBattlefield.IsEmpty() )
        {
            coroutineExecutor = CoroutineExecutorPrototype.GetCopy();

            coroutineExecutor.ExecuteCoroutine(AttackCoroutine());
        }
    }

    IEnumerator AttackCoroutine()
    {
        for (int i = 0; i < playerBattlefield.GetSize(); i++)
        {
            if (enemyBattlefield.ContainsCardInIndex(i))
            {
                enemyBattlefield.SetSelectedIndex(i);
                playerBattlefield.SelectCardIndexWithLowestVitality();
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}

// Not being used, but CustomUpdate is an alternative to coroutine usage
/*
public abstract class CustomUpdate
{
    public abstract void Execute();
}

public class RepositionAction : CustomUpdate
{
    private int frameCounter = 0;
    private Battlefield enemyBattlefield;
    private UICustomBtn endRepositionBtn;

    public RepositionAction(Battlefield enemyBattlefield, UICustomBtn endRepositionBtn)
    {
        this.enemyBattlefield = enemyBattlefield;
        this.endRepositionBtn = endRepositionBtn;
    }

    public override void Execute()
    {
        bool change0With2 = ChangeCardInFrontWithCardBehind(0, 2);
        bool change1With3 = ChangeCardInFrontWithCardBehind(1, 3);

        frameCounter++;

        switch (frameCounter)
        {
            case 1:
                if (change0With2)
                {
                    enemyBattlefield.SetSelectedIndex(0);
                }
                break;
            case 2:
                if (change0With2)
                {
                    enemyBattlefield.SetSelectedIndex(2);
                }
                break;
            case 3:
                if (change1With3)
                {
                    enemyBattlefield.SetSelectedIndex(1);
                }
                break;
            case 4:
                if (change1With3)
                {
                    enemyBattlefield.SetSelectedIndex(3);
                }
                break;
            case 5:
                endRepositionBtn.onClicked();
                break;
        }
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
*/