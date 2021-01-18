using System.Collections;

public class DeckCardHolder : DynamicSizeScrollableCardHolder
{
    #region Initialization
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
        InitializeCards(amountOfSlots);
    }
    private void InitializeCards(int amountOfSlots)
    {
        cards = DeckPrototypeFactory.GetPreparedCardsForThePlayerWithTheRandomCards();

        for (int i = 0; i < amountOfSlots; i++)
        {
            PutCardInIndexWithSmoothMovement(cards[i], i);

            ChildMaker.CopySizeDelta(slots[i], cards[i].GetRectTransform());
        }
    }
    #endregion

    public void SaveInPersistence()
    {
        DeckPrototypeFactory.PrepareManuallyBuiltDeckForThePlayerAndSaveInStorage(cards);
    }
}