using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildMaker : MonoBehaviour
{
    public static void AdoptAndTeleport(Transform parent, RectTransform child)
    {
        child.position = parent.position;
        child.GetComponent<RectTransform>().SetParent(parent, true);
    }

    public static void AdoptTeleportAndScale(Transform parent, RectTransform child)
    {
        child.position = parent.position;
        RectTransform childTransform = child.GetComponent<RectTransform>();
        childTransform.SetParent(parent, true);
        childTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public static void AdoptAndSmoothlyMoveToParent(Transform parent, RectTransform child, float totalTime)
    {
        GameObject childMaker = new GameObject();
        childMaker.AddComponent(typeof(ChildMaker));
        ChildMaker maker = childMaker.GetComponent<ChildMaker>();

        maker.SmoothlyMoveChildToParentPosition(parent, child, totalTime);
    }

    private void SmoothlyMoveChildToParentPosition(Transform parent, RectTransform child, float totalTime)
    {
        StartCoroutine(SmothlyMoveToPos(parent, child, totalTime));
    }

    IEnumerator SmothlyMoveToPos(Transform parent, RectTransform child, float totalTime)
    {
        child.GetComponent<RectTransform>().SetParent(UIBattle.parentOfDynamicUIThatMustAppear, true);

        Vector3 initialPosition = child.transform.position;

        Vector3 direction = parent.transform.position - child.transform.position;

        float time = 0.0f;

        while (time < totalTime)
        {
            time += Time.deltaTime;

            float completePercentage = time / totalTime;

            child.transform.position = initialPosition + completePercentage * direction;

            yield return null;
        }

        child.GetComponent<RectTransform>().SetParent(parent, true);

        child.transform.position = parent.transform.position;

        Destroy(gameObject);
    }
}   