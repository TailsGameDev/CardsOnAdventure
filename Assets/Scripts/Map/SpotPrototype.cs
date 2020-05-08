using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class SpotPrototype : OpenersSuperclass
{
    [SerializeField]
    private Color backgroundColor = Color.white;

    [SerializeField]
    private UIMap uiMap = null;

    [SerializeField]
    private Classes deckClass = Classes.NOT_A_CLASS;

    [SerializeField]
    private float deckSizeMultiplier = 0;

    [SerializeField]
    private AudioClip battleBGM = null;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GoToSpot()
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrue();
        if (deckClass == Classes.NOT_A_CLASS)
        {
            CurrentBattleInfo.PrepareBattle(backgroundColor, bgmParam: battleBGM);
            DeckPrototypeFactory.PrepareModifiedSizeRandomDeckForTheEnemy(deckSizeMultiplier);
        }
        else
        {
            CurrentBattleInfo.PrepareBattle(deckClass, battleBGM);
            DeckPrototypeFactory.PrepareClassDeckForTheEnemy(deckSizeMultiplier, deckClass);
        }
        Spot spot = transform.parent.GetComponent<Spot>();
        uiMap.SetSpotInfoToClearIfPlayerSucceed(spot);
        sceneOpener.OpenBattle();
    }
}
