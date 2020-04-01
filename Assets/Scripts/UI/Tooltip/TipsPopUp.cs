using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsPopUp : MonoBehaviour
{
    [SerializeField]
    GameObject tooltipSectionModel = null;

    GameObject[] tooltipSections = null;

    [SerializeField]
    Transform verticalLayoutGroup = null;

    public void Populate( TooltipSectionData[] sections )
    {
        if (this.tooltipSections != null)
        {
            foreach (GameObject section in this.tooltipSections)
            {
                Destroy(section);
            }
        }

        this.tooltipSections = new GameObject[sections.Length];

        float totalHeight = 0;

        for (int i = 0; i < sections.Length; i++)
        {
            GameObject newSection = Instantiate(tooltipSectionModel);
            newSection.GetComponent<TooltipSection>().PopulateSection(sections[i]);
            newSection.transform.SetParent(verticalLayoutGroup, false);
            tooltipSections[i] = newSection;
            newSection.SetActive(true);
            RectTransform sectionRectTransform = newSection.GetComponent<RectTransform>();
            Vector2 sizeDelta = sectionRectTransform.sizeDelta;
            sizeDelta.y = sections[i].height;
            sectionRectTransform.sizeDelta = sizeDelta;
            totalHeight += sizeDelta.y;
        }

        RectTransform layoutRectTransform = verticalLayoutGroup.GetComponent<RectTransform>();
        Vector2 layoutSizeDelta = layoutRectTransform.sizeDelta;
        layoutSizeDelta.y = totalHeight;
        layoutRectTransform.sizeDelta = layoutSizeDelta;
    }
}
