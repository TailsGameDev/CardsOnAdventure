using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuilding : PopUpOpener
{
    [SerializeField]
    private CardsCollection cardsCollection = null;

    [SerializeField]
    private DeckCardHolder deck = null;

    [SerializeField]
    private SceneOpener sceneOpener = null;

    void Update()
    {
        if (cardsCollection.SelectedUnavailableCard())
        {
            ClearAllSelections();
            return;
        }

        if (deck.SomeIndexWasSelected() && cardsCollection.AnyIndexWasSelected())
        {
            ClearAllSelections();
            return;
        }

        bool selectedCardFromCollection = cardsCollection.SelectedAvailableCard();
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
        Card cardToPlaceInTheDeck = cardsCollection.GetCloneOfSelectedCard();
        deck.PutCardInSelectedIndex(cardToPlaceInTheDeck);
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
        popUpOpener.SetLoadingPopUpActiveToTrue();
        Card[] cards = deck.GetCards();
        DeckPrototypeFactory.PrepareManuallyBuiltDeckForThePlayer(cards);
        sceneOpener.OpenMapScene();
    }
}