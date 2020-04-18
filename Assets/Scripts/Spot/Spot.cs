using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Spot : MonoBehaviour
{
    [SerializeField]
    private string mapName = "";

    [SerializeField]
    protected Button playLvlBtn;

    [SerializeField]
    protected Button[] possiblePlayLvlBtns;
    protected int playLvlBtnIndex;

    [SerializeField]
    private Spot[] antecessors = new Spot[0];

    private static float disabledColorAlpha = 0.5f;

    private bool shouldBeExpanding = false;

    private Vector3 originalScale;

    private bool cleared = false;
    private bool visited = false;

    float t = 1;
    bool isGrowing = true;

    public bool Cleared { set => cleared = value; }
    public string MapName { get => mapName; }

    public class SpotInfo
    {
        public string GOName;
        public bool Cleared;
        public int PlayLvlBtnIndex;
        public SpotInfo[] antecessors;

        public SpotInfo(string goName, bool cleared, int playLvlBtn, SpotInfo[] antecessors)
        {
            GOName = goName;
            Cleared = cleared;
            PlayLvlBtnIndex = playLvlBtn;
            this.antecessors = antecessors;
        }

        public SpotInfo GetInfoFromTreeOrGetNull(string desiredName)
        {
            L.ogWarning("GetInfoFromTreeOrGetNull: this.GOName: "+GOName+". desiredName: "+desiredName, this);
            if ( GOName.Equals(desiredName) )
            {
                return this;
            }
            else
            {
                foreach (SpotInfo antecessor in antecessors)
                {
                    if (antecessor == null)
                    {
                        if (GOName != "Simple Initial (1)")
                        {
                            L.ogError(GOName+" antecessor is null in GetInfoFromTreeOrGetNull", this);
                        }
                    }
                    else
                    {
                        SpotInfo desired = antecessor.GetInfoFromTreeOrGetNull(desiredName);
                        if (desired != null)
                        {
                            return desired;
                        }
                    }
                }
            }
            return null;
        }
    }

    private void Awake()
    {
        GetComponent<Image>().enabled = false;
        originalScale = transform.localScale;
    }


    private void Update()
    {
        if (shouldBeExpanding)
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

    public void BuildFromZero()
    {
        BuildMapDownTheTree();

        LockIfNeededDownTheTree();
        HighlightOrObfuscateDownTheTree();
    }

    public void BuildFromInfo(SpotInfo spotInfo)
    {
        BuildFromInfoDownTheTree(spotInfo);

        LockIfNeededDownTheTree();
        HighlightOrObfuscateDownTheTree();
    }

    private void BuildFromInfoDownTheTree(SpotInfo spotInfo)
    {
        visited = true;

        // L.ogWarning(spotInfo.GOName + " <- GOName // myName -> " + gameObject.name, this);

        if (spotInfo != null)
        {
            cleared = spotInfo.Cleared;
            MakePlayLvlBtnFrom(possiblePlayLvlBtns[spotInfo.PlayLvlBtnIndex]);

            for (int a = 0; a < antecessors.Length; a++)
            {
                if ( ! antecessors[a].visited )
                {
                    antecessors[a].BuildFromInfoDownTheTree(spotInfo.antecessors[a]);
                }
            }
        }
    }

    public void BuildMapDownTheTree()
    {
        Build();

        foreach (Spot antecessor in antecessors)
        {
            antecessor.BuildMapDownTheTree();
        }
    }

    private void Build()
    {
        if (playLvlBtn == null)
        {
            playLvlBtnIndex = UnityEngine.Random.Range(0, possiblePlayLvlBtns.Length);
            MakePlayLvlBtnFrom(possiblePlayLvlBtns[playLvlBtnIndex]);
        }
    }

    private void MakePlayLvlBtnFrom(Button btn)
    {
        playLvlBtn = Instantiate(btn);
        ChildMaker.AdoptTeleportAndScale(transform, playLvlBtn.GetComponent<RectTransform>());
    }

    public void LockIfNeededDownTheTree()
    {
        playLvlBtn.enabled = IsThereAClearedPathToThisSpot() && !cleared;

        foreach (Spot antecessor in antecessors)
        {
            antecessor.LockIfNeededDownTheTree();
        }
    }

    private bool IsThereAClearedPathToThisSpot()
    {
        bool thereIsAClearedPathToThisSpot;

        if (antecessors.Length == 0)
        {
            thereIsAClearedPathToThisSpot = true;
        }
        else
        {
            thereIsAClearedPathToThisSpot = false;
            foreach (Spot antecessor in antecessors)
            {
                if (antecessor.cleared)
                {
                    thereIsAClearedPathToThisSpot = true;
                    break;
                }
            }
        }

        return thereIsAClearedPathToThisSpot;
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

        shouldBeExpanding = playLvlBtn.enabled;

        foreach (Spot antecessor in antecessors)
        {
            antecessor.HighlightOrObfuscateDownTheTree();
        }
    }

    // private static List<SpotInfo> treeNodes;

    public SpotInfo GetInfo()
    {
        // treeNodes = new List<SpotInfo>();
        SpotInfo treeInfo = GetInfoDownTheTree();
        return treeInfo;
    }

    private SpotInfo GetInfoDownTheTree()
    {
        visited = true;

        SpotInfo[] antecessorsInfo = new SpotInfo[antecessors.Length];

        for (int a = 0; a < antecessors.Length; a++)
        {
            if ( !antecessors[a].visited )
            {
                antecessorsInfo[a] = antecessors[a].GetInfoDownTheTree();
            }
        }

        SpotInfo thisSpotInfo = new SpotInfo(gameObject.name, cleared, playLvlBtnIndex, antecessorsInfo);

        // L.ogError(gameObject.name, this);
        // treeNodes.Add(thisSpotInfo);

        return thisSpotInfo;
    }

    private void ClearVisitedDownTheTree()
    {
        visited = false;
        foreach (Spot antecessor in antecessors)
        {
            ClearVisitedDownTheTree();
        }
    }

    public static void LogInfo(SpotInfo spt)
    {
        if ( spt != null)
        {
            Debug.Log("[Spot] "+spt.GOName+"; btnIndex: "+spt.PlayLvlBtnIndex+"; cleared: "+spt.Cleared+"; antecessors.Length: "+spt.antecessors.Length);
            if (spt.antecessors != null)
            {
                foreach (SpotInfo antess in spt.antecessors)
                {
                    LogInfo(antess);
                }
            }
        }
    }

    public static void LogInfoWarning(SpotInfo spt)
    {
        if (spt != null)
        {
            string antecessors = spt.antecessors == null ? "null" : "length: " + spt.antecessors.Length;
            Debug.LogWarning("[Spot] "+spt.GOName + "; btnIndex: " + spt.PlayLvlBtnIndex + "; cleared: " + spt.Cleared + "; antecessors.Length: " + spt.antecessors.Length);
            {
                foreach (SpotInfo antess in spt.antecessors)
                {
                    LogInfoWarning(antess);
                }
            }
        }
    }
}