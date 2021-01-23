using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UICardsHolderEventHandler : MonoBehaviour
{
    #region Attributes
    public static bool inputEnabled = true;

    [SerializeField]
    private RectTransform parentForDynamicUIThatMustAppear = null;
    public static RectTransform parentOfDynamicUIThatMustAppear;

    [SerializeField]
    private Text skillTipText = null;

    [SerializeField]
    private TipPopUpOpener tipPopUpOpener = null;

    [SerializeField]
    private CardsHolder[] emergenceClearCardHolders = null;

    private Card cardBeingDragged;

    [SerializeField]
    private float delayToComeBackFromOtherSpot = 0.25f;

    private bool isDragging = false;
    private bool isRunningDragHistoryCoroutine = false;

    private CardsHolder cardsHolderBelowMouseOnDrop;
    private int indexOfSlotBelowMouseOnDrop;
    private bool wasDroppedInValidSpot = false;
    #endregion

    private void Awake()
    {
        parentOfDynamicUIThatMustAppear = parentForDynamicUIThatMustAppear;
    }

    public void OnCardsHolderBeginDrag(CardsHolder cardsHolder, int index)
    {
        if (inputEnabled && !isRunningDragHistoryCoroutine && cardsHolder.ContainsCardInIndex(index))
        {
            StartCoroutine(DragHistory(cardsHolder, index));
        }
    }

    public void OnCardsHolderEndDrag()
    {
        isDragging = false;
    }

    public void OnCardsHolderDrop(CardsHolder cardsHolder, int index)
    {
        cardsHolderBelowMouseOnDrop = cardsHolder;
        indexOfSlotBelowMouseOnDrop = index;

        wasDroppedInValidSpot = true;
    }

    private IEnumerator DragHistory(CardsHolder cardsHolder, int index)
    {
        isRunningDragHistoryCoroutine = true;
        isDragging = true;
        wasDroppedInValidSpot = false;

        ClearSelections(new CardsHolder[] { cardsHolder });

        cardBeingDragged = null;
        cardsHolderBelowMouseOnDrop = null;
        indexOfSlotBelowMouseOnDrop = -1;

        // Wait for the BattleFSM to recognize that the variables ware cleared
        yield return null;

        cardsHolder.SetSelectedIndex(index);
        cardBeingDragged = cardsHolder.GetReferenceToCardAt(index);
        cardBeingDragged.cardDragAndDrop.StartDragging();

        ShowTipAboutCardBeingDragged();

        while (isDragging)
        {
            yield return null;
        }

        cardBeingDragged.cardDragAndDrop.Drop();

        if (!wasDroppedInValidSpot)
        {
            // Wait one more frame for the drop event to be called.
            yield return null;
        }

        if (wasDroppedInValidSpot)
        {
            cardsHolderBelowMouseOnDrop.SetSelectedIndex(indexOfSlotBelowMouseOnDrop);

            // Wait for battleFSM to do it's job (like set the parent of the card).
            yield return null;
            yield return null;

            while (cardBeingDragged != null && ChildMaker.IsRectTransformBeingMoved(cardBeingDragged.GetRectTransform()))
            {
                yield return null;
            }

            // A card can be null because to make the card go back instantly, a controller script should delete
            // this card, and put a clone in the place the original was.
            if (cardBeingDragged != null)
            {
                ChildMaker.AdoptAndScaleAndSmoothlyMoveToParent
                (
                    cardBeingDragged.RectTransform.parent,
                    cardBeingDragged.RectTransform,
                    delayToComeBackFromOtherSpot
                );
            }
        }

        HideCardsBeingDraggedTip();

        if (cardsHolderBelowMouseOnDrop == null)
        {
            ClearSelections(new CardsHolder[] { cardsHolder });
        }
        else
        {
            ClearSelections(new CardsHolder[] { cardsHolder, cardsHolderBelowMouseOnDrop });
        }

        if (wasDroppedInValidSpot && cardBeingDragged != null)
        {
            yield return new WaitForSeconds(delayToComeBackFromOtherSpot);
        }

        isRunningDragHistoryCoroutine = false;
    }

    private void ShowTipAboutCardBeingDragged()
    {
        skillTipText.transform.parent.gameObject.SetActive(true);
        skillTipText.text = cardBeingDragged.GetSkillsExplanatoryText();
    }

    private void HideCardsBeingDraggedTip()
    {
        skillTipText.transform.parent.gameObject.SetActive(false);
    }

    public void OnSlotClicked(CardsHolder cardsHolder, int index)
    {
        Card cardInSlot = cardsHolder.GetReferenceToCardAtOrGetNull(index);
        if (cardInSlot != null)
        {
            cardInSlot.OpenTip(tipPopUpOpener);
        }
    }

    private void ClearSelections(CardsHolder[] cardsHoldersToClear)
    {
        foreach (CardsHolder cardsHolder in cardsHoldersToClear)
        {
            cardsHolder.ClearSelection();
        }
    }

    protected virtual void EmergencyClearCardHolders()
    {
        foreach (CardsHolder cardHolder in emergenceClearCardHolders)
        {
            cardHolder.ClearSelection();
        }
    }
}
