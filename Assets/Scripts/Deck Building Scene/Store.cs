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

    private IEnumerator Start()
    {
        // Wait for the DeckPrototypeFactory to PopulateArrayOfAllCardPrototypes.
        yield return null;
        // Wait for CardsCollectionDisplayer to populate it's attributes
        yield return null;

        cardToImprove = cardsCollection.GetReferenceToCardAt(0).GetClone();
        UpdateUI(cardToImprove);
    }
    private void UpdateUI(Card cardToImprove)
    {
        storeCardHolder.PutCardInIndexWithSmoothMovement(cardToImprove, 0);

        priceText.text = GetPriceOfCard(cardToImprove).ToString();

        amountOfCardText.text = cardsCollection.GetAmountOfCardNotCurrentlyInDeck(cardToImprove).ToString();
    }
    private int GetPriceOfCard(Card card)
    {
        // It's a Geometric Progression with the level.
        int price = 1;
        for (int level = card.GetLevel(); level >= 0; level--)
        {
            price *= 2;
        }
        return price;
    }

    void Update()
    {
        if (cardsCollection.SomeIndexWasSelected() && storeCardHolder.SomeIndexWasSelected())
        {
            // Update card to improve
            Destroy(cardToImprove.gameObject);
            cardToImprove = cardsCollection.GetSelectedCard().GetClone();

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
            cardsCollection.RemoveAmountOfCardFromCollection(cardToImprove, price);
            cardToImprove.LevelUp();
            UpdateUI(cardToImprove);
            cardsCollection.UpdateColorAndAmountTextOfCard(cardToImprove);
        }
    }
}
