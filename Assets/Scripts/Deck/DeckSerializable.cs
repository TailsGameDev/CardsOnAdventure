[System.Serializable]
public class DeckSerializable
{
    public int[] cardsIndexes;

    public DeckSerializable(int[] cardsIndexes)
    {
        this.cardsIndexes = cardsIndexes;
    }

    public int[] GetCardsIndexes()
    {
        return cardsIndexes;
    }
}
