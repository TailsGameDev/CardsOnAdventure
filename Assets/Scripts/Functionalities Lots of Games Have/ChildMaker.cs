using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildMaker : MonoBehaviour
{
    private static readonly float DEFAULT_TRANSITION_TIME = 0.25f;

    public static void CopySizeDelta(RectTransform fontOfDelta, RectTransform deltaToChange)
    {
        deltaToChange.sizeDelta = fontOfDelta.sizeDelta;
    }

    public static void AdoptAndTeleport(Transform parent, RectTransform child)
    {
        child.position = parent.position;
        child.SetParent(parent, true);
    }

    public static void AdoptTeleportAndScale(Transform parent, RectTransform child)
    {
        child.position = parent.position;
        child.SetParent(parent, true);
        child.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public static void AdoptAndScaleAndSmoothlyMoveToParentThenDestroyChild(Transform parent, RectTransform child, float totalTime = -1.0f)
    {
        AdoptAndScaleAndSmoothlyMoveToParent(parent, child, totalTime);
        DestroyItselfInTime suicideChild = child.gameObject.AddComponent(typeof(DestroyItselfInTime)) as DestroyItselfInTime;
        suicideChild.SetDestructionTime(totalTime + 0.2f);
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
            ChildMaker maker = (ChildMaker) childMaker.AddComponent(typeof(ChildMaker));

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
        child.SetParent(parent, true);
        yield return null;
        child.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // Make it float above everybody.
        child.SetParent(UIBattle.parentOfDynamicUIThatMustAppear, true);

        Vector3 initialPosition = child.transform.position;

        Vector3 direction = parent.transform.position - child.transform.position;

        float time = 0.0f;

        while (child != null && time < totalTime && child != null)
        {
            time += TimeFacade.DeltaTime;

            float completePercentage = time / totalTime;

            child.transform.position = initialPosition + completePercentage * direction;

            yield return null;
        }

        if (child != null)
        {
            child.transform.position = parent.transform.position;
        
            child.SetParent(parent, true);

            Destroy(gameObject);
        }
    }

    private void SmoothlyMoveChildToPosition(Transform parent, RectTransform child, Vector3 position, float totalTime)
    {
        StartCoroutine(ScaleAndSmothlyMoveToPos(parent, child, totalTime));
    }
}   