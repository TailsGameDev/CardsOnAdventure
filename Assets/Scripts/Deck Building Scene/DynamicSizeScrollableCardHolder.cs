using UnityEngine;
using UnityEngine.UI;

public class DynamicSizeScrollableCardHolder : CardsHolder
{
    protected class CardReceptorTransformWrapper : TransformWrapper
    {
        private CardReceptor cardReceptor;

        public CardReceptorTransformWrapper(Transform transform) : base(transform)
        {
            cardReceptor = transform.GetComponent<CardReceptor>();

            if (cardReceptor == null)
            {
                Debug.LogWarning("[DynamicSizeScrollableCardHolder] card receptor is null. I thought all would have it assigned.", transform);
            }
        }

        public CardReceptor CardReceptor { get => cardReceptor; }

        public RectTransform GetRectTransform()
        {
            // NOTE: Take RectTransform from cardReceptor so we don't need to access the transform that belongs to the superclass.
            return cardReceptor.GetComponent<RectTransform>();
        }

        public void DestroyGameObject()
        {
            // NOTE: Take gameObject from cardReceptor so we don't need to access the transform that belongs to the superclass.
            Destroy(cardReceptor.gameObject);
        }

        public void DeactivateGameObject()
        {
            // NOTE: Take gameObject from cardReceptor so we don't need to access the transform that belongs to the superclass.
            cardReceptor.gameObject.SetActive(false);
        }

        public Text GetTextInChildren()
        {
            // NOTE: Take gameObject from cardReceptor so we don't need to access the transform that belongs to the superclass.
            return cardReceptor.GetComponentInChildren<Text>();
        }
    }

    [SerializeField]
    private RectTransform slotPrototype = null;

    [SerializeField]
    private RectTransform scrollableBackground = null;

    // protected RectTransform[] slots = null;
    protected CardReceptorTransformWrapper[] slotWrappers;

    private TransformWrapper transformWrapper;

    private int amountOfSlots;

    #region Initialization
    protected void InitializeSlotsRectSizeAndTransformWrapper(int amountOfSlots)
    {
        transformWrapper = new TransformWrapper(transform);
        this.amountOfSlots = amountOfSlots;
        SetHorizontalSizeOfRect(amountOfSlots);
        InstantiateSlots();
        PopulateCardPositionsArray();
    }

    protected void SetHorizontalSizeOfRect(int amountOfSlots)
    {
        float sizeXOfASlot = slotPrototype.sizeDelta.x;
        float spaceBetweenSlots = GetComponent<HorizontalLayoutGroup>().spacing;

        float sizeX = amountOfSlots * (spaceBetweenSlots + sizeXOfASlot);

        scrollableBackground.sizeDelta = new Vector2(sizeX, scrollableBackground.sizeDelta.y);
    }

    private void InstantiateSlots()
    {
        slotPrototype.GetComponent<CardReceptor>().CardsHolder = this;

        slotWrappers = new CardReceptorTransformWrapper[amountOfSlots];

        for (int i = 0; i < amountOfSlots; i++)
        {
            slotWrappers[i] = new CardReceptorTransformWrapper( Instantiate(slotPrototype) );
            slotWrappers[i].SetParent(transformWrapper, false);
            slotWrappers[i].CardReceptor.Index = i;
        }
    }

    private void PopulateCardPositionsArray()
    {
        cardPositions = new RectTransform[amountOfSlots];
        for (int i = 0; i < amountOfSlots; i++)
        {
            cardPositions[i] = slotWrappers[i].GetRectTransform();
        }
    }
    #endregion

    protected void Clear()
    {
        for (int i = 0; i < slotWrappers.Length; i++)
        {
            if (slotWrappers[i] != null)
            {
                slotWrappers[i].DestroyGameObject();
                slotWrappers[i] = null;
            }
        }
        slotWrappers = null;
        amountOfSlots = -1;
    }
}