using UnityEngine;

public class TipPopUp : MonoBehaviour
{
    [SerializeField]
    GameObject tooltipSectionPrototype = null;

    TipPopUpSection[] storedSections = null;

    [SerializeField]
    RectTransform verticalLayoutGroup = null;

    private TipSectionData[] tipsData;

    public void PopulateAllSections( TipSectionData[] tipsData )
    {
        this.tipsData = tipsData;

        ClearDataStructures();

        ApplyDefaultHeightIfNeeded();

        for (int i = 0; i < tipsData.Length; i++)
        {
            MakeAndStoreSection(i);
        }

        float totalHeight = 0;
        for (int i = 0; i < tipsData.Length; i++)
        {
            totalHeight += tipsData[i].height;
        }

        SetRectTransformHeight(verticalLayoutGroup, totalHeight);
    }

    private void ApplyDefaultHeightIfNeeded()
    {
        float defaultHeight = tooltipSectionPrototype.GetComponent<RectTransform>().sizeDelta.y;

        for (int i = 0; i < tipsData.Length; i++)
        {
            if (tipsData[i].height == 0)
            {
                tipsData[i].height = defaultHeight;
            }
        }
    }
    private void ClearDataStructures()
    {
        DestroySectionsFromLastCallIfThereAreAny();

        this.storedSections = new TipPopUpSection[tipsData.Length];
    }

    private void DestroySectionsFromLastCallIfThereAreAny()
    {
        if (this.storedSections != null)
        {
            foreach (TipPopUpSection section in this.storedSections)
            {
                Destroy(section.gameObject);
            }
        }
    }

    private void MakeAndStoreSection(int i) 
    {
        TipPopUpSection newSection = CloneDefaultSection();

        ConfigureSection(newSection, tipsData[i]);

        storedSections[i] = newSection;
    }
    private TipPopUpSection CloneDefaultSection()
    {
        return Instantiate(tooltipSectionPrototype).GetComponent<TipPopUpSection>();
    }
    private void ConfigureSection(TipPopUpSection newSection, TipSectionData sectionData)
    {
        newSection.PopulateSection(sectionData);
        newSection.transform.SetParent(verticalLayoutGroup, false);

        SetRectTransformHeight(newSection.GetComponent<RectTransform>(), sectionData.height);
        
        newSection.gameObject.SetActive(true);
    }
    private void SetRectTransformHeight(RectTransform rt, float height)
    {
        Vector2 sizeDelta = rt.sizeDelta;
        sizeDelta.y = height;
        rt.sizeDelta = sizeDelta;
    }
}
