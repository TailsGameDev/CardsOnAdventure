using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Duelist : MonoBehaviour
{
    [SerializeField]
    private Battlefield battlefield = null;

    [SerializeField]
    private Hand hand = null;

    [SerializeField]
    private Deck deck = null;

    [SerializeField]
    private Text text = null;

    private string initialText;

    int standingAmount = 8;

    [SerializeField]
    int healthPoints;

    private void Start()
    {
        initialText = text.text;
        standingAmount = 0;
    }

    void Update()
    {
        int standingAmount = battlefield.GetAmountOfOccupiedSlots() + hand.GetAmountOfOccupiedSlots() + deck.GetSize();
        if (standingAmount != this.standingAmount)
        {
            text.text = initialText + standingAmount;
        }
    }

    public void TakeDamage(int amount)
    {
        healthPoints -= amount;
        if(healthPoints < 0) healthPoints = 0;
    }
}
