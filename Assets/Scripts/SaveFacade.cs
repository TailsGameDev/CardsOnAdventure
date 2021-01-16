public class SaveFacade
{
    private MapsPersistence mapsPersistence = new MapsPersistence();
    private ClassesPersistence classesPersistence = new ClassesPersistence();
    private DeckPersistence deckPersistence = new DeckPersistence("FirstDeck");
    private DeckPersistence cardsCollectionPersistence = new DeckPersistence("CardsCollection");

    // Needed for saving.
    private static string[] nameOfMapsToSave;
    private static MapSerializable[] dataOfMapsToSave;
    private static ClassesSerializable classesSerializableToSave;
    private static DeckSerializable deckSerializableToSave;
    private static DeckSerializable cardsCollectionToSave;

    // Needed for loading.
    private static string[] namesOfMapsToLoad;

    // Results of loading
    private static MapSerializable[] loadedMapsInfo;
    private static ClassesSerializable loadedClassesSerializable;
    private static DeckSerializable loadedDeckSerializable;
    private static DeckSerializable loadedCardsCollection;

    public bool DoesAnySaveExist()
    {
        return mapsPersistence.DoesMapSaveExist("First");
    }

    public void PrepareMapsForSaving(string[] mapNames, MapSerializable[] mapsInfo)
    {
        nameOfMapsToSave = mapNames;
        dataOfMapsToSave = mapsInfo;
    }
    public void PrepareClassesBonusesForSaving( ClassesSerializable classesInfo )
    {
        classesSerializableToSave = classesInfo;
    }
    public void PrepareDeckForSaving(DeckSerializable deckSerializableParam)
    {
        deckSerializableToSave = deckSerializableParam;
        // Because DeckPrototypeFactory checks if deck is loaded. And from this moment on we can say it is.
        loadedDeckSerializable = deckSerializableParam;
    }
    public void PrepareCardsCollectionForSaving(DeckSerializable cardsCollectionSerializableParam)
    {
        cardsCollectionToSave = cardsCollectionSerializableParam;
        loadedCardsCollection = cardsCollectionSerializableParam;
    }

    public void PrepareMapsForLoading(string[] mapNames)
    {
        namesOfMapsToLoad = mapNames;
    }

    public void LoadEverything()
    {
        if (DoesAnySaveExist())
        {
            loadedMapsInfo = mapsPersistence.LoadAllMaps(namesOfMapsToLoad);
            loadedClassesSerializable = classesPersistence.LoadClasses();
            loadedDeckSerializable = deckPersistence.Load();
            loadedCardsCollection = cardsCollectionPersistence.Load();

            classesSerializableToSave = loadedClassesSerializable;
            deckSerializableToSave = loadedDeckSerializable;
        }
        else
        {
            L.ogError("LoadEverything was called, but any save exist on storage", this);
        }
    }
    
    public MapSerializable[] GetLoadedMapsInfo()
    {
        if (loadedMapsInfo == null)
        {
            L.ogError("loadedMapsInfo is null!! LoadAll method should be called first!", this);
        }
        return loadedMapsInfo;
    }
    public ClassesSerializable GetLoadedClasses()
    {
        if (loadedClassesSerializable == null)
        {
            L.ogError("loadedMapsInfo is null!! LoadAll method should be called first!", this);
        }
        return loadedClassesSerializable;
    }
    public DeckSerializable GetLoadedDeck()
    {
        return loadedDeckSerializable;
    }
    public DeckSerializable GetLoadedCardsCollection()
    {
        return loadedCardsCollection;
    }

    public bool IsDeckLoaded()
    {
        return loadedDeckSerializable != null;
    }
    public bool IsCardsCollectionLoaded()
    {
        return loadedCardsCollection != null;
    }

    public void SaveEverything()
    {
        /*
        if ( SafeToSave() )
        {
            mapsPersistence.SaveAllMaps(nameOfMapsToSave, dataOfMapsToSave);
            classesPersistence.SaveClasses(classesSerializableToSave);
            deckPersistence.Save(deckSerializableToSave);
            if (cardsCollectionToSave == null)
            {
                cardsCollectionToSave = new DeckSerializable(DeckPrototypeFactory.GetCardsCollectionAmounts());
            }
            cardsCollectionPersistence.Save(cardsCollectionToSave);
        }
        else
        {
            L.ogWarning("SaveEverything was called, but at least one attribute is still null. " +
                "This is ok at the start of the game", this);
        }
        */
    }
    private bool SafeToSave()
    {
        return  dataOfMapsToSave != null && 
                nameOfMapsToSave != null && 
                classesSerializableToSave != null &&
                deckSerializableToSave != null;
                // cardsCollectionToSave is optional
    }
}
