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

    private TransformWrapper transformWrapper;

    private const int CODE_TO_STOP = int.MaxValue;

    public TransformWrapper TransformWrapper 
    {
        get 
        {
            // NOTE: Doing lazy initialization because my brain is really tired. If you find a way to do it one time only great!
            if (transformWrapper == null)
            {
                transformWrapper = new TransformWrapper(transform);
            }
            return transformWrapper;
        }
    }

    public void SwapCards(int index, int anotherIndex)
    {
        Card aux = GetReferenceToCardAt(anotherIndex);

        PutCardInIndexWithSmoothMovement(cards[index], anotherIndex);

        PutCardInIndexWithSmoothMovement(aux, index);
    }

    public void LoopThrougCardsAndSelectBestTarget(EnemyAI.CurrentTargetIsBetterThanTheOneBefore isCurrentTargetBetter)
    {
        int iterator = GetFirstOccupiedIndex();
        int selected = iterator;

        int k = 10;
        while (iterator != CODE_TO_STOP && k > 0)
        {
            k--;
            if (isCurrentTargetBetter(indexBefore: selected, currentIndex: iterator, this))
            {
                selected = iterator;
            }
            // NOTE: the order of the iteration acctualy does not matter, because if
            // 'isCurrentTargetBetter' is called in all cards, the best should be found
            iterator = GetNextIndexToAttackOrGetCodeToStop(iterator);
        }

        if (k < 0)
        {
            // Obs: this log was never triggered.
            L.ogError(this, "k avoided an infinite loop. Please review the loop logic");
        }

        SetSelectedIndex(selected);
    }
    private int GetNextIndexToAttackOrGetCodeToStop(int current)
    {
        int MaybeTheNext = GetNextIndexInVerticalOrderOrGetCodeToStop(current: current);
        if (MaybeTheNext == CODE_TO_STOP || cards[MaybeTheNext] != null)
        {
            return MaybeTheNext;
        }
        else
        {
            return GetNextIndexInVerticalOrderOrGetCodeToStop(MaybeTheNext);
        }
    }
    private int GetNextIndexInVerticalOrderOrGetCodeToStop(int current)
    {
        int next;
        switch (current)
        {
            case 0: next = 2; break;
            case 1: next = 3; break;
            case 2: next = 1; break;
            case 3: next = CODE_TO_STOP; break;
            default:
                L.ogError("GetNextIndexToAttack called, but current is out of bounds", this);
                next = CODE_TO_STOP;
                break;
        }
        return next;
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
    public void MakeProtectionEvidentOnSelectedIfNeeded(bool attackerIgnoresBlock)
    {
        int selectedIndex = GetSelectedIndex();
        Card cardInFrontOfTarget = GetCardInFrontOf(selectedIndex);
        bool attackWillBeBlocked = cardInFrontOfTarget!= null && cardInFrontOfTarget.HasBlockSkill();
        if (IsThereACardInFrontOf(selectedIndex) && !attackerIgnoresBlock && !attackWillBeBlocked)
        {
            StartCoroutine(MakeProtectionEvident(selectedIndex));
            cards[selectedIndex].MakeProtectionEvident();
        }
    }
    private IEnumerator MakeProtectionEvident(int selectedIndex)
    {
        Transform protection = protectionVFXtoEachCard[selectedIndex].transform;
        Vector3 originalScale = protection.localScale;
        Vector3 targetScale = originalScale + new Vector3(increaseScaleValueInProtectionAnimation,
                                                        increaseScaleValueInProtectionAnimation,0.0f);
        while (protection.localScale.x < targetScale.x)
        {
            float t = TimeFacade.DeltaTime * increaseScaleSpeedMultiplier;
            protection.localScale += new Vector3(t,t,t);
            yield return null;
        }

        while (protection.localScale.x > originalScale.x)
        {
            float t = TimeFacade.DeltaTime * increaseScaleSpeedMultiplier;
            protection.localScale -= new Vector3(t, t, t);
            yield return null;
        }
    }
    #endregion
}