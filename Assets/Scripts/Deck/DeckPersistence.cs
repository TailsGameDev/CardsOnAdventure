using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckPersistence : Persistence
{

    const string FIRST_DECK_FILENAME = "FirstDeck";
    
    public void Save(DeckSerializable deckSerializable)
    {
        GenericSave(FIRST_DECK_FILENAME, deckSerializable);
    }

    public bool SaveExists()
    {
        return base.DoesSaveExist(FIRST_DECK_FILENAME);
    }

    public DeckSerializable Load()
    {
        if (SaveExists())
        {
            return GenericLoad<DeckSerializable>(FIRST_DECK_FILENAME);
        }
        else
        {
            L.ogError(this, "Trying to load inexistend deck files");
            return new DeckSerializable(DeckPrototypeFactory.GetArrayFilledWithTheRandomCardIndex());
        }
    }
}
