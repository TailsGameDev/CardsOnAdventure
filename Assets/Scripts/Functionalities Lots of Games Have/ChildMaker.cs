using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildMaker : MonoBehaviour
{
    private static List<TransformWrapper> movingObjects = new List<TransformWrapper>();

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

    public static bool IsRectTransformBeingMoved(TransformWrapper transformWrapper)
    {
        return movingObjects.Contains(transformWrapper);
    }

    public static void CopySizeDelta(RectTransform fontOfDelta, RectTransform deltaToChange)
    {
        deltaToChange.sizeDelta = fontOfDelta.sizeDelta;
    }

    public static void AdoptAndTeleport(TransformWrapper parent, TransformWrapper child)
    {
        child.Position = parent.Position;
        child.SetParent(parent, true);
    }

    public static void AdoptTeleportAndScale(TransformWrapper parent, TransformWrapper child)
    {
        child.Position = parent.Position;
        child.SetParent(parent, true);
        child.LocalScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public static void AdoptAndScaleAndSmoothlyMoveToParent(TransformWrapper parent, TransformWrapper child, float totalTime = -1.0f)
    {
        if (totalTime < 0.0f)
        {
            totalTime = DEFAULT_TRANSITION_TIME;
        }

        if (parent != null && child != null)
        {
            instance.StartCoroutine(instance.WaitForPreviousCoroutinesToEndThenMakeSmoothMovement(parent, child, totalTime));
        }
    }

    private IEnumerator WaitForPreviousCoroutinesToEndThenMakeSmoothMovement(TransformWrapper parent, TransformWrapper child, float totalTime)
    {
        while (parent != null && child != null && movingObjects.Contains(child))
        {
            yield return null;
        }

        StartCoroutine(ScaleAndSmothlyMoveToPos(parent, child, totalTime));
    }

    private IEnumerator ScaleAndSmothlyMoveToPos(TransformWrapper parent, TransformWrapper child, float totalTime)
    {
        movingObjects.Add(child);

        // Scale
        child.SetParent(parent, true);
        yield return null;
        child.LocalScale = new Vector3(1.0f, 1.0f, 1.0f);

        // Make it float above everybody.
        child.SetParent(UIBattle.parentOfDynamicUIThatMustAppear, true);

        Vector3 initialPosition = child.Position;

        Vector3 direction = parent.Position - child.Position;

        float time = 0.0f;

        while (child != null && time < totalTime)
        {
            time += TimeFacade.DeltaTime;

            float completePercentage = time / totalTime;

            child.Position = initialPosition + completePercentage * direction;

            yield return null;
        }

        if (child != null)
        {
            child.Position = parent.Position;
        
            child.SetParent(parent, true);
        }

        movingObjects.Remove(child);
    }

    /*
    private void SmoothlyMoveChildToPosition(Transform parent, RectTransform child, Vector3 position, float totalTime)
    {
        StartCoroutine(ScaleAndSmothlyMoveToPos(parent, child, totalTime));
    }
    */
}   