using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChildMaker : MonoBehaviour
{
    private static List<RectTransform> movingRects = new List<RectTransform>();

    private static ChildMaker instance;

    private static readonly float DEFAULT_TRANSITION_TIME = 0.25f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static bool IsRectTransformBeingMoved(RectTransform rt)
    {
        return movingRects.Contains(rt);
    }

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

    public static void AdoptAndScaleAndSmoothlyMoveToParent(Transform parent, RectTransform child, float totalTime = -1.0f)
    {
        if (totalTime < 0)
        {
            totalTime = DEFAULT_TRANSITION_TIME;
        }

        if (parent != null && child != null)
        {
            instance.ScaleAndSmoothlyMoveChildToParentPosition(parent, child, totalTime);
        }
    }

    private void ScaleAndSmoothlyMoveChildToParentPosition(Transform parent, RectTransform child, float totalTime)
    {
        StartCoroutine(WaitForPreviousCoroutinesToEndThenMakeSmoothMovement(parent, child, totalTime));
    }

    private IEnumerator WaitForPreviousCoroutinesToEndThenMakeSmoothMovement(Transform parent, RectTransform child, float totalTime)
    {
        while (parent != null && child != null && movingRects.Contains(child))
        {
            yield return null;
        }

        if (parent != null && child != null)
        {
            StartCoroutine(ScaleAndSmothlyMoveToPos(parent, child, totalTime));
        }
    }

    private IEnumerator ScaleAndSmothlyMoveToPos(Transform parent, RectTransform child, float totalTime)
    {
        if (child != null)
        {
            movingRects.Add(child);

            // Scale
            child.SetParent(parent, true);
            child.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            // Make it float above everybody.
            child.SetParent(UIBattle.parentOfDynamicUIThatMustAppear, true);

            Vector3 initialPosition = child.transform.position;

            Vector3 direction = parent.transform.position - child.transform.position;

            float time = 0.0f;

            while (child != null && time < totalTime)
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
            }

            movingRects.Remove(child);
        }
    }

    /*
    private void SmoothlyMoveChildToPosition(Transform parent, RectTransform child, Vector3 position, float totalTime)
    {
        StartCoroutine(ScaleAndSmothlyMoveToPos(parent, child, totalTime));
    }
    */
}   