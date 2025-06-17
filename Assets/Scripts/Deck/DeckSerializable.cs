[System.Serializable]
public class DeckSerializable
{
    public int[] cardAmounts;

    public DeckSerializable(int[] cardAmounts)
    {
        this.cardAmounts = cardAmounts;
    }

    public int[] GetCardsAmounts()
    {
        return cardAmounts;
    }
}
