using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildMaker
{
    public static void AdoptAndTeleport(Transform father, RectTransform child)
    {
        child.position = father.position;
        child.GetComponent<RectTransform>().SetParent(father, true);
    }
}   