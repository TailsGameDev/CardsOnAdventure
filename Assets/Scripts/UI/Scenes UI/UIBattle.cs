using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBattle : PopUpOpener
{
    public static bool inputEnabled = true;

    [SerializeField]
    private RectTransform parentForDynamicUIThatMustAppear = null;
    public static RectTransform parentOfDynamicUIThatMustAppear;

    [SerializeField]
    private Battlefield enemyBattlefield = null;

    [SerializeField]
    private Battlefield playerBattlefield = null;

    [SerializeField]
    private Hand playerHand = null;

    private Card cardBeingDragged;

    [SerializeField]
    private float delayToComeBackFromEnemyBattlefield = 0.5f;

    private bool isDragging = false;

    private CardsHolder cardsHolderBelowMouseOnDrop;
    private int indexOfSlotBelowMouseOnDrop;
    private bool wasDroppedInValidSpot = false;

    private void Awake()
    {
        parentOfDynamicUIThatMustAppear = parentForDynamicUIThatMustAppear;
    }

    #region On CardHolder Begin Drag
    public void OnEnemyBattlefieldSlotBeginDrag(int index)
    {
        // Currently there is nothing to done by dragging the enemy cards
    }

    public void OnPlayerBattlefieldSlotBeginDrag(int index)
    {
        OnAnyCardsHolderBeginDrag(playerBattlefield, index);
    }

    public void OnPlayerHandSlotBeginDrag(int index)
    {
        OnAnyCardsHolderBeginDrag(playerHand, index);
    }

    private void OnAnyCardsHolderBeginDrag(CardsHolder cardsHolder, int index)
    {
        if (inputEnabled && cardsHolder.ContainsCardInIndex(index))
        {
            StartCoroutine(DragHistory(cardsHolder, index));
        }
    }
    #endregion

    IEnumerator DragHistory(CardsHolder cardsHolder, int index)
    {
        isDragging = true;
        wasDroppedInValidSpot = false;

        ClearWholeDragAndDropSystem();

        // Wait for the BattleFSM to recognize that the variables ware cleared
        yield return null;

        cardsHolder.SetSelectedIndex(index);
        cardBeingDragged = cardsHolder.GetReferenceToCardAt(index);
        cardBeingDragged.cardDragAndDrop.StartDragging();

        while (isDragging)
        {
            Debug.Log("is dragging");
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

        ClearWholeDragAndDropSystem();
    }

    private void ClearWholeDragAndDropSystem()
    {
        playerHand.ClearSelection();
        playerBattlefield.ClearSelection();
        enemyBattlefield.ClearSelection();

        cardBeingDragged = null;

        cardsHolderBelowMouseOnDrop = null;
        indexOfSlotBelowMouseOnDrop = -1;
    }

    #region On CardHolder End Drag
    public void OnEnemyBattlefieldSlotEndDrag(int index)
    {
        OnAnyCardsHolderEndDrag(enemyBattlefield, index);
    }

    public void OnPlayerBattlefieldSlotEndDrag(int index)
    {
        OnAnyCardsHolderEndDrag(playerBattlefield, index);
    }

    public void OnPlayerHandSlotEndDrag(int index)
    {
        OnAnyCardsHolderEndDrag(playerHand, index);
    }

    private void OnAnyCardsHolderEndDrag(CardsHolder cardsHolder, int index)
    {
        isDragging = false;
    }
    #endregion

    // Drop Event is called by other scripts
    public void OnAnyCardsHolderDrop(CardsHolder cardsHolder, int index)
    {
        cardsHolderBelowMouseOnDrop = cardsHolder;
        indexOfSlotBelowMouseOnDrop = index;

        wasDroppedInValidSpot = true;
    }

    #region On CardHolder Click
    public void OnPlayerHandSlotClicked(int index)
    {
        OnSlotClicked(playerHand, index);
    }

    public void OnEnemyBattlefieldSlotClicked(int index)
    {
        OnSlotClicked(enemyBattlefield, index);
    }

    public void OnPlayerBattlefieldSlotClicked(int index)
    {
        OnSlotClicked(playerBattlefield, index);
    }

    public void OnSlotClicked(CardsHolder cardsHolder, int index)
    {
        if (inputEnabled)
        {
            cardsHolder.SetSelectedIndex(index);
        }
    }
    #endregion

    public void OnPauseBtnClicked()
    {
        popUpOpener.OpenPausePopUp();
    }
}
