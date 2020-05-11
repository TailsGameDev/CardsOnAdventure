using System;
using UnityEngine;

public abstract class DragAndDropReceptor : MonoBehaviour
{
    [SerializeField]
    private int priority = 0;

    public int Priority { get => priority; }

    public abstract Type GetDragAndDropReceptorType();

    public abstract void OnDroppedInReceptor();

    public RectTransform GetRectTransform()
    {
        return GetComponent<RectTransform>();
    }
}