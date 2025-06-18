using System.Collections;
using UnityEngine;

public class DeckBuildingButtons : OpenersSuperclass
{
    [SerializeField]
    private PreMadeAudioFactory preMadeAudioFactory = null;

    [SerializeField]
    private GameObject deckSection = null;
    [SerializeField]
    private GameObject storeSection = null;

    public void OnDrinkBtnClicked()
    {
        preMadeAudioFactory.CreateDrinkAudioRequest(gameObject).RequestPlaying();

        storeSection.SetActive(!storeSection.activeSelf);
        deckSection.SetActive(!deckSection.activeSelf);
    }

    public void OnSaveAndQuitBtnClicked()
    {
        StartCoroutine(SaveAndQuitCoroutine());
    }
    private IEnumerator SaveAndQuitCoroutine()
    {
        openerOfPopUpsMadeInEditor.SetLoadingPopUpActiveToTrueAndDeactivateTips();
        yield return null;
        CardsLevel.PrepareForSaving();
        sceneOpener.OpenMapScene();
    }
}
