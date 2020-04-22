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
    private CardsHolder[] emergenceClearCardHolders = null;

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

        HideCardsBeingDraggedTip();
        ClearWholeDragAndDropSystem(new CardsHolder[] { cardsHolder, cardsHolderBelowMouseOnDrop });
    }

    private void ShowTipAboutCardBeingDragged()
    {
        skillTipText.text = cardBeingDragged.GetSkillsExplanatoryText();
        skillTipText.transform.parent.gameObject.SetActive(true);
    }

    private void HideCardsBeingDraggedTip()
    {
        skillTipText.transform.parent.gameObject.SetActive(false);
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
            // TODO: understand reason of null pointer exception and avoid this possibility if possible
            if (cardsHolder != null)
            {
                cardsHolder.ClearSelection();
            }
            else
            {
                // I don't know exactly why that's null. That's why I'll clear some cardHolders that with some luck
                // are the ones that should be cleared.
                EmergencyClearCardHolders();
                L.og("cardsHolder is null!! It would be nice if you stop some day to understand why...", this);
            }
        }

        cardBeingDragged = null;

        cardsHolderBelowMouseOnDrop = null;
        indexOfSlotBelowMouseOnDrop = -1;
    }

    protected virtual void EmergencyClearCardHolders()
    {
        foreach (CardsHolder cardHolder in emergenceClearCardHolders)
        {
            cardHolder.ClearSelection();
        }
    }
}
