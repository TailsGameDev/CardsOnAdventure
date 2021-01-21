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
        int amountOfSlots = DeckBuilderSuperclass.DEFAULT_DECK_SIZE;
        InitializeSlotsAndRectSize(amountOfSlots);
        InitializeCards(amountOfSlots);
    }
    private void InitializeCards(int amountOfSlots)
    {
        cards = PlayerAndEnemyDeckHolder.GetPreparedCardsForThePlayerWithTheRandomCards();

        for (int i = 0; i < amountOfSlots; i++)
        {
            PutCardInIndexWithSmoothMovement(cards[i], i);

            ChildMaker.CopySizeDelta(slots[i], cards[i].GetRectTransform());
        }
    }
    #endregion

    public void PrepareDeckForPlayerAndGetReadyForSaving()
    {
        PlayerAndEnemyDeckHolder.PrepareManuallyBuiltDeckForThePlayerAndGetReadyForSaving(cards);
    }
}