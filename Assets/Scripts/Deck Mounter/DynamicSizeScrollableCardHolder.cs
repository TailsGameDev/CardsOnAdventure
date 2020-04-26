using UnityEngine;
using UnityEngine.UI;

public class DynamicSizeScrollableCardHolder : CardsHolder
{
    [SerializeField]
    private RectTransform slotPrototype = null;

    [SerializeField]
    private RectTransform scrollableBackground = null;

    protected RectTransform[] slots = null;

    protected void InitializeSlotsAndRectSize(int amountOfSlots)
    {
        SetHorizontalSizeOfRect(amountOfSlots);
        InstantiateSlots(amountOfSlots);
    }

    private void SetHorizontalSizeOfRect(int amountOfSlots)
    {
        float sizeXOfASlot = slotPrototype.sizeDelta.x;
        float spaceBetweenSlots = GetComponent<HorizontalLayoutGroup>().spacing;

        float sizeX = amountOfSlots * (spaceBetweenSlots + sizeXOfASlot);

        scrollableBackground.sizeDelta = new Vector2(sizeX, scrollableBackground.sizeDelta.y);
    }

    private void InstantiateSlots(int amountOfSlots)
    {
        slotPrototype.GetComponent<CardReceptor>().CardsHolder = this;

        slots = new RectTransform[amountOfSlots];

        for (int i = 0; i < amountOfSlots; i++)
        {
            slots[i] = Instantiate(slotPrototype);
            slots[i].SetParent(transform);
            slots[i].GetComponent<CardReceptor>().Index = i;
        }
    }
}