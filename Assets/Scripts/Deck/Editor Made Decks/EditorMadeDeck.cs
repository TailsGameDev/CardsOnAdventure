using System.Collections.Generic;
using UnityEngine;

public class EditorMadeDeck : MonoBehaviour
{
    // Let the deck name be the game object's name.

    protected static Dictionary<string, EditorMadeDeck> allEditorMadeDecks = null;

    [SerializeField]
    protected List<Card> cards = new List<Card>();

    [System.Serializable]
    public struct Buff
    {
        public Classes classe;
        public int atkBuff;
        public int vitBuff;
    }
    [SerializeField]
    protected Buff[] buffs;

    [SerializeField]
    private int initialHP = 0;

    public Buff[] Buffs { get => buffs; }
    public int InitialHP { get => initialHP; }

    protected void Start()
    {
        if (allEditorMadeDecks == null)
        {
            allEditorMadeDecks = new Dictionary<string, EditorMadeDeck>();
        }

        if (!allEditorMadeDecks.ContainsKey(gameObject.name))
        {
            allEditorMadeDecks.Add(gameObject.name, this);
        }
        // TODO: understand why even destroying CardManagers on Awake, it was necessary to check for
        // previous keys in the dicionary.
    }

    public List<Card> GetCardPrototypes()
    {
        return cards;
    }

    public static EditorMadeDeck GetDeckByName(string deckName)
    {
        return allEditorMadeDecks[deckName];
    }
}

public class EditorMadeDeckBuilder : DeckBuilderSuperclass
{
    private Card[] cardPrototypes;
    private EditorMadeDeck.Buff[] buffs;
    private int initialHP;

    public static EditorMadeDeckBuilder CreateEditorMadeDeckBuilder(string deckName)
    {
        EditorMadeDeck madeDeck = EditorMadeDeck.GetDeckByName(deckName);
        Card[] cards = madeDeck.GetCardPrototypes().ToArray();
        EditorMadeDeckBuilder deckBuilder = new EditorMadeDeckBuilder(cards.Length, cards, madeDeck.Buffs);
        deckBuilder.initialHP = madeDeck.InitialHP;
        return deckBuilder;
    }

    private EditorMadeDeckBuilder(int size, Card[] cards, EditorMadeDeck.Buff[] buffs) : base(size)
    {
        this.cardPrototypes = cards;
        this.buffs = buffs;
    }

    public override Card[] GetDeck()
    {
        Card[] cards = new Card[cardPrototypes.Length];


        for (int i = 0; i < cards.Length; i++)
        {
            cards[i] = cardPrototypes[i].GetClone();
        }

        Dictionary<Classes, EditorMadeDeck.Buff> buffsDictionary = new Dictionary<Classes, EditorMadeDeck.Buff>();
        for (int i = 0; i < buffs.Length; i++)
        {
            buffsDictionary.Add(buffs[i].classe, buffs[i]);
        }

        for (int i = 0; i < cards.Length; i++)
        {
            if (buffsDictionary.ContainsKey(cards[i].Classe))
            {
                EditorMadeDeck.Buff buff = buffsDictionary[cards[i].Classe];
                cards[i].ApplyEditorMadeDeckBuffAndRefreshUI(buff.atkBuff, buff.vitBuff, levelBuff: 0);
            }
        }

        return cards;
    }

    public int[] GetAmountForEachCardPrototype()
    {
        return FindTheAmountForEachCard(cardPrototypes);
    }

    public override int GetInitialHP()
    {
        return initialHP;
    }
}
