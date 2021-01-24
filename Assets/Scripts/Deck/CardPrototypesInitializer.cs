using System.Collections.Generic;
using UnityEngine;

public class CardPrototypesInitializer : MonoBehaviour
{
    private static CardPrototypesInitializer deckPrototypeFactory;

    [SerializeField]
    protected Card theRandomCard;

    private static PrototypesInitializer prototypesInitializer;

    #region Initialization
    private void Awake()
    {
        BecomeSingleton();
    }
    private void BecomeSingleton()
    {
        if (deckPrototypeFactory == null)
        {
            deckPrototypeFactory = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeIfNeeded();
    }
    private static void InitializeIfNeeded()
    {
        if (prototypesInitializer == null)
        {
            prototypesInitializer = new PrototypesInitializer();
            Card theRandomCard = deckPrototypeFactory.theRandomCard;
            prototypesInitializer.PopulateCardDataStructures(theRandomCard);
        }
    }
    
    // This method belongs to this class because it may initialize the data structures
    public static int GetAmountOfCardPrototypes()
    {
        InitializeIfNeeded();
        return prototypesInitializer.GetAmountOfCardPrototypes();
    }

    #endregion

    private class PrototypesInitializer : CardPrototypesAccessor
    {
        public void PopulateCardDataStructures(Card theRandomCard)
        {
            CardPrototypesAccessor.theRandomCardPrototype = theRandomCard;
            PopulateArrayOfAllCardPrototypes();
            PopulateArrayOfNotMonsterPrototypes();
        }
        private void PopulateArrayOfAllCardPrototypes()
        {
            Classes[] classes = (Classes[])System.Enum.GetValues(typeof(Classes));

            List<Card> allCardPrototypesList = new List<Card>();

            // Starts at 1 so it won't take the Classes.NOT_A_CLASS key
            for (int i = 1; i < classes.Length; i++)
            {
                Card[] cardsOfClass = ClassInfo.GetCardsOfClass(classes[i]);
                allCardPrototypesList.AddRange(cardsOfClass);
            }

            allCardPrototypes = allCardPrototypesList.ToArray();
        }
        private void PopulateArrayOfNotMonsterPrototypes()
        {
            List<Card>notMonsterPrototypesList = new List<Card>();
            for (int i = 0; i < allCardPrototypes.Length; i++)
            {
                if (allCardPrototypes[i].Classe != Classes.MONSTER)
                {
                    notMonsterPrototypesList.Add(allCardPrototypes[i]);
                }
            }
            notMonsterPrototypes = notMonsterPrototypesList.ToArray();
        }
    }
}

