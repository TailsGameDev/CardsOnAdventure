using System.Collections;
using UnityEngine;

public class UICardsHolderEventHandler : MonoBehaviour
{
    #region Attributes
    public static bool inputEnabled = true;

    [SerializeField]
    private RectTransform parentForDynamicUIThatMustAppear = null;
    public static RectTransform parentOfDynamicUIThatMustAppear;

    private Card cardBeingDragged;

    [SerializeField]
    private float delayToComeBackFromEnemyBattlefield = 0.5f;

    private bool isDragging = false;

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
        if (inputEnabled && cardsHolder.ContainsCardInIndex(index))
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
        isDragging = true;
        wasDroppedInValidSpot = false;

        ClearWholeDragAndDropSystem(new CardsHolder[] { cardsHolder });

        // Wait for the BattleFSM to recognize that the variables ware cleared
        yield return null;

        cardsHolder.SetSelectedIndex(index);
        cardBeingDragged = cardsHolder.GetReferenceToCardAt(index);
        cardBeingDragged.cardDragAndDrop.StartDragging();

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

            // Wait for battleFSM to do it's job.
            yield return null;
            yield return null;

            ChildMaker.AdoptAndSmoothlyMoveToParent
                (
                    cardBeingDragged.transform.parent,
                    cardBeingDragged.GetComponent<RectTransform>(),
                    delayToComeBackFromEnemyBattlefield
                );
        }

        ClearWholeDragAndDropSystem(new CardsHolder[] { cardsHolder, cardsHolderBelowMouseOnDrop });
    }

    public void OnSlotClicked(CardsHolder cardsHolder, int index)
    {
        if (inputEnabled)
        {
            cardsHolder.SetSelectedIndex(index);
        }
    }

    private void ClearWholeDragAndDropSystem(CardsHolder[] cardsHoldersToClear)
    {
        foreach (CardsHolder cardsHolder in cardsHoldersToClear)
        {
            cardsHolder.ClearSelection();
        }

        cardBeingDragged = null;

        cardsHolderBelowMouseOnDrop = null;
        indexOfSlotBelowMouseOnDrop = -1;
    }
}
