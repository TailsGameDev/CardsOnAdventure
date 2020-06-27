using System.Collections.Generic;
using UnityEngine;

public class CardCollection : MonoBehaviour
{
    [SerializeField]
    Card[] cards = null;

    public Card[] GetCards()
    {
        return cards;
    }
}
