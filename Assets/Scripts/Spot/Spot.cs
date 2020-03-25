using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spot : MonoBehaviour
{
    public bool logIterations;

    private static Spot lastChoosedSpot;

    [SerializeField]
    protected Button playLvlBtn;

    [SerializeField]
    protected Button[] possiblePlayLvlBtns;

    [SerializeField]
    private Spot[] antecessorSpots = null;

    [SerializeField]
    private bool cleared = false;

    public bool Cleared { get => cleared; set => cleared = value; }

    public void ResetMap()
    {
        DemolishDownTheTree();
        BuildMapDownTheTree();
        LockIfNeededDownTheTree();
    }

    public void UpdateMap()
    {
        LockIfNeededDownTheTree();
    }

    public void BuildMapDownTheTree()
    {
        Build();

        foreach (Spot antecessor in antecessorSpots)
        {
            antecessor.BuildMapDownTheTree();
        }
    }

    private void Build()
    {
        if (playLvlBtn == null)
        {
            int randomIndex = Random.Range(0, possiblePlayLvlBtns.Length);
            playLvlBtn = Instantiate(possiblePlayLvlBtns[randomIndex]);
            ChildMaker.AdoptTeleportAndScale(transform, playLvlBtn.GetComponent<RectTransform>());
        }
    }

    private void DemolishDownTheTree()
    {
        if (playLvlBtn != null)
        {
            Destroy(playLvlBtn.gameObject);
            playLvlBtn = null;
        }

        cleared = false;

        foreach (Spot antecessor in antecessorSpots)
        {
            antecessor.DemolishDownTheTree();
        }
    }

    public void LockIfNeededDownTheTree()
    {
        playLvlBtn.enabled = IsThereAClearedPathToThisSpot() && !cleared;

        foreach (Spot antecessor in antecessorSpots)
        {
            antecessor.LockIfNeededDownTheTree();
        }
    }

    private bool PathToThisSpotIsBlocked()
    {
        return ! IsThereAClearedPathToThisSpot();
    }

    private bool IsThereAClearedPathToThisSpot()
    {
        bool thereIsAClearedPathToThisSpot;

        if (antecessorSpots == null || antecessorSpots.Length == 0)
        {
            thereIsAClearedPathToThisSpot = true;
            if (logIterations)
            {
                Debug.LogError("antecessorSpots == null", this);
            }
        }
        else
        {
            thereIsAClearedPathToThisSpot = false;
            foreach (Spot antecessor in antecessorSpots)
            {
                if (antecessor.Cleared)
                {
                    thereIsAClearedPathToThisSpot = true;
                    break;
                }
            }
        }

        if (logIterations)
        {
            Debug.LogError("[Spot]There is a cleared path to this spot: "+thereIsAClearedPathToThisSpot, this);
        }

        return thereIsAClearedPathToThisSpot;
    }
}
