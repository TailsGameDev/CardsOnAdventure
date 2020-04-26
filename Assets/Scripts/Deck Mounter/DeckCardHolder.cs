using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckCardHolder : DynamicSizeScrollableCardHolder
{
    private void Start()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        // Wait for the DeckPrototypeFactory to PopulateArrayOfAllCardPrototypes.
        yield return null;
        int amountOfSlots = DeckPrototypeFactory.DefaultDeckSize;
        InitializeSlotsAndRectSize(amountOfSlots);
    }
}
