using System;
using UnityEngine;

public abstract class DragAndDropReceptor : MonoBehaviour
{
    public abstract Type GetDragAndDropReceptorType();

    public abstract void OnDroppedInReceptor();
}