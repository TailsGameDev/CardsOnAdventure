using System.Collections;
using UnityEngine;

public class DeckBuilding : OpenersSuperclass
{
    [SerializeField]
    protected CardsCollectionDisplayer cardsCollection = null;

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
    private void ClearAllSelections()
    {
        cardsCollection.ClearSelection();
        deck.ClearSelection();
    }
    private void GiveBackCardOfDeckToTheCollection()
    {
        Card cardToGiveBackToTheCollection = deck.RemoveCardFromSelectedIndex();
        cardsCollection.GiveCardBack(cardToGiveBackToTheCollection);
    }
    private void PlaceTheCardOfCollectionInTheDeck()
    {
        Card cardToPlaceInTheDeck = cardsCollection.GetCloneOfSelectedCardAndReduceAmountInDeck();
        deck.PutCardInSelectedIndexThenTeleportToSlot(cardToPlaceInTheDeck);

        Card selected = cardsCollection.GetSelectedCard();
        Card secondClone = selected.GetClone();

        Destroy(selected.gameObject);
        cardsCollection.PutCardInIndexThenTeleportToSlot(secondClone, cardsCollection.GetSelectedIndex());
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
    protected virtual IEnumerator SaveAndQuitCoroutine()
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrueAndDeactivateTips();
        yield return null;
        Card[] cards = deck.GetCards();
        DeckPrototypeFactory.PrepareManuallyBuiltDeckForThePlayerAndSaveInStorage(cards);
        sceneOpener.OpenMapScene();
    }
}