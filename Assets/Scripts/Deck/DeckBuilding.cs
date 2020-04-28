using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuilding : MonoBehaviour
{
    [SerializeField]
    private CardsCollection cardsCollection = null;

    [SerializeField]
    private DeckCardHolder deckCardHolder = null;

    [SerializeField]
    private CardsHolder garbage = null;

    [SerializeField]
    private SceneOpener sceneOpener = null;

    void Update()
    {
        bool selectedCardFromCollection = cardsCollection.SelectedIndexIsOcupied();
        bool selectedSlotFromDeckHolder = deckCardHolder.SelectedIndexIsFree();

        bool selectedCardFromDeckHolder = deckCardHolder.SelectedIndexIsOcupied();

        if (selectedCardFromCollection && selectedCardFromDeckHolder)
        {
            Card collectionCard = cardsCollection.GetReferenceToSelectedCardOrGetNull();
            Card cloneOfCollectionCard = Instantiate(collectionCard);

            Card garbageCard = deckCardHolder.RemoveCardFromSelectedIndex();
            garbage.PutCardInIndex(garbageCard, 0);

            deckCardHolder.PutCardInSelectedIndex(cloneOfCollectionCard);

            cardsCollection.ClearSelection();
            deckCardHolder.ClearSelection();
        }
    }

    public void SaveAndQuit()
    {
        Card[] cards = deckCardHolder.GetCards();
        DeckPrototypeFactory.PrepareManuallyBuiltDeckForThePlayer(cards);
        sceneOpener.OpenMapScene();
    }
}