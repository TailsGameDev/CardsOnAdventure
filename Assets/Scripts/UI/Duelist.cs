using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Duelist : MonoBehaviour
{
    [SerializeField]
    private Battlefield battlefield = null;

    [SerializeField]
    private Hand hand = null;

    [SerializeField]
    private Deck deck = null;

    [SerializeField][FormerlySerializedAs("text")]
    private Text hpText = null;

    private int healthPoints;

    private void Start()
    {
        healthPoints = battlefield.GetAmountOfOccupiedSlots() + hand.GetAmountOfOccupiedSlots() + deck.GetSize();
        SetHPText(healthPoints);
    }

    private void SetHPText(int hp)
    {
        const string PREFIX = "HP: ";
        hpText.text = PREFIX + hp;
    }

    public void TakeDamage(int amount)
    {
        healthPoints -= amount;
        if (healthPoints < 0)
        {
            healthPoints = 0;
        }
        SetHPText(healthPoints);
    }
}
