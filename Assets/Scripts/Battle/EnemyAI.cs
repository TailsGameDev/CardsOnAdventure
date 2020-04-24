using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI
{
    private static float aiDelay = 0.0f;

    private Hand enemyHand = null;

    private Battlefield enemyBattlefield = null;

    private Battlefield playerBattlefield = null;

    private UICustomBtn endRepositionBtn;

    private CoroutineExecutorPrototype coroutineExecutor;

    private const float MIN_AI_DELAY = 0.5f;
    private const float DEFAULT_AI_DELAY = 1.5f;

    public delegate bool CurrentTargetIsBetterThanTheOneBefore(int indexBefore, int currentIndex, int attackPower, Battlefield obf);

    public static float AIDelay { set => aiDelay = value; }

    public void PlaceCard(Hand enemyHand, Battlefield enemyBattlefield)
    {
        UIBattle.inputEnabled = false;

        HandleAIDelay();

        this.enemyHand = enemyHand;
        this.enemyBattlefield = enemyBattlefield;

        coroutineExecutor = CoroutineExecutorPrototype.GetCopy();

        coroutineExecutor.gameObject.AddComponent(typeof(DestroyItselfInTime));

        coroutineExecutor.ExecuteCoroutine(PlaceCardCoroutine());
    }

    private IEnumerator PlaceCardCoroutine()
    {
        if (enemyHand.HasCards())
        {
            yield return new WaitForSeconds(aiDelay/2);
            enemyHand.SelectFirstOccupiedIndex();
        }

        if (enemyBattlefield.HasEmptySlot())
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

        coroutineExecutor.gameObject.AddComponent(typeof(DestroyItselfInTime));

        coroutineExecutor.ExecuteCoroutine(RepositionCoroutine());
    }

    IEnumerator RepositionCoroutine()
    {
        bool change0With2 = ChangeCardInFrontWithCardBehind(0, 2);
        bool change1With3 = ChangeCardInFrontWithCardBehind(1, 3);

        yield return new WaitForSeconds(aiDelay/2);

        if (change0With2)
        {
            enemyBattlefield.SetSelectedIndex(0);
            yield return new WaitForSeconds(aiDelay/2);
            enemyBattlefield.SetSelectedIndex(2);
            yield return new WaitForSeconds(aiDelay/2);
        }


        if (change1With3)
        {
            enemyBattlefield.SetSelectedIndex(1);
            yield return new WaitForSeconds(aiDelay / 2);
            enemyBattlefield.SetSelectedIndex(3);
            yield return new WaitForSeconds(aiDelay / 2);
        }

        yield return new WaitForSeconds(aiDelay / 2);

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
            change = cardInFront.Vitality < cardBehind.Vitality;
        }

        return change;
    }
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
        else
        {
            UIBattle.inputEnabled = true;
        }
    }

    private void HandleAIDelay()
    {
        if (aiDelay < MIN_AI_DELAY)
        {
            aiDelay = UISettings.GetAIDelayFromPlayerPrefs();
            // Ff the player never modified the value, it will still be less than the minimum.
            if (aiDelay < MIN_AI_DELAY)
            {
                aiDelay = DEFAULT_AI_DELAY;
            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        for (int i = 0; i < playerBattlefield.GetSize(); i++)
        {
            if (enemyBattlefield.ContainsCardInIndex(i))
            {
                enemyBattlefield.SetSelectedIndex(i);
                int attackPower = enemyBattlefield.GetSelectedCard().AttackPower;
                playerBattlefield.LoopThrougEnemyesAndSelectBestTarget(currentTargetIsBetterThanTheOneBefore, attackPower);

                yield return new WaitForSeconds(aiDelay);
            }
        }

        UIBattle.inputEnabled = true;
        coroutineExecutor.SelfDestroy();
    }

    private bool currentTargetIsBetterThanTheOneBefore(int indexBefore, int currentIndex, int attackPower, Battlefield obf)
    {
        Card cardBefore = obf.GetReferenceToCardAt(indexBefore);
        Card currentCard = obf.GetReferenceToCardAt(currentIndex);

        bool dealsDamage = obf.IsThereACardInFrontOf(currentIndex) && attackPower <= 1;
        bool vitalityIsSmaller = currentCard.Vitality < cardBefore.Vitality;

        return dealsDamage && vitalityIsSmaller;
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