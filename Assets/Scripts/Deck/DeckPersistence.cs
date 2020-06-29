using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckPersistence : Persistence
{
    
    private string fileName;
    
    public DeckPersistence(string fileName)
    {
        this.fileName = fileName;
    }

    public void Save(DeckSerializable deckSerializable)
    {
        GenericSave(fileName, deckSerializable);
    }

    public bool SaveExists()
    {
        return base.DoesSaveExist(fileName);
    }

    public DeckSerializable Load()
    {
        if (SaveExists())
        {
            return GenericLoad<DeckSerializable>(fileName);
        }
        else
        {
            L.ogError(this, "Trying to load inexistent files");
            return new DeckSerializable(DeckPrototypeFactory.GetArrayFilledWithTheRandomCardIndex());
        }
    }
}
