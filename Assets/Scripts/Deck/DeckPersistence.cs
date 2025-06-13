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

    public bool HasSave()
    {
        return base.HasSave(fileName);
    }

    public DeckSerializable Load()
    {
        if (HasSave())
        {
            return GenericLoad<DeckSerializable>(fileName);
        }
        else
        {
            L.ogError(this, "Trying to load inexistent files");
            return new DeckSerializable(DeckBuilderSuperclass.GetArrayFilledWithTheRandomCardIndex()); ;
        }
    }
}
