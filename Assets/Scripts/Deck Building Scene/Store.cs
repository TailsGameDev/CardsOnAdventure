using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    [SerializeField]
    private CardsCollectionDisplayer cardsCollection = null;

    [SerializeField]
    private CardsHolder storeCardHolder = null;

    [SerializeField]
    private Text priceText = null;
    [SerializeField]
    private Text amountOfCardText = null;

    private Card cardToImprove;
    private void OnEnable()
    {
        if (cardToImprove != null)
        {
            UpdateUI(cardToImprove);
        }
    }
    private IEnumerator Start()
    {
        // Wait for the DeckPrototypeFactory to PopulateArrayOfAllCardPrototypes.
        yield return null;
        // Wait for CardsCollectionDisplayer to populate it's attributes
        yield return null;

        cardToImprove = cardsCollection.GetReferenceToCardAt(0).GetClone();
        cardToImprove.RefreshStatsForThePlayer();
        UpdateUI(cardToImprove);
    }
    private void UpdateUI(Card cardToImprove)
    {
        storeCardHolder.PutCardInIndexThenTeleportToSlot(cardToImprove, 0);

        priceText.text = GetPriceOfCard(cardToImprove).ToString();

        amountOfCardText.text = cardsCollection.GetAmountOfCardNotCurrentlyInDeck(cardToImprove).ToString();
    }
    private int GetPriceOfCard(Card card)
    {
        return card.GetLevel() + 2;
    }

    void Update()
    {
        if (cardsCollection.SomeIndexWasSelected() && storeCardHolder.SomeIndexWasSelected())
        {
            // Update card to improve
            Destroy(cardToImprove.gameObject);
            cardToImprove = cardsCollection.GetSelectedCard().GetClone();
            cardToImprove.RefreshStatsForThePlayer();

            UpdateUI(cardToImprove);

            cardsCollection.ClearSelection();
            storeCardHolder.ClearSelection();
        }
    }

    public void OnImproveBtnClicked()
    {
        int cardAmount = cardsCollection.GetAmountOfCardNotCurrentlyInDeck(cardToImprove);
        int price = GetPriceOfCard(cardToImprove);
        if (cardAmount >= price)
        {
            // Subtract 1 from price to simulate we gave him a card after the upgrade
            cardsCollection.RemoveAmountOfCardFromCollection(cardToImprove, price - 1);
            cardToImprove.LevelUp();
            UpdateUI(cardToImprove);
            cardsCollection.UpdateColorAndAmountTextOfCard(cardToImprove);
        }
    }
}
