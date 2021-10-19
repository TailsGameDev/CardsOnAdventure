using System.Collections;

public class DeckCardHolder : DynamicSizeScrollableCardHolder
{
    private bool initialized = false;

    #region Initialization
    private void Start()
    {
        StartCoroutine(DelayedStart());
        initialized = true;
    }
    private IEnumerator DelayedStart()
    {
        // Wait for the DeckPrototypeFactory to PopulateArrayOfAllCardPrototypes.
        yield return null;
        int amountOfSlots = DeckBuilderSuperclass.DEFAULT_DECK_SIZE;
        InitializeSlotsRectSizeAndTransformWrapper(amountOfSlots);
        InitializeCards(amountOfSlots);
    }
    private void InitializeCards(int amountOfSlots)
    {
        cards = PlayerAndEnemyDeckHolder.GetPreparedCardsForThePlayerWithTheRandomCards();

        for (int i = 0; i < amountOfSlots; i++)
        {
            PutCardInIndexWithSmoothMovement(cards[i], i);

            ChildMaker.CopySizeDelta(slotWrappers[i].GetRectTransform(), cards[i].GetRectTransform());
        }
    }
    #endregion

    public void PrepareDeckForPlayerAndGetReadyForSaving()
    {
        if (initialized)
        {
            PlayerAndEnemyDeckHolder.PrepareManuallyBuiltDeckForThePlayerAndGetReadyForSaving(cards);
        }
    }
}