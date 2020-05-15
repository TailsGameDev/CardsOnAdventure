using System.Collections;
using UnityEngine;

public class DeckBuilding : OpenersSuperclass
{
    [SerializeField]
    private CardsCollection cardsCollection = null;

    [SerializeField]
    private DeckCardHolder deck = null;

    [SerializeField]
    private PreMadeAudioFactory preMadeAudioFactory = null;

    private static bool shouldAskTip = true;

    private void Start()
    {
        if (shouldAskTip)
        {
            shouldAskTip = false;
            TipDragAndDrop.AskToUseTips();
        }
    }

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
            preMadeAudioFactory.CreateRandomPlaceCardAudioRequest(gameObject).RequestPlaying();
            ClearAllSelections();
        }
    }

    private void PlaceTheCardOfCollectionInTheDeck()
    {
        Card cardToPlaceInTheDeck = cardsCollection.GetCloneOfSelectedCardAndReduceAmountInDeck();
        deck.PutCardInSelectedIndexThenTeleportToSlot(cardToPlaceInTheDeck);
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

    public void OnDrinkBtnClicked()
    {
        preMadeAudioFactory.CreateDrinkAudioRequest(gameObject).RequestPlaying();
    }

    public void OnSaveAndQuitBtnClicked()
    {
        SaveAndQuit();
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