using System.Collections;
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

    public Buff[] Buffs { get => buffs; }

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

public class EditorMadeDeckBuilder : DeckPrototypeFactory.DeckBuilder
{
    private Card[] cardPrototypes;
    private EditorMadeDeck.Buff[] buffs;

    public static EditorMadeDeckBuilder CreateEditorMadeDeckBuilder(string deckName)
    {
        EditorMadeDeck madeDeck = EditorMadeDeck.GetDeckByName(deckName);
        Card[] cards = madeDeck.GetCardPrototypes().ToArray();
        return new EditorMadeDeckBuilder(cards.Length, cards, madeDeck.Buffs);
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
            if (cardPrototypes[i] == null)
            {
                Debug.LogError(i);
            }
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
                cards[i].AttackPower += buff.atkBuff;
                cards[i].InconditionalHealing(buff.vitBuff);
            }
        }

        return cards;
    }

    public int[] GetIndexOfEachCardPrototype()
    {
        return FindThePrototypeIndexForEachCard(cardPrototypes);
    }
}
