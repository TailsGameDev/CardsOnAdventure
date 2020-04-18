using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMap : PopUpOpener
{
    [SerializeField]
    private Spot[] finalSpotForEachMap = null;

    [SerializeField]
    private SceneOpener sceneOpener = null;

    [SerializeField]
    private AudioRequisitor audioRequisitor = null;

    [SerializeField]
    private AudioClip winSound = null;

    private static bool startOfMatch = true;

    public static bool StartOfMatch { set => startOfMatch = value; }

    Dictionary<string, Spot.SpotInfo> maps;

    private void Awake()
    {
        maps = GetMapsFromCacheOrGetNull();

        if (maps == null || startOfMatch)
        {
            BuildAllMaps();
        }
        else
        {
            RecoverSavedMaps();
        }

        MapsRuntimeCache.Instance.CacheMaps(maps);
    }

    private Dictionary<string, Spot.SpotInfo> GetMapsFromCacheOrGetNull()
    {
        MapsCache cache = MapsRuntimeCache.Instance;

        if (cache == null)
        {
            return null;
        }
        else
        {
            return cache.RetrieveMapsOrGetNull();
        }
    }

    private void BuildAllMaps()
    {
        startOfMatch = false;
        maps = new Dictionary<string, Spot.SpotInfo>();

        foreach (Spot mapFinalSpot in finalSpotForEachMap)
        {
            BuildMapAndSaveInCollection(mapFinalSpot, maps);
        }
    }

    private void BuildMapAndSaveInCollection(Spot mapFinalSpot, Dictionary<string, Spot.SpotInfo> maps)
    {
        string mapName = mapFinalSpot.MapName;
        if (!maps.ContainsKey(mapName))
        {
            mapFinalSpot.BuildFromZero();
            maps[mapName] = mapFinalSpot.GetInfo();
        }
    }

    public void RecoverSavedMaps()
    {
        foreach (Spot spot in finalSpotForEachMap)
        {
            spot.BuildFromInfo(maps[spot.MapName]);
        }
    }

    void GoToMenu()
    {
        sceneOpener.OpenScene("Main Menu");
        popUpOpener.CloseAllPopUpsExceptLoading();
    }

    public void OnEndOfGameClicked(Transform btnTransform)
    {
        audioRequisitor.RequestBGM(winSound);
        customPopUpOpener.Open("You Beat the game!!!", "You are Awesome!", "Go to Menu", "Look the Map", GoToMenu);
        //ClearSpot(btnTransform);
        //BattleDefaultBehaviour();
    }

    #region On Battle Spot Buttons Clicked

    public void OnSimpleBattleClicked(Transform btnTransform)
        {
            popUpOpener.SetLoadingPopUpActiveToTrue();
            BattleInfo.PrepareSimpleBattle();
            ClearSpotAndSaveInfo(btnTransform);
            DeckPrototypeFactory.PrepareRandomDeckForTheEnemy();
            SetUpBattleAndOpenIt();
        }

    public void OnToughBattleClicked(Transform btnTransform)
    {
        popUpOpener.SetLoadingPopUpActiveToTrue();
        BattleInfo.PrepareToughBattle();
        ClearSpotAndSaveInfo(btnTransform);
        DeckPrototypeFactory.PrepareToughRandomDeckForTheEnemy();
        SetUpBattleAndOpenIt();
    }

    // the btnTransform parameter might be used if we substitute the final spot for some battle
    public void OnBossBattleClicked(Transform btnTransform)
    {
        popUpOpener.SetLoadingPopUpActiveToTrue();
        BattleInfo.PrepareBossBattle();
        ClearSpotAndSaveInfo(btnTransform);
        DeckPrototypeFactory.PrepareBossRandomDeckForTheEnemy();
        SetUpBattleAndOpenIt();
    }

    #endregion

    #region On Master Buttons Clicked

    public void OnMageMasterClicked(Transform btnTransform)
    {
        popUpOpener.SetLoadingPopUpActiveToTrue();
        BattleInfo.PrepareMasterBattle(Classes.MAGE);
        DeckPrototypeFactory.PrepareMageDeckForTheEnemy();
        ClearSpotAndSaveInfo(btnTransform);
        SetUpBattleAndOpenIt();
    }

    public void OnWarriorMasterClicked(Transform btnTransform)
    {
        popUpOpener.SetLoadingPopUpActiveToTrue();
        BattleInfo.PrepareMasterBattle(Classes.WARRIOR);
        DeckPrototypeFactory.PrepareWarriorDeckForTheEnemy();
        ClearSpotAndSaveInfo(btnTransform);
        SetUpBattleAndOpenIt();
    }

    public void OnRougueMasterClicked(Transform btnTransform)
    {
        popUpOpener.SetLoadingPopUpActiveToTrue();
        BattleInfo.PrepareMasterBattle(Classes.ROGUE);
        DeckPrototypeFactory.PrepareRogueDeckForTheEnemy();
        ClearSpotAndSaveInfo(btnTransform);
        SetUpBattleAndOpenIt();
    }

    public void OnGuardianMasterClicked(Transform btnTransform)
    {
        popUpOpener.SetLoadingPopUpActiveToTrue();
        BattleInfo.PrepareMasterBattle(Classes.GUARDIAN);
        DeckPrototypeFactory.PrepareGuardianDeckForTheEnemy();
        ClearSpotAndSaveInfo(btnTransform);
        SetUpBattleAndOpenIt();
    }

    #endregion

    private void SetUpBattleAndOpenIt()
    {
        DeckPrototypeFactory.PrepareRandomDeckForThePlayer();
        popUpOpener.CloseAllPopUpsExceptLoading();
        sceneOpener.OpenScene("Battle");
    }

    private void ClearSpotAndSaveInfo(Transform spotBtnTransform)
    {
        Spot spot = FindAndClearSpotInItsGameObject(spotBtnTransform);

        SetClearedInPersistenceDataStructure(spot);
    }

    private Spot FindAndClearSpotInItsGameObject(Transform btnTransform)
    {
        Spot spot = btnTransform.parent.GetComponent<Spot>();

        if (spot == null)
        {
            Debug.LogError("[UIMap] spot is null.", this);
        }

        spot.Cleared = true;

        return spot;
    }

    private void SetClearedInPersistenceDataStructure(Spot spot)
    {
        Spot.SpotInfo rootInfo = maps[spot.MapName];

        Spot.SpotInfo desiredInfo = rootInfo.GetInfoFromTreeOrGetNull(spot.gameObject.name);

        desiredInfo.Cleared = true;
    }

    public void OnPauseMenuOppenerBtnClick()
    {
        popUpOpener.OpenPausePopUp();
    }
}
