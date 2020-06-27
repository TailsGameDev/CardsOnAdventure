using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextThatExpandsOnUpdated : MonoBehaviour
{
    [SerializeField]
    private float expantionMultiplyerMaximum = 2.0f;
    private float currentExpansionMultiplier = 1.0f;
    [SerializeField]
    private float minimumExpansionMultiplier = 1.0f;

    [SerializeField]
    private Text text = null;

    private Vector3 originalSize;
    private bool animating;

    private bool growing;

    private Color originalColor;

    private void Awake()
    {
        originalSize = transform.localScale;
        originalColor = text.color;
    }
    private void Update()
    {
        if (animating)
        {
            if ( growing )
            {
                if (currentExpansionMultiplier < expantionMultiplyerMaximum)
                {
                    currentExpansionMultiplier += TimeFacade.DeltaTime;
                    transform.localScale = originalSize * currentExpansionMultiplier;
                } else
                {
                    growing = false;
                }
            }
            else
            {
                if (currentExpansionMultiplier > minimumExpansionMultiplier)
                {
                    currentExpansionMultiplier -= TimeFacade.DeltaTime;
                    transform.localScale = originalSize * currentExpansionMultiplier;
                }
                else
                {
                    animating = false;
                }
            }
        }
    }

    public void StartAnimationFromBeggining()
    {
        animating = true;
        growing = true;
        currentExpansionMultiplier = minimumExpansionMultiplier;
    }

    string cachedMessage;

    public void DisplayText(string message)
    {
        if (message.CompareTo(cachedMessage) != 0)
        {
            cachedMessage = message;
            text.text = message;

            if (message.Contains("Bonus"))
            {
                text.color = new Color(0x9E, 0xFA, 0x9D);
            }
            else
            {
                text.color = originalColor;
            }

            StartAnimationFromBeggining();
        }
    }
}
