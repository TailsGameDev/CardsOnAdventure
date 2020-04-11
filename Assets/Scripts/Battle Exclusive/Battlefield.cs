using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battlefield : CardsHolder
{

    public int GetSize()
    {
        return cards.Length;
    }

    public bool IsSlotIndexFree(int slotIndex)
    {
        return cards[slotIndex] == null;
    }

    public bool IsSlotIndexOccupied(int index)
    {
        return cards[index] != null;
    }

    public void PlaceCardInSelectedIndex(Card card)
    {
        card.Battlefield = this;
        PutCardInIndex(card, GetSelectedIndex());
    }

    public void SelectFirstFreeIndex()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null)
            {
                SetSelectedIndex(i);
                break;
            }
        }
    }

    public bool HasEmptySlot()
    {
        bool has = false;
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null)
            {
                has = true;
                break;
            }
        }
        return has;
    }

    public Card GetReferenceToSelectedCard()
    {
        return GetReferenceToCardAt(GetSelectedIndex());
    }

    public Card GetReferenceToCardAt(int index)
    {
        if (cards[index] == null)
        {
            Debug.LogError("[Battlefield] trying to get reference to a null card.", this);
        }

        return cards[index];
    }

    public Card GetReferenceToCardAtOrGetNull(int index)
    {
        return cards[index];
    }

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

    internal void Remove(object card)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            #pragma warning disable CS0253 // Possível comparação de referência inesperada; o lado direito precisa de conversão
            if (cards[i] == card)
            #pragma warning restore CS0253 // Possível comparação de referência inesperada; o lado direito precisa de conversão
            {
                cards[i] = null;
            }
        }
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

    public Card GetSelectedCard()
    {
        return cards[GetSelectedIndex()];
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

    public void SwapCards(int index, int anotherIndex)
    {
        Card aux = GetReferenceToCardAt(anotherIndex);

        PutCardInIndex(cards[index], anotherIndex);

        PutCardInIndex(aux, index);
    }

    public void RemoveFreezingStateFromAllCards()
    {
        for (int i =0; i < cards.Length; i++)
        {
            if (ContainsCardInIndex(i))
            {
                cards[i].RemoveFreezing();
            }
        }
    }

    public void LoopThrougEnemyesAndSelectBestTarget(EnemyAI.CurrentTargetIsBetterThanTheOneBefore isCurrentTargetBetter, int attackPower)
    {
        int selected = 0;

        while (!ContainsCardInIndex(selected))
        {
            selected++;
            if (selected == cards.Length)
            {
                break;
            }
        }

        for (int indexBefore = 0; indexBefore < cards.Length-1; indexBefore++)
        {
            int currentIndex = indexBefore + 1;
            if (ContainsCardInIndex(indexBefore) && ContainsCardInIndex(currentIndex))
            {
                if (isCurrentTargetBetter(indexBefore, currentIndex, attackPower, this))
                {
                    selected = currentIndex;
                }
            }
        }

        SetSelectedIndex(selected);
    }

    public void SelectCardIndexWithLowestVitality()
    {
        int lowest = 999999;
        int lowestVitalityIndex = 0;
        for (int i = 0; i < cards.Length; i++)
        {
            if (ContainsCardInIndex(i))
            {
                int vitalityForThisIndex = GetReferenceToCardAt(i).Vitality;
                if ( vitalityForThisIndex < lowest)
                {
                    lowestVitalityIndex = i;
                    lowest = vitalityForThisIndex;
                }
            }
        }
        SetSelectedIndex( lowestVitalityIndex );
    }

    public void RemoveObfuscateFromAllCards()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] != null)
            {
                cards[i].SetObfuscate(false);
            }
        }
    }

    public void BuffAllCardsAttackPowerForThisMatch()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] != null)
            {
                cards[i].BuffAttackPowerForThisMatch();
            }
        }
    }
}