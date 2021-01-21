using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsLevel : MonoBehaviour
{
    private static CardsLevel instance;
    private SaveFacade saveFacade;
    private int[] cardsLevel;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            saveFacade = new SaveFacade();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (cardsLevel == null)
        {
            Clear();
        }
    }

    public int GetLevelOfCard(Card card)
    {
        int index = CardPrototypesAccessor.FindIndexOnPrototypesArray(card);
        if (index < 0 || index > cardsLevel.Length)
        {
            print("trouble"+index+card.name);
        }
        int cardLevel = cardsLevel[index];
        return cardLevel;
    }

    public static void Clear()
    {
        int size = CardPrototypesInitializer.GetAmountOfCardPrototypes();
        instance.cardsLevel = new int[size];
    }

    public static void PrepareForSaving()
    {
        DeckSerializable cardsLevelSerializable = new DeckSerializable(instance.cardsLevel);
        instance.saveFacade.PrepareCardsLevelForSaving(cardsLevelSerializable);
    }
    public static void CopyLoadedDataToAttributes()
    {
        DeckSerializable cardsLevelSerializable = instance.saveFacade.GetLoadedCardsLevel();
        instance.cardsLevel = cardsLevelSerializable.cardsIndexes;
    }

    public void LevelUpCard(Card card)
    {
        int cardIndex = CardPrototypesAccessor.FindIndexOnPrototypesArray(card);
        cardsLevel[cardIndex]++;
        CardPrototypesAccessor.UpdatePrototypesLevel();
    }
}
