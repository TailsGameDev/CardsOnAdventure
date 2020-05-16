using UnityEngine;
using UnityEngine.UI;

public class RandomColorChangingText : MonoBehaviour
{
    [SerializeField]
    private float max = 20;

    [SerializeField]
    private float r = 0;

    [SerializeField]
    private float dr = 1.0f;

    [SerializeField]
    private float divisor = 40;

    [SerializeField]
    private Text crazyColorText = null;

    [SerializeField]
    private bool timeCanStop = true;

    void Update()
    {
        if (r > max)
        {
            r = -max;
        }
        if (timeCanStop)
        {
            r += dr * TimeFacade.DeltaTime;
        }
        else
        {
            r += dr * TimeFacade.GetDeltaTimeEvenIfTimeIsStopped();
        }

        crazyColorText.color = new Color(changed(r), changed(r), changed(r));;
    }

    float changed(float n)
    {
        return Mathf.Abs(n) / divisor;
    }
}
