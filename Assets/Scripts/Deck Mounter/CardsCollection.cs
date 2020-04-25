using UnityEngine;
using System.Collections;

public class CardsCollection : CardsHolder
{
    [SerializeField]
    private RectTransform slotPrototype = null;

    private void Awake()
    {
        cards = new Card[10];
    }
}
