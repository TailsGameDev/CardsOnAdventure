using System.Collections;
using UnityEngine;

public class SpeakBalloon : MonoBehaviour
{
    [SerializeField]
    private Shakeable shakeable = null;

    [SerializeField]
    private float timeToHide = 2.0f;

    private void OnEnable()
    {
        StartCoroutine(PayAttentionToMe());
    }

    private IEnumerator PayAttentionToMe()
    {
        shakeable.Shake();
        yield return new WaitForSeconds(timeToHide);
        gameObject.SetActive(false);
    }
}
