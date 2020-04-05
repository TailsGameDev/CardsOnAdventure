using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
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
    private float timeToFade = 0.8f;

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

        initialA = text.color.a;
        vA = - initialA / timeToFade;

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

        float a = initialA + vA * time;

        if (a <= 0)
        {
            Destroy(gameObject);
        }

        text.color = new Color(r, g, b, a);
    }
}

/*
public class DamageText : MonoBehaviour
{
    [SerializeField]
    private Text text;

    private Color textColor;
    private float r, g, b, a;

    [SerializeField]
    private float deltaY = 0.0f;
    [SerializeField]
    private float deltaX = 0.0f;

    [SerializeField]
    private float timeY = 0.0f;
    [SerializeField]
    private float timeX = 0.0f;

    [SerializeField]
    private float timeToFade = 0.0f;

    void Start()
    {
        LeanTween.moveY(gameObject, transform.position.y + deltaY, timeY).setEaseInOutQuad()..setLoopPingPong();
        LeanTween.moveX(gameObject, transform.position.x + deltaX, timeX);

        textColor = text.color;
        r = textColor.r;
        g = textColor.g;
        b = textColor.b;
        a = textColor.a;
    }

    // Update is called once per frame
    void Update()
    {
        a -= Time.deltaTime/timeToFade;
        text.color = new Color(r, g, b, a);
    }
}
*/
