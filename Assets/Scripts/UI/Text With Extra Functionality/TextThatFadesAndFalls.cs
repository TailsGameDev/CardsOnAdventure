using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextThatFadesAndFalls : MonoBehaviour
{
    // Y
    [SerializeField]
    float initialYVelocity = 0.0f;
    [SerializeField]
    float gravity = 10.0f;

    // X
    [SerializeField]
    float xVelocityRange = 0.0f;
    float xVelocity;

    [SerializeField]
    private Text text = null;

    
    [SerializeField]
    private float bonusToInitialAlpha = 0.5f;
    
    [SerializeField]
    private float timeToFade = 0.8f;

    /*
    [SerializeField]
    private Image[] imagesToMaybeActivate = null;
    */

    private float initialA;
    private float vA;

    private float r, g, b;

    // private
    private float initialX, initialY;

    private float time = 0.0f;

    private void Awake()
    {
        initialX = transform.position.x;
        initialY = transform.position.y;

        xVelocity = Random.Range(-xVelocityRange, xVelocityRange);

        initialA = text.color.a + bonusToInitialAlpha;
        vA = - initialA / timeToFade;

        SaveColorInRGBAttributes();
    }

    private void SaveColorInRGBAttributes()
    {
        Color textColor = text.color;
        r = textColor.r;
        g = textColor.g;
        b = textColor.b;
    }

    private void Update()
    {
        time += Time.deltaTime;

        float y = initialY + initialYVelocity * time + gravity * time * time / 2;
        float x = initialX + xVelocity * time;

        transform.position = new Vector3(x,y,0);

        float a = initialA + vA * (time);

        if (a <= 0)
        {
            Destroy(gameObject);
        }

        text.color = new Color(r, g, b, a);
    }

    /*
    public void ActivateAssignedImagesAndChangeColorToYellow()
    {
        foreach (Image image in imagesToMaybeActivate)
        {
            image.enabled = true;
        }
        text.color = Color.yellow;
        SaveColorInRGBAttributes();
    }
    */
}