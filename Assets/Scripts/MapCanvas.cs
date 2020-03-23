using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCanvas : MonoBehaviour
{
    public static MapCanvas instance;

    [SerializeField]
    private Spot finalSpot = null;

    private void Awake()
    {
        if( instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        finalSpot.MakeAndLockIfNeededDownTheTree();
    }

    public void OnSimpleBattleClicked()
    {

    }

    public void OnDifficultBattleClicked()
    {

    }

    public void OnBossBattleClicked()
    {

    }

    public void OnMasterClicked(Classes masterType)
    {
        Debug.LogWarning("[MapCanvas] OnMasterClicked "+masterType, this);
    }
}
