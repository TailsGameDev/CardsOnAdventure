using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
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

    private static float disabledColorAlpha = 0.5f;

    private bool isExpanding = false;

    private Vector3 originalScale;

    [SerializeField]
    private bool cleared = false;

    public bool Cleared { get => cleared; set => cleared = value; }

    private void Awake()
    {
        GetComponent<Image>().enabled = false;
        originalScale = transform.localScale;
    }

    float t = 1;
    bool isGrowing = true;
    private void Update()
    {
        if (isExpanding)
        {
            if (isGrowing)
            {
                t += Time.deltaTime;
                if (t > 2)
                {
                    t = 1.99f;
                    isGrowing = false;
                }
            }
            else
            {
                t -= Time.deltaTime;
                if (t < 1)
                {
                    t = 1.01f;
                    isGrowing = true;
                }
            }
            transform.localScale = originalScale * t;
        }

    }

    public void ResetMap()
    {
        DemolishDownTheTree();
        BuildMapDownTheTree();
        LockIfNeededDownTheTree();
        HighlightOrObfuscateDownTheTree();
    }

    public void UpdateMap()
    {
        LockIfNeededDownTheTree();
        HighlightOrObfuscateDownTheTree();
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
            int randomIndex = UnityEngine.Random.Range(0, possiblePlayLvlBtns.Length);
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

    private void HighlightOrObfuscateDownTheTree()
    {
        Image btnImg = playLvlBtn.GetComponent<Image>();
        Color btnColor = btnImg.color;

        if (playLvlBtn.enabled)
        {
            const float ENABLED_COLOR_ALPHA = 1.0f;
            btnImg.color = new Color(btnColor.r, btnColor.g, btnColor.b, ENABLED_COLOR_ALPHA);
        }
        else
        {
            btnImg.color = new Color(btnColor.r, btnColor.g, btnColor.b, disabledColorAlpha);
            transform.localScale = Vector3.one;
        }

        isExpanding = playLvlBtn.enabled;

        foreach (Spot antecessor in antecessorSpots)
        {
            antecessor.HighlightOrObfuscateDownTheTree();
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
            foreach (Spot antecessor in antecessorSpots)
            {
                if (antecessor.Cleared)
                {
                    thereIsAClearedPathToThisSpot = true;
                    break;
                }
            }
        }

        return thereIsAClearedPathToThisSpot;
    }
}
