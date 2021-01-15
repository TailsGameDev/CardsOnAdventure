using UnityEngine;
using UnityEngine.UI;

public class DynamicSizeScrollableCardHolder : CardsHolder
{
    [SerializeField]
    private RectTransform slotPrototype = null;

    [SerializeField]
    private RectTransform scrollableBackground = null;

    protected RectTransform[] slots = null;

    protected int amountOfSlots;

    #region Initialization
    protected void InitializeSlotsAndRectSize(int amountOfSlots)
    {
        this.amountOfSlots = amountOfSlots;
        SetHorizontalSizeOfRect();
        InstantiateSlots();
        PopulateCardPositionsArray();
    }
    private void SetHorizontalSizeOfRect()
    {
        float sizeXOfASlot = slotPrototype.sizeDelta.x;
        float spaceBetweenSlots = GetComponent<HorizontalLayoutGroup>().spacing;

        float sizeX = amountOfSlots * (spaceBetweenSlots + sizeXOfASlot);

        scrollableBackground.sizeDelta = new Vector2(sizeX, scrollableBackground.sizeDelta.y);
    }
    private void InstantiateSlots()
    {
        slotPrototype.GetComponent<CardReceptor>().CardsHolder = this;

        slots = new RectTransform[amountOfSlots];

        for (int i = 0; i < amountOfSlots; i++)
        {
            slots[i] = Instantiate(slotPrototype);
            slots[i].SetParent(transform, false);
            slots[i].GetComponent<CardReceptor>().Index = i;
        }
    }
    private void PopulateCardPositionsArray()
    {
        cardPositions = new RectTransform[amountOfSlots];
        for (int i = 0; i < amountOfSlots; i++)
        {
            cardPositions[i] = slots[i].GetComponent<RectTransform>();
        }
    }
    #endregion

    protected void Clear()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                Destroy(slots[i].gameObject);
                slots[i] = null;
            }
        }
        slots = null;
        amountOfSlots = -1;
    }
}