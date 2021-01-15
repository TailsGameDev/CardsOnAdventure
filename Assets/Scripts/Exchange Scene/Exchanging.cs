using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exchanging : DeckBuilding
{
    protected override IEnumerator SaveAndQuitCoroutine()
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrueAndDeactivateTips();
        yield return null;
        (new SaveFacade()).PrepareCardsCollectionForSaving(new DeckSerializable(cardsCollection.AmountOfEachCard));
        sceneOpener.OpenMapScene();
    }
}
