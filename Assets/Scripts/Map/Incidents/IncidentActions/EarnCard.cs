﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarnCard : IncidentAction
{
    [SerializeField]
    private AudioClip earnCardAudio = null;
    [SerializeField]
    private AudioRequisitor audioRequisitor = null;

    private Card[] cards;

    public override void Execute()
    {
        do
        {
            if (cards != null)
            {
                Destroy(cards[0].gameObject);
                Destroy(cards[1].gameObject);
            }

            cards = PlayerAndEnemyDeckHolder.Get2DifferentRandomUnlockedNotMonsterCards(deckSize: 2);

        } while (cards[0].IsAnotherInstanceOf(cards[1]));

        customPopUpOpener.OpenDisplayingCardsToChoose
            (
                title: "Earn a Card",
                warningMessage: "Choose a card to add to your collection, I mean, to become your friend.",
                confirmBtnMessage: "< Left One <",
                cancelBtnMessage: "> Right One >",
                onConfirm: () => { AddCardClosePopUpClearSpot(0); },
                onCancel: () => { AddCardClosePopUpClearSpot(1); },
                PreMadeAudioRequest.CreateSFX_AND_STOP_BGMAudioRequest(earnCardAudio, audioRequisitor, assignor: gameObject),
                cards
            );
    }

    private void AddCardClosePopUpClearSpot(int cardIndex)
    {
        CardsCollection.SumToCurrentAmount(cards[cardIndex], 1);
        openerOfPopUpsMadeInEditor.CloseAllPopUpsExceptLoading();
        sceneOpener.OpenMapScene();
    }
}
