using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipUseAsker : MonoBehaviour
{
    [SerializeField]
    private bool askOnStart = false;

    [SerializeField]
    private float delayAfterStartBeforeAsk = 1.0f;

    void Start()
    {
        if (askOnStart)
        {
            Invoke("Ask", delayAfterStartBeforeAsk);
        }
    }

    private void Ask()
    {
        TipDragAndDrop.AskToUseTips();
    }
}
