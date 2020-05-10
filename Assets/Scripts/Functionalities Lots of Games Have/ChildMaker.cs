using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildMaker : MonoBehaviour
{
    private static readonly float DEFAULT_TRANSITION_TIME = 0.25f;

    public static void AdoptAndTeleport(Transform parent, RectTransform child)
    {
        child.position = parent.position;
        child.GetComponent<RectTransform>().SetParent(parent, true);
    }

    public static void AdoptTeleportAndScale(Transform parent, RectTransform child)
    {
        child.position = parent.position;
        child.SetParent(parent, true);
        child.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public static void AdoptAndScaleAndSmoothlyMoveToParent(Transform parent, RectTransform child, float totalTime = -1.0f)
    {
        if (totalTime < 0)
        {
            totalTime = DEFAULT_TRANSITION_TIME;
        }

        if (parent != null && child != null)
        {
            GameObject childMaker = new GameObject();
            childMaker.AddComponent(typeof(ChildMaker));
            ChildMaker maker = childMaker.GetComponent<ChildMaker>();

            maker.ScaleAndSmoothlyMoveChildToParentPosition(parent, child, totalTime);
        }
    }

    private void ScaleAndSmoothlyMoveChildToParentPosition(Transform parent, RectTransform child, float totalTime)
    {
        StartCoroutine(ScaleAndSmothlyMoveToPos(parent, child, totalTime));
    }

    IEnumerator ScaleAndSmothlyMoveToPos(Transform parent, RectTransform child, float totalTime)
    {
        // Scale
        child.GetComponent<RectTransform>().SetParent(parent, true);
        yield return null;
        child.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // Make it float above everybody.
        child.GetComponent<RectTransform>().SetParent(UIBattle.parentOfDynamicUIThatMustAppear, true);

        Vector3 initialPosition = child.transform.position;

        Vector3 direction = parent.transform.position - child.transform.position;

        float time = 0.0f;

        while (time < totalTime && child != null)
        {
            time += Time.deltaTime;

            float completePercentage = time / totalTime;

            child.transform.position = initialPosition + completePercentage * direction;

            yield return null;
        }

        child.transform.position = parent.transform.position;
        
        child.GetComponent<RectTransform>().SetParent(parent, true);

        Destroy(gameObject);
    }

    public static void AdoptAndSmoothlyMoveToPosition(Transform parent, RectTransform child, Vector3 position, float totalTime = -1.0f)
    {
        if (totalTime < 0)
        {
            totalTime = DEFAULT_TRANSITION_TIME;
        }

        if (parent != null && child != null)
        {
            GameObject childMaker = new GameObject();
            childMaker.AddComponent(typeof(ChildMaker));
            ChildMaker maker = childMaker.GetComponent<ChildMaker>();

            maker.ScaleAndSmoothlyMoveChildToParentPosition(parent, child, totalTime);
        }
    }

    private void SmoothlyMoveChildToPosition(Transform parent, RectTransform child, Vector3 position, float totalTime)
    {
        StartCoroutine(ScaleAndSmothlyMoveToPos(parent, child, totalTime));
    }
}   