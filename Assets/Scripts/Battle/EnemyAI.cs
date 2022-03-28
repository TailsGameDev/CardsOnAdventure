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

    private int maxAmountOfAttacks;

    public delegate bool CurrentTargetIsBetterThanPrevious(int previousIndex, int currentIndex, Battlefield obf);

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
            aiDelay = Settings.GetAIDelayFromPlayerPrefs();
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
            yield return WaitForSeconds(aiDelay/2);
            enemyHand.SelectFirstOccupiedIndex();
        }

        if (enemyBattlefield.HasEmptySlot())
        {
            enemyBattlefield.SelectFirstFreeIndex();
        }
    }
    #endregion
    protected IEnumerator WaitForSeconds(float seconds)
    {
        while (TimeFacade.TimeIsStopped)
        {
            yield return null;
        }

        yield return new WaitForSeconds(seconds);
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
    private IEnumerator RepositionCoroutine()
    {
        bool change0With2 = ChangeCardInFrontWithCardBehind(0, 2);
        bool change1With3 = ChangeCardInFrontWithCardBehind(1, 3);

        // Shit Avoidance that you can test to see if is necessary
        yield return null; yield return null;
        enemyBattlefield.ClearSelection();
        yield return null; yield return null;

        if (change0With2)
        {
            enemyBattlefield.SetSelectedIndex(0);
            yield return WaitForSeconds(aiDelay / 2);
            enemyBattlefield.SetSelectedIndex(2);
            yield return WaitForSeconds(aiDelay / 2);
        }

        // Shit Avoidance that you can test to see if is necessary
        yield return null; yield return null;
        enemyBattlefield.ClearSelection();
        yield return null; yield return null;

        if (change1With3)
        {
            enemyBattlefield.SetSelectedIndex(1);
            yield return WaitForSeconds(aiDelay / 2);
            enemyBattlefield.SetSelectedIndex(3);
            yield return WaitForSeconds(aiDelay / 2);
        }

        // Shit Avoidance that you can test to see if is necessary
        yield return null; yield return null;
        enemyBattlefield.ClearSelection();
        yield return null; yield return null;

        if (endRepositionBtn != null)
        {
            endRepositionBtn.onClicked();
        }
        else
        {
            Debug.LogError("[EnemyAI] endRepositionBtn is null.");
        }

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
    public void Attack(int maxAmountOfAttacks, Battlefield enemyBattlefield, Battlefield playerBattlefield)
    {
        this.maxAmountOfAttacks = maxAmountOfAttacks;
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
    private IEnumerator AttackCoroutine()
    {
        // Populate attackers List
        List<Card> attackerCards = new List<Card>();
        List<int> attackerIndexes = new List<int>();

        iAttackersChooser attackersChooser = new StrongestAttackersChooser();
        attackersChooser.PopulateAttackersLists(this, attackerCards, attackerIndexes);

        // Perform attack for each card in attackers list
        for (int i = 0; (i < attackerCards.Count) && playerBattlefield.HasCards(); i++)
        {
            yield return WaitForSeconds(aiDelay);

            enemyBattlefield.ClearSelection();
            playerBattlefield.ClearSelection();

            Card attacker = attackerCards[i];

            // Note: here, "i" is not always equals to the index in the index of the card in the bf
            enemyBattlefield.SetSelectedIndex(attackerIndexes[i]);

            this.attackerPower = attacker.AttackPower;
            playerBattlefield.LoopThrougCardsAndSelectBestTarget(IsCurrentTargetBetterThanPrevious);
        }

        UIBattle.inputEnabled = true;
        coroutineExecutor.SelfDestroy();
    }
    private bool IsCurrentTargetBetterThanPrevious(int previousIndex, int currentIndex, Battlefield obf)
    {
        bool isCurrentTargetBetterThanPrevious;

        if (obf.IsSlotIndexOccupied(currentIndex))
        {
            Card cardBefore = obf.GetReferenceToCardAt(previousIndex);
            Card currentCard = obf.GetReferenceToCardAt(currentIndex);

            bool wouldCurrentCardTakeDamage = WouldCurrentCardTakeDamage(obf, currentIndex, currentCard);
            bool isVitalitySmaller = currentCard.Vitality < cardBefore.Vitality;

            isCurrentTargetBetterThanPrevious = wouldCurrentCardTakeDamage && isVitalitySmaller;
        }
        else
        {
            // Then currentIndex corresponds to an empty spot, so it's certainly not better than previousIndex
            isCurrentTargetBetterThanPrevious = false;
        }

        return isCurrentTargetBetterThanPrevious;
    }
    private bool WouldCurrentCardTakeDamage(Battlefield obf, int currentIndex, Card currentCard)
    {
        bool hasDamageReduction = obf.IsThereACardInFrontOf(currentIndex) ||
                                    currentCard.HasHeavyArmorSkill();
        bool damageWouldBeZero = attackerPower <= 1 && hasDamageReduction;
        return !damageWouldBeZero;
    }

    private interface iAttackersChooser
    {
        public void PopulateAttackersLists(EnemyAI enemyAI, List<Card> attackerCards, List<int> attackerIndexes);
    }
    private class StrongestAttackersChooser : iAttackersChooser
    {
        EnemyAI enemyAI;

        public void PopulateAttackersLists(EnemyAI enemyAI, List<Card> attackerCards, List<int> attackerIndexes)
        {
            this.enemyAI = enemyAI;

            List<Card> allEnemyCards = GetAllEnemyCardsOnBattlefield();
            
            allEnemyCards.Sort(CompareCardsByCurrentPower);

            AddMostPowerfulCardsToAttackerCardsList(cardsSortedByPower: allEnemyCards, attackerCards);

            PopulateAttackerIndexesList(attackerCards, attackerIndexes);
        }
        private List<Card> GetAllEnemyCardsOnBattlefield()
        {
            Battlefield enemyBattlefield = enemyAI.enemyBattlefield;
            List<Card> allEnemyCardsOnBattlefield = new List<Card>();
            for (int c = 0; c < enemyBattlefield.GetSize(); c++)
            {
                Card card = enemyBattlefield.GetReferenceToCardAtOrGetNull(c);
                if (card != null)
                {
                    allEnemyCardsOnBattlefield.Add(card);
                }
            }
            return allEnemyCardsOnBattlefield;
        }
        /// <summary>
        /// Compare cards considering the AttackPower and if they CanAttack()
        /// </summary>
        private int CompareCardsByCurrentPower(Card cardA, Card cardB)
        {
            int comparisonResult;

            const int A_IS_GREATER = +1;
            const int CARDS_ARE_EQUAL = 0;
            const int B_IS_GREATER = -1;

            if ( (!cardA.CanAttack()) && (!cardB.CanAttack()) )
            {
                comparisonResult = CARDS_ARE_EQUAL;
            }
            else if (!cardA.CanAttack())
            {
                comparisonResult = B_IS_GREATER;
            }
            else if (!cardB.CanAttack())
            {
                comparisonResult = A_IS_GREATER;
            }
            else if (cardA.AttackPower == cardB.AttackPower)
            {
                comparisonResult = CARDS_ARE_EQUAL;
            }
            else if (cardA.AttackPower > cardB.AttackPower)
            {
                comparisonResult = A_IS_GREATER;
            }
            else if (cardA.AttackPower < cardB.AttackPower)
            {
                comparisonResult = B_IS_GREATER;
            }
            else
            {
                L.ogWarning("[EnemyAI] Couldn't calculate what is the better card among " + cardA.name + " and " + cardB.name, this);
                comparisonResult = CARDS_ARE_EQUAL;
            }

            return comparisonResult;
        }
        private void AddMostPowerfulCardsToAttackerCardsList(List<Card> cardsSortedByPower, List<Card> attackerCardsList)
        {
            int k = 0;
            while ((k < enemyAI.maxAmountOfAttacks) && (k < cardsSortedByPower.Count))
            {
                Card card = cardsSortedByPower[(cardsSortedByPower.Count-1)-k];
                if (card != null && card.CanAttack())
                {
                    attackerCardsList.Add(card);
                }
                k++;
            }
        }
        private void PopulateAttackerIndexesList(List<Card> attackerCards, List<int> attackerIndexes)
        {
            Battlefield enemyBattlefield = this.enemyAI.enemyBattlefield;
            for (int a = 0; a < attackerCards.Count; a++)
            {
                Card attackerCard = attackerCards[a];
                for (int c = 0; c < enemyBattlefield.GetSize(); c++)
                {
                    if (attackerCard == enemyBattlefield.GetReferenceToCardAtOrGetNull(c))
                    {
                        attackerIndexes.Add(c);
                    }
                }
            }
        }
    }
    private class OnlyTheStrongestAttackersChooser : iAttackersChooser
    {
        /// <summary>
        /// Choose only the strongest card as an attacker.
        /// </summary>
        public void PopulateAttackersLists(EnemyAI enemyAI, List<Card> attackerCards, List<int> attackerIndexes)
        {
            Battlefield enemyBattlefield = enemyAI.enemyBattlefield;

            // Get only the highest attack card to attack
            Card currentAttackerOrNull = enemyBattlefield.GetReferenceToCardAtOrGetNull(0);
            int currentAttackerIndex = 0;
            for (int i = 1; i < enemyBattlefield.GetSize(); i++)
            {
                Card iteractionCard = enemyBattlefield.GetReferenceToCardAtOrGetNull(i);
                bool isIteractionCardABetterAttackerThanTheCurrentOne =
                    // We shouldn't change if the iteraction card is null. In this case, we mantain the current.
                    (iteractionCard != null) &&
                    // Otherwise, we can change the currentAttackerOrNull if it is null...
                    (currentAttackerOrNull == null ||
                    // Also change it if currentAttackerOrNull can't attack and next is not null
                    (!currentAttackerOrNull.CanAttack() && iteractionCard != null) ||
                    // We can also change if the iteractionCard CanAttack() and it is more powerful than the current one (greatest attack power).
                    (iteractionCard.CanAttack() && currentAttackerOrNull.AttackPower < iteractionCard.AttackPower));
                if (isIteractionCardABetterAttackerThanTheCurrentOne)
                {
                    currentAttackerOrNull = iteractionCard;
                    currentAttackerIndex = i;
                }
            }
            attackerCards.Add(currentAttackerOrNull);
            attackerIndexes.Add(currentAttackerIndex);
        }
    }
    // NOTE: this class is not currently in use.
    private class AllCardsAttackersChooser : iAttackersChooser
    {
        /// <summary>
        /// Choose all possible cards as attackers.
        /// </summary>
        public void PopulateAttackersLists(EnemyAI enemyAI, List<Card> attackerCards, List<int> attackerIndexes)
        {
            Battlefield enemyBattlefield = enemyAI.enemyBattlefield;

            for (int i = 0; i < enemyBattlefield.GetSize(); i++)
            {
                Card card = enemyBattlefield.GetReferenceToCardAtOrGetNull(i);
                if (card != null && !card.Freezing)
                {
                    attackerCards.Add(card);
                    attackerIndexes.Add(i);
                }
            }
        }
    }
    #endregion
}