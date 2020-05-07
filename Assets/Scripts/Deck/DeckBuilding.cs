using System.Collections;
using UnityEngine;

public class DeckBuilding : OpenersSuperclass
{
    [SerializeField]
    private CardsCollection cardsCollection = null;

    [SerializeField]
    private DeckCardHolder deck = null;

    void Update()
    {
        if (cardsCollection.UnavailableCardWasSelected())
        {
            ClearAllSelections();
            return;
        }

        if (deck.SomeIndexWasSelected() && cardsCollection.SelectionIsCleared())
        {
            ClearAllSelections();
            return;
        }

        bool selectedCardFromCollection = cardsCollection.AvailableCardWasSelected();
        bool selectedCardFromDeck = deck.SomeIndexWasSelected();

        if (selectedCardFromCollection && selectedCardFromDeck)
        {
            GiveBackCardOfDeckToTheCollection();
            PlaceTheCardOfCollectionInTheDeck();
            ClearAllSelections();
        }
    }

    private void PlaceTheCardOfCollectionInTheDeck()
    {
        Card cardToPlaceInTheDeck = cardsCollection.GetCloneOfSelectedCardAndManageState();
        deck.PutCardInSelectedIndex(cardToPlaceInTheDeck, smooth: false);
    }

    private void GiveBackCardOfDeckToTheCollection()
    {
        Card cardToGiveBackToTheCollection = deck.RemoveCardFromSelectedIndex();
        cardsCollection.GiveCardBack(cardToGiveBackToTheCollection);
    }

    private void ClearAllSelections()
    {
        cardsCollection.ClearSelection();
        deck.ClearSelection();
    }

    public void OnSaveAndQuitBtnClicked()
    {
        customPopUpOpener.OpenConfirmationRequestPopUp
            (
                warningMessage: "Once you leave this Tavern, your current party will remain the" +
                " same until you enter another Tavern.",
                onConfirm: SaveAndQuit
            );
    }

    private void SaveAndQuit()
    {
        StartCoroutine(SaveAndQuitCoroutine());
    }

    private IEnumerator SaveAndQuitCoroutine()
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
        yield return null;
        Card[] cards = deck.GetCards();
        DeckPrototypeFactory.PrepareManuallyBuiltDeckForThePlayerAndSaveInStorage(cards);
        sceneOpener.OpenMapScene();
    }
}