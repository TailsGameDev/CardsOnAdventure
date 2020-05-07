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

    private int attackerPower;

    public delegate bool CurrentTargetIsBetterThanTheOneBefore(int indexBefore, int currentIndex, Battlefield obf);

    public static float AIDelay { set => aiDelay = value; }

    #region PlaceCard
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
    private void HandleAIDelay()
    {
        if (aiDelay < MIN_AI_DELAY)
        {
            aiDelay = UISettings.GetAIDelayFromPlayerPrefs();
            // If the player never modified the value, it will still be less than the minimum.
            if (aiDelay < MIN_AI_DELAY)
            {
                aiDelay = DEFAULT_AI_DELAY;
            }
        }
    }
    private IEnumerator PlaceCardCoroutine()
    {
        // Waiting in case of Bonus Reposition from AI, because I think the base constructor is executed first.
        yield return null;
        yield return null;

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
    #endregion

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

    #region Attack
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
    IEnumerator AttackCoroutine()
    {
        // Populate attackers List
        List<Card> attackerCards = new List<Card>();
        List<int> attackerIndexes = new List<int>();
        for (int i = 0; i < enemyBattlefield.GetSize(); i++)
        {
            Card card = enemyBattlefield.GetReferenceToCardAtOrGetNull(i);
            if (card != null && !card.Freezing)
            {
                attackerCards.Add(card);
                attackerIndexes.Add(i);
            }
        }

        // Perform attack for each card in attackers list
        for (int i = 0; i < attackerCards.Count; i++)
        {
            enemyBattlefield.ClearSelection();
            playerBattlefield.ClearSelection();

            Card attacker = attackerCards[i];

            // Note: here, "i" is not always equals to the index in the index of the card in the bf
            enemyBattlefield.SetSelectedIndex(attackerIndexes[i]);

            this.attackerPower = attacker.AttackPower;
            playerBattlefield.LoopThrougCardsAndSelectBestTarget(currentTargetIsBetterThanTheOneBefore);

            yield return new WaitForSeconds(aiDelay);
        }

        UIBattle.inputEnabled = true;
        coroutineExecutor.SelfDestroy();
    }
    private bool currentTargetIsBetterThanTheOneBefore(int indexBefore, int currentIndex, Battlefield obf)
    {
        if (indexBefore < 0)
        {
            L.ogError(this, "negative indexBefore in currentTargetIsBetterThanTheOneBefore");
            return false;
        }

        Card cardBefore = obf.GetReferenceToCardAt(indexBefore);
        Card currentCard = obf.GetReferenceToCardAt(currentIndex);

        if (currentCard == null)
        {
            L.ogError(this, "current card is null. Index: "+currentIndex);
            currentCard = cardBefore;
        }

        bool curentCardWouldTakeDamege = WouldCurrentCardTakeDamage(obf, currentIndex, currentCard);

        bool vitalityIsSmaller = currentCard.Vitality < cardBefore.Vitality;

        return curentCardWouldTakeDamege && vitalityIsSmaller;
    }
    private bool WouldCurrentCardTakeDamage(Battlefield obf, int currentIndex, Card currentCard)
    {
        bool hasDamageReduction = obf.IsThereACardInFrontOf(currentIndex) ||
                                    currentCard.HasHeavyArmorSkill();
        bool damageWouldBeZero = attackerPower <= 1 && hasDamageReduction;
        return !damageWouldBeZero;
    }
    #endregion
}