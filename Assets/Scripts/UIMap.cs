using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMap : PopUpOpener
{
    [SerializeField]
    private Spot finalSpot = null;

    [SerializeField]
    private PauseMenu sceneOpener = null;

    [SerializeField]
    private GameObject mapPopUp = null;

    public void ResetMap()
    {
        finalSpot.ResetMap();
    }

    public void UpdateMap()
    {
        finalSpot.UpdateMap();
    }

    #region On Button Clicks
    public void OnSimpleBattleClicked(Transform btnTransform)
    {
        ClearSpot(btnTransform);
        BattleDefaultBehaviour();
    }

    public void OnDifficultBattleClicked(Transform btnTransform)
    {
        ClearSpot(btnTransform);
        BattleDefaultBehaviour();
    }

    public void OnBossBattleClicked(Transform btnTransform)
    {
        ClearSpot(btnTransform);
        BattleDefaultBehaviour();
    }

    public void OnMageMasterClicked(Transform btnTransform)
    {
        ClearSpot(btnTransform);
        BattleDefaultBehaviour();
    }

    public void OnWarriorMasterClicked(Transform btnTransform)
    {
        ClearSpot(btnTransform);
        BattleDefaultBehaviour();
    }

    public void OnRougueMasterClicked(Transform btnTransform)
    {
        ClearSpot(btnTransform);
        BattleDefaultBehaviour();
    }

    public void OnGuardianMasterClicked(Transform btnTransform)
    {
        ClearSpot(btnTransform);
        BattleDefaultBehaviour();
    }

    #endregion

    private void BattleDefaultBehaviour()
    {
        sceneOpener.OpenScene("Battle");
        mapPopUp.SetActive(false);
    }

    void ClearSpot(Transform btnTransform)
    {
        Spot spot = btnTransform.parent.GetComponent<Spot>();
        if (spot == null)
        {
            Debug.LogError("[UIMap] spot is null.", this);
        }
        spot.Cleared = true;
    }

    public void OnPauseMenuOppenerBtnClick()
    {
        popUpOpener.OpenPausePopUp();
    }
}
