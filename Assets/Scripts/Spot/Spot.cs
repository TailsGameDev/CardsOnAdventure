using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spot : MonoBehaviour
{
    private static Spot lastChoosedSpot;

    [SerializeField]
    protected Button[] possiblePlayLvlBtns;

    [SerializeField]
    private Spot[] antecessorSpots = null;

    protected Button playLvlBtn;

    private bool cleared;

    protected bool Cleared { get => cleared; set => cleared = value; }

    public void MakeAndLockIfNeededDownTheTree()
    {
        Make();

        if (PathToThisSpotIsBlocked())
        {
            LockSpot();
        }

        foreach (Spot antecessor in antecessorSpots)
        {
            antecessor.MakeAndLockIfNeededDownTheTree();
        }
    }

    private void Make()
    {
        if (playLvlBtn == null)
        {
            int randomIndex = Random.Range(0, possiblePlayLvlBtns.Length);
            playLvlBtn = Instantiate(possiblePlayLvlBtns[randomIndex]);
            ChildMaker.AdoptAndTeleport(transform, playLvlBtn.GetComponent<RectTransform>());
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
        }
        else
        {
            thereIsAClearedPathToThisSpot = false;
            for (int i = 0; i < antecessorSpots.Length; i++)
            {
                if (antecessorSpots[i].Cleared)
                {
                    thereIsAClearedPathToThisSpot = true;
                }
            }
        }

        return thereIsAClearedPathToThisSpot;
    }

    private void LockSpot()
    {
        playLvlBtn.enabled = false;
    }
}
