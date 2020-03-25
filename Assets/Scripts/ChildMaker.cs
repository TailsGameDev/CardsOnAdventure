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

    public static void AdoptTeleportAndScale(Transform father, RectTransform child)
    {
        child.position = father.position;
        RectTransform childTransform = child.GetComponent<RectTransform>();
        childTransform.SetParent(father, true);
        childTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
}   