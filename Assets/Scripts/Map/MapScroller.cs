using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScroller : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform = null;

    [SerializeField]
    private float timeToGetToDestination = 1.0f;

    static private float targetLeft;

    private bool interpolating = false;

    public void FocusOnInitialMap()
    {
        targetLeft = 0.0f;
        SetLeft(rectTransform, targetLeft);
    }
    private void SetLeft(RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }
    private void Start()
    {
        SetLeft(rectTransform, targetLeft);
    }

    public void InterpolateLeftTo(float targetLeft)
    {
        MapScroller.targetLeft = targetLeft;
        StartCoroutine(InterpolateToTargetLeft());
    }
    private IEnumerator InterpolateToTargetLeft()
    {
        while (interpolating == true)
        {
            yield return null;
        }
        interpolating = true;

        float initialLeft = rectTransform.rect.xMin;
        float t = 0;

        while (t < timeToGetToDestination)
        {
            float newLeft = Mathf.Lerp(initialLeft, targetLeft, t);

            SetLeft(rectTransform, newLeft);

            t += Time.deltaTime;
            yield return null;
        }

        SetLeft(rectTransform, targetLeft);
        interpolating = false;
    }
}
