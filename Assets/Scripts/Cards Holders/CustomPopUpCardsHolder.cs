using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPopUpCardsHolder : DynamicSizeScrollableCardHolder
{
    [SerializeField]
    private GameObject node = null;

    public void ShowCardsOfClass(Classes classe)
    {
        node.SetActive(true);
        cards = OneOfEachClassDeckBuilder.Create(classe).GetDeck();
        base.InitializeSlotsAndRectSize(cards.Length);

        MoveAndBuffCards(cards);

        UICardsHolderEventHandler.inputEnabled = false;
    }

    private void MoveAndBuffCards(Card[] cards)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            PutCardInIndexWithSmoothMovement(cards[i], i);
            cards[i].GetCardDragAndDrop().ForceReceptorToNullBeforeDrop = true;
            cards[i].ApplyPlayerBonuses();
        }
    }

    public void ShowCards(Card[] cards)
    {
        node.SetActive(true);
        this.cards = cards;
        base.InitializeSlotsAndRectSize(cards.Length);

        MoveAndBuffCards(cards);

        UICardsHolderEventHandler.inputEnabled = false;
    }

    public void ClearAttributes()
    {
        UICardsHolderEventHandler.inputEnabled = true;

        for (int i = 0; i < cards.Length; i++)
        {
            Destroy(cards[i].gameObject);
            cards[i] = null;
        }
        cards = null;

        base.Clear();

        node.SetActive(false);
    }
}
