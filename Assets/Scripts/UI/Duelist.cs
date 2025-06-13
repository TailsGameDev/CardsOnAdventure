using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Duelist : MonoBehaviour
{
    // TODO: Review if to unite all duelist references here, like battlefield and hand
    // [SerializeField]
    // private Battlefield battlefield = null;
    // [SerializeField]
    // private Hand hand = null;

    [SerializeField]
    private Deck deck = null;

    [SerializeField][FormerlySerializedAs("text")]
    private Text hpText = null;

    private int healthPoints;

    private IEnumerator Start()
    {
        // Wait for deck to initialize
        yield return null;
        yield return null;

        healthPoints = deck.GetInitialHP();
        SetHPText(hp: healthPoints);
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
