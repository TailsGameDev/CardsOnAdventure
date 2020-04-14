using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBattle : PopUpOpener
{
    public static bool inputEnabled = true;

    [SerializeField]
    private RectTransform UIDamageTextParent = null;
    public static RectTransform parentOfDynamicUIThatMustAppear;

    [SerializeField]
    private Battlefield enemyBattlefield = null;

    [SerializeField]
    private Battlefield playerBattlefield = null;

    [SerializeField]
    private Hand playerHand = null;

    private Card cardBeingDragged;
    private bool isDragging;

    [SerializeField]
    private float delayToComeBackFromEnemyBattlefield;

    private void Awake()
    {
        parentOfDynamicUIThatMustAppear = UIDamageTextParent;
    }


    // Enemy Battlefield
    public void OnEnemyBattlefieldSlotBeginDrag(int index)
    {
        OnAnyCardsHolderBeginDrag(enemyBattlefield, index);
    }

    public void OnEnemyBattlefieldSlotEndDrag(int index)
    {
        OnAnyCardsHolderEndDrag(enemyBattlefield, index);
    }

    // Player Battlefield
    public void OnPlayerBattlefieldSlotBeginDrag(int index)
    {
        OnAnyCardsHolderBeginDrag(playerBattlefield, index);
    }

    public void OnPlayerBattlefieldSlotEndDrag(int index)
    {
        OnAnyCardsHolderEndDrag(playerBattlefield, index);
    }

    // Player Hand
    public void OnPlayerHandSlotBeginDrag(int index)
    {
        OnAnyCardsHolderBeginDrag(playerHand, index);
    }

    public void OnPlayerHandSlotEndDrag(int index)
    {
        OnAnyCardsHolderEndDrag(playerHand, index);
    }

    private void OnAnyCardsHolderBeginDrag(CardsHolder cardsHolder, int index)
    {
        if (inputEnabled)
        {
            Drag(cardsHolder, index);
        }
    }

    private void Drag(CardsHolder cardsHolder, int index)
    {
        if (cardsHolder.ContainsCardInIndex(index))
        {
            cardsHolder.SetSelectedIndex(index);
            cardBeingDragged = cardsHolder.GetReferenceToCardAt(index);
            cardBeingDragged.cardDragAndDrop.StartDragging();
        }
    }

    private void OnAnyCardsHolderEndDrag(CardsHolder cardsHolder, int index)
    {
        if (inputEnabled)
        {
            StartCoroutine(EndDrag());
        }
    }

    IEnumerator EndDrag()
    {
        if (cardBeingDragged != null)
        {
            cardBeingDragged.cardDragAndDrop.Drop();
        }

        yield return null;
        // in this frame, delayed drop should be seting selectedIndex of cardsHolder
        yield return null;
        // here, the battleFSM should be doing it's stuff with the selectedIndex.
        // Also, DelayedDrop should be doing  ChildMaker.AdoptAndSmoothlyMoveToParent().
        yield return null;

        cardBeingDragged = null;
        playerHand.ClearSelection();
        playerBattlefield.ClearSelection();
        enemyBattlefield.ClearSelection();
    }

    public void OnAnyCardsHolderDrop(CardsHolder cardsHolder, int index)
    {
        if (inputEnabled)
        {
            StartCoroutine(DelayedDrop(cardsHolder, index));
        }
    }

    IEnumerator DelayedDrop(CardsHolder cardsHolder, int index)
    {
        yield return null;
        cardsHolder.SetSelectedIndex(index);
        if (cardsHolder == enemyBattlefield)
        {
            yield return null;
            ChildMaker.AdoptAndSmoothlyMoveToParent
                (
                    cardBeingDragged.transform.parent,
                    cardBeingDragged.GetComponent<RectTransform>(),
                    delayToComeBackFromEnemyBattlefield
                );
        }
    }

    public void OnEndDrag()
    {
        
    }

    public void OnPauseBtnClicked()
    {
        popUpOpener.OpenPausePopUp();
    }
}
