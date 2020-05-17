using System.Collections.Generic;
using UnityEngine;
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

    [SerializeField]
    private bool canBeRevisited = false;

    private bool cleared = false;

    private bool visited = false;

    float t = 1;
    bool isGrowing = true;

    public bool Cleared { set => cleared = value; }
    public string MapName { get => mapName; }

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
                t += TimeFacade.DeltaTime;
                if (t > 2)
                {
                    t = 1.99f;
                    isGrowing = false;
                }
            }
            else
            {
                t -= TimeFacade.DeltaTime;
                if (t < 1)
                {
                    t = 1.01f;
                    isGrowing = true;
                }
            }
            transform.localScale = originalScale * t;
        }
    }

    public void BuildFromInfo(SpotInfo spotInfo, List<SpotInfo> allSpotsInfo)
    {
        BuildFromInfoDownTheGraph(spotInfo, allSpotsInfo);

        LockIfNeededDownTheGraph();
        HighlightOrObfuscateDownTheGraph();
        ClearVisitedDownTheGraph();
        MakeRevisitingPossibleDownTheGraph();
    }

    private void BuildFromInfoDownTheGraph(SpotInfo spotInfo, List<SpotInfo> rootAllSpotsInfo)
    {
        visited = true;

        cleared = spotInfo.Cleared;
        MakePlayLvlBtnFrom(possiblePlayLvlBtns[spotInfo.PlayLvlBtnIndex]);

        for (int a = 0; a < antecessors.Length; a++)
        {
            if ( ! antecessors[a].visited )
            {
                antecessors[a].playLvlBtnIndex = spotInfo.PlayLvlBtnIndex;

                int antecessorIndex = spotInfo.AntecessorsIndexes[a];
                SpotInfo antecessorSpotInfo = rootAllSpotsInfo[antecessorIndex];
                antecessors[a].BuildFromInfoDownTheGraph(antecessorSpotInfo, rootAllSpotsInfo);
            }
        }
    }

    public void BuildFromZero()
    {
        BuildMapDownTheGraph();

        LockIfNeededDownTheGraph();
        HighlightOrObfuscateDownTheGraph();
    }

    public void BuildMapDownTheGraph()
    {
        Build();

        foreach (Spot antecessor in antecessors)
        {
            antecessor.BuildMapDownTheGraph();
        }
    }

    private void Build()
    {
        if (playLvlBtn == null)
        {
            playLvlBtnIndex = Random.Range(0, possiblePlayLvlBtns.Length);
            MakePlayLvlBtnFrom(possiblePlayLvlBtns[playLvlBtnIndex]);
        }
    }

    private void MakePlayLvlBtnFrom(Button btn)
    {
        playLvlBtn = Instantiate(btn);
        ChildMaker.AdoptTeleportAndScale(transform, playLvlBtn.GetComponent<RectTransform>());
    }

    public void LockIfNeededDownTheGraph()
    {
        playLvlBtn.enabled = IsThereAClearedPathToThisSpot() && !cleared;

        foreach (Spot antecessor in antecessors)
        {
            antecessor.LockIfNeededDownTheGraph();
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

    private void HighlightOrObfuscateDownTheGraph()
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
            antecessor.HighlightOrObfuscateDownTheGraph();
        }
    }

    private void MakeRevisitingPossibleDownTheGraph()
    {
        visited = true;

        if (cleared && canBeRevisited)
        {
            
            Button revisitBtn = Instantiate(possiblePlayLvlBtns[playLvlBtnIndex]);
            revisitBtn.transform.SetParent( transform.parent, true );
            revisitBtn.transform.position = transform.position;
            revisitBtn.GetComponent<SpotPrototype>().BelongsToMap = false;
        }

        foreach (Spot antecessor in antecessors)
        {
            if ( ! antecessor.visited )
            {
                antecessor.MakeRevisitingPossibleDownTheGraph();
            }
        }
    }

    public List<SpotInfo> GetInfo()
    {
        return GetInfo(out int rootIndex);
    }

    public List<SpotInfo> GetInfo(out int rootIndex)
    {
        // Make allSpotsInfo
        List<SpotInfo> allSpotsInfo = new List<SpotInfo>();
        SpotInfo rootInfo = GetInfoDownTheGraph(allSpotsInfo);
        allSpotsInfo.Add(rootInfo);

        rootIndex = allSpotsInfo.Count-1;

        ClearVisitedDownTheGraph();

        return allSpotsInfo;
    }

    private SpotInfo GetInfoDownTheGraph(List<SpotInfo> rootAllSpotsColection)
    {
        visited = true;

        List<int> myAntecessorsIndexes = new List<int>();

        for (int a = 0; a < antecessors.Length; a++)
        {
            if (!antecessors[a].visited)
            {
                SpotInfo spotInfo = antecessors[a].GetInfoDownTheGraph(rootAllSpotsColection);
                myAntecessorsIndexes.Add(rootAllSpotsColection.Count);
                rootAllSpotsColection.Add(spotInfo);
            }
        }

        SpotInfo thisSpotInfo = new SpotInfo(gameObject.name, cleared, playLvlBtnIndex, myAntecessorsIndexes);

        return thisSpotInfo;
    }

    private void ClearVisitedDownTheGraph()
    {
        visited = false;
        foreach (Spot antecessor in antecessors)
        {
            antecessor.ClearVisitedDownTheGraph();
        }
    }
}