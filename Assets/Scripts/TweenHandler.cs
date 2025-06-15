using System;
using System.Collections.Generic;
using UnityEngine;

public class Tween
{
    public System.Action execute;
}

public class TweenHandler : Singleton<TweenHandler>
{
    private List<Tween> tweens;

    protected override void Awake()
    {
        base.Awake();
        tweens = new List<Tween>();
    }

    private void Update()
    {
        for (int t = tweens.Count - 1; t >= 0; t--)
        {
            tweens[t].execute();
        }
    }

    public Tween BeginQuadPositionTweenCheckNull(Transform transform, Transform target, float duration, Action onEnd = null)
    {
        Tween tween = new Tween();
        float startTime = Time.time;
        Vector3 originalLocalPosition = transform.position;

        tween.execute = () =>
        {
            if ((transform != null) && (target != null))
            {
                float elapsedTime = Time.time - startTime;

                // Quadratic easing in-out function (AI code)
                float t = Mathf.Clamp01(elapsedTime / duration);
                t = t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;

                transform.position = Vector3.Lerp(originalLocalPosition, target.position, t);

                if (elapsedTime >= duration)
                {
                    EndTween();
                }
            }
            else
            {
                EndTween();
            }

            void EndTween()
            {
                transform.position = target.position;
                tweens.Remove(tween);

                onEnd?.Invoke();
            }
        };

        tweens.Add(tween);
        return tween;
    }

    public void Cancel(Tween tween)
    {
        tweens.Remove(tween);
    }
}
