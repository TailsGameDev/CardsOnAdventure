using System.Collections;
using UnityEngine;

public class Battlefield : CardsHolder
{
    [SerializeField]
    private GameObject[] protectionVFXtoEachCard = null;

    [SerializeField]
    private float increaseScaleValueInProtectionAnimation = 0.2f;
    [SerializeField]
    private float increaseScaleSpeedMultiplier = 1.0f;

    const int CODE_TO_STOP = int.MaxValue;

    public void SwapCards(int index, int anotherIndex)
    {
        Card aux = GetReferenceToCardAt(anotherIndex);

        PutCardInIndexWithSmoothMovement(cards[index], anotherIndex);

        PutCardInIndexWithSmoothMovement(aux, index);
    }

    public void LoopThrougCardsAndSelectBestTarget(EnemyAI.CurrentTargetIsBetterThanPrevious isCurrentTargetBetterThanPrevious)
    {
        int previousIndex = GetFirstOccupiedIndex();
        int currentIndex = previousIndex;

        int k = 10;
        while (currentIndex != CODE_TO_STOP && k > 0)
        {
            k--;
            if (isCurrentTargetBetterThanPrevious(previousIndex, currentIndex, this))
            {
                previousIndex = currentIndex;
            }
            currentIndex = GetNextIndexToAttackOrGetCodeToStop(currentIndex);
        }

        if (k < 0)
        {
            // Obs: this log was never triggered.
            L.ogError(this, "k avoided an infinite loop. Please review the loop logic");
        }

        SetSelectedIndex(previousIndex);
    }
    private int GetNextIndexToAttackOrGetCodeToStop(int current)
    {
        return (current < (cards.Length-1)) ? (current + 1) : CODE_TO_STOP;
    }

    public void BuffAllCardsAttackPowerForThisMatch()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] != null)
            {
                cards[i].ModifyAttackPowerForThisMatch(valueToSum: 1);
            }
        }
    }

    #region Neighborhoods
    public Card GetCardInFrontOf(int index)
    {
        return cards[GetIndexInFrontOf(index)];
    }
    public int GetIndexInFrontOf(int index)
    {
        int cardIndex = index;

        switch (index)
        {
            case 2:
                cardIndex = 0;
                break;
            case 3:
                cardIndex = 1;
                break;
        }

        Debug.LogWarning("[Battlefield] index in front of "+index+" was not found. Maybe the card is not in the back line", this);

        return cardIndex;
    }
    public Card GetCardBehind(int index)
    {
        return cards[GetCardIndexBehind(index)];
    }
    public int GetCardIndexBehind(int index)
    {
        int cardIndex = index;

        switch (index)
        {
            case 0:
                cardIndex = 2;
                break;
            case 1:
                cardIndex = 3;
                break;
        }

        return cardIndex;
    }
    public Card GetCardBeside(int index)
    {
        return cards[GetCardIndexBeside(index)];
    }
    public int GetCardIndexBeside(int index)
    {
        int cardIndex = index;

        switch (index)
        {
            case 0:
                cardIndex = 1;
                break;
            case 1:
                cardIndex = 0;
                break;
            case 2:
                cardIndex = 3;
                break;
            case 3:
                cardIndex = 2;
                break;
        }

        return cardIndex;
    }
    public int GetVerticalNeighborIndex(int index)
    {
        int verticalNeighbor = index;

        switch (index)
        {
            case 0:
                verticalNeighbor = 2;
                break;
            case 1:
                verticalNeighbor = 3;
                break;
            case 2:
                verticalNeighbor = 0;
                break;
            case 3:
                verticalNeighbor = 1;
                break;
        }

        return verticalNeighbor;
    }
    public bool IsThereACardInFrontOf(int index)
    {
        bool thereIs = false;

        switch (index)
        {
            case 2:
                thereIs = cards[0] != null;
                break;
            case 3:
                thereIs = cards[1] != null;
                break;
        }

        return thereIs;
    }
    public bool IsInBackline(int index)
    {
        return IsThereACardInFrontOf(index);
    }
    #endregion

    #region Get or Select by Vitality
    public void SelectCardIndexWithLowestVitality()
    {
        int lowestVitalityIndex = GetIndexWithLowestVitality();
        SetSelectedIndex( lowestVitalityIndex );
    }
    public int GetIndexWithLowestVitalityThatCanBeHealed()
    {
        int lowest = 999999;
        int lowestVitalityIndex = 0;
        for (int i = 0; i < cards.Length; i++)
        {
            if (ContainsCardInIndex(i))
            {
                Card card = GetReferenceToCardAt(i);
                int vitalityForThisIndex = card.Vitality;
                if (vitalityForThisIndex < lowest && card.CanBeHealed())
                {
                    lowestVitalityIndex = i;
                    lowest = vitalityForThisIndex;
                }
            }
        }
        return lowestVitalityIndex;
    }
    public int GetIndexWithLowestVitality()
    {
        int lowest = 999999;
        int lowestVitalityIndex = 0;
        for (int i = 0; i < cards.Length; i++)
        {
            if (ContainsCardInIndex(i))
            {
                int vitalityForThisIndex = GetReferenceToCardAt(i).Vitality;
                if (vitalityForThisIndex < lowest)
                {
                    lowestVitalityIndex = i;
                    lowest = vitalityForThisIndex;
                }
            }
        }
        return lowestVitalityIndex;
    }
    #endregion

    #region Protection
    public void DisplayProtectionVFXOnlyofCardsInBackline()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            protectionVFXtoEachCard[i].SetActive(cards[i] != null && IsThereACardInFrontOf(i));
            if (cards[i] != null)
            {
                cards[i].SetProtectionIconActive( IsThereACardInFrontOf(i) );
            }
        }
    }
    public void HideAllProtectionVFX()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            protectionVFXtoEachCard[i].SetActive(false);
            if (cards[i] != null)
            {
                cards[i].SetProtectionIconActive(false);
            }
        }
    }
    public void MakeProtectionEvidentOnSelectedIfNeeded(bool attackerIgnoresProtection)
    {
        int selectedIndex = GetSelectedIndex();
        bool attackWillBeBlocked = IsThereACardInFrontOf(selectedIndex) && GetCardInFrontOf(selectedIndex).HasBlockSkill();
        if (IsThereACardInFrontOf(selectedIndex) && !attackerIgnoresProtection && !attackWillBeBlocked)
        {
            Transform protectionTransformWrapper = protectionVFXtoEachCard[selectedIndex].transform;
            StartCoroutine(MakeEvident(protectionTransformWrapper));
            cards[selectedIndex].MakeProtectionEvident();
        }
    }
    public void MakeSelectedCardEvident()
    {
        if (GetSelectedIndex() > 0)
        {
            Card selectedCard = GetSelectedCard();
            StartCoroutine(MakeEvident(selectedCard.TransformWrapper));
        }
    }
    private IEnumerator MakeEvident(Transform objectToDetach)
    {
        Vector3 originalScale = objectToDetach.localScale;
        Vector3 targetScale = originalScale + new Vector3(increaseScaleValueInProtectionAnimation,
                                                        increaseScaleValueInProtectionAnimation, 0.0f);

        while (objectToDetach.localScale.x < targetScale.x)
        {
            float t = TimeFacade.DeltaTime * increaseScaleSpeedMultiplier;
            objectToDetach.localScale += new Vector3(t, t, t);
            yield return null;
        }

        while (objectToDetach.localScale.x > originalScale.x)
        {
            float t = TimeFacade.DeltaTime * increaseScaleSpeedMultiplier;
            objectToDetach.localScale -= new Vector3(t, t, t);
            yield return null;
        }

        objectToDetach.localScale = originalScale;
    }
    #endregion

    #region Death Mark
    public void DetachCardsThatWouldDie(Card attackerCard)
    {
        for (int defenderIndex = 0; defenderIndex < cards.Length; defenderIndex++)
        {
            Card defenderCard = cards[defenderIndex];
            if (defenderCard != null)
            {
                bool attackerIgnoresProtection = attackerCard.IgnoresProtection;
                bool hasProtector = (!attackerIgnoresProtection) && IsThereACardInFrontOf(defenderIndex);
                bool wouldAttackBeBlocked =  hasProtector && GetCardInFrontOf(defenderIndex).HasBlockSkill();
                if (!wouldAttackBeBlocked)
                {
                    bool wouldDefenderDie;
                    int valueForAttackToOvercomeConsideringHeavyArmor = defenderCard.HasHeavyArmorSkill() ?
                                            (defenderCard.Vitality + defenderCard.Vitality) : defenderCard.Vitality;
                    if (hasProtector)
                    {
                        int valueForAttackToOvercomeConsideringProtection = valueForAttackToOvercomeConsideringHeavyArmor
                                                                        + valueForAttackToOvercomeConsideringHeavyArmor;
                        wouldDefenderDie = attackerCard.AttackPower >= valueForAttackToOvercomeConsideringProtection;
                    }
                    else
                    {
                        wouldDefenderDie = attackerCard.AttackPower >= valueForAttackToOvercomeConsideringHeavyArmor;
                    }
                    defenderCard.ShowDeathMarkVFX(show: wouldDefenderDie);
                }
            }
        }
    }
    public void ClearDeathMarks()
    {
        for (int c = 0; c < cards.Length; c++)
        {
            Card card = cards[c];
            if (card != null)
            {
                card.ShowDeathMarkVFX(show: false);
            }
        }
    }
    #endregion
}