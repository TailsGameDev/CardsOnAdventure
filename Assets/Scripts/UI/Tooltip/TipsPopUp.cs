using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsPopUp : MonoBehaviour
{
    [SerializeField]
    GameObject tooltipSectionModel = null;

    TipsPopUpSection[] storedSections = null;

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
        float defaultHeight = tooltipSectionModel.GetComponent<RectTransform>().sizeDelta.y;

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

        this.storedSections = new TipsPopUpSection[tipsData.Length];
    }

    private void DestroySectionsFromLastCallIfThereAreAny()
    {
        if (this.storedSections != null)
        {
            foreach (TipsPopUpSection section in this.storedSections)
            {
                Destroy(section.gameObject);
            }
        }
    }

    private void MakeAndStoreSection(int i) 
    {
        TipsPopUpSection newSection = CloneDefaultSection();

        ConfigureSection(newSection, tipsData[i]);

        storedSections[i] = newSection;
    }

    private TipsPopUpSection CloneDefaultSection()
    {
        return Instantiate(tooltipSectionModel).GetComponent<TipsPopUpSection>();
    }

    private void ConfigureSection(TipsPopUpSection newSection, TipSectionData sectionData)
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
