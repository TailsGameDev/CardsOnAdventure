using System.Collections;
using UnityEngine;

public class Shakeable : MonoBehaviour
{
    [SerializeField]
    private int pointsQty = 20;
    [SerializeField]
    private int range = 20;
    [SerializeField]
    private float durationForEachPoint = 0.2f;

    [SerializeField]
    public bool infinite = false;

    public void Shake()
    {
        StartCoroutine(shake());
    }
    private IEnumerator shake()
    {
        // Create points
        Vector3[] points = new Vector3[pointsQty];
        float myRange = range;
        for (int i = 0; i < pointsQty; i++)
        {
            // get a random point
            points[i] = (new Vector3(
                Random.Range(-myRange, myRange), Random.Range(-myRange, myRange), 0));
            points[i] += transform.position;

            // shrink the range fot the shake to get smaller
            myRange = range * (pointsQty - i) / pointsQty;
        }

        float t = 0;
        int k = 0;
        // make the first and the last points equal to the current position
        points[0] = transform.position;
        points[pointsQty - 1] = transform.position;

        while (t < durationForEachPoint && k < pointsQty - 1)
        {
            t += Time.deltaTime;
            
            float x = Mathf.Lerp(points[k].x, points[k + 1].x, t / durationForEachPoint);
            float y = Mathf.Lerp(points[k].y, points[k + 1].y, t / durationForEachPoint);

            transform.position = new Vector3(x, y, transform.position.z);
            
            if (t >= durationForEachPoint)
            {
                k += 1;
                t = 0;
            }
            yield return null;
        }
        // Quick solving bug of cards staying in the wrong place
        yield return null;
        transform.position = points[0];

        if (infinite)
        {
            Shake();
        }
    }
}