using UnityEngine;
using System.Collections;

public class CardsCollection : DynamicSizeScrollableCardHolder
{
    private void Start()
    {
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        // Wait for the DeckPrototypeFactory to PopulateArrayOfAllCardPrototypes.
        yield return null;
        cards = DeckPrototypeFactory.GetCopyOfAllAndEachCardPrototype();

        InitializeSlotsAndRectSize(amountOfSlots: cards.Length);

        yield return PopulateSlotsWithCards();
    }

    private IEnumerator PopulateSlotsWithCards()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            // It's not necessary to wait a frame. Just makes an effect of each card being placed, instead of all in once
            yield return null;
            ChildMaker.AdoptTeleportAndScale(slots[i], cards[i].GetComponent<RectTransform>());
        }
    }
}