using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipDisplayer : MonoBehaviour
{
    public static TooltipDisplayer instance;

    [SerializeField]
    private GameObject tooltipNode = null;

    private Image background = null;

    [SerializeField]
    private Text message = null;

    private double timer = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(instance);
        }

        background = tooltipNode.GetComponent<Image>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            tooltipNode.transform.position = Input.mousePosition;
        }

        tooltipNode.SetActive(timer > 0 && !string.IsNullOrEmpty(message.text));

        timer -= Time.deltaTime;
    }

    public void SetMessageAndTimerAndSize(string message, float timeInSeconds = 1.0f, int width = 60, int height = 17)
    {
        if (timer <= 0)
        {
            this.message.text = message;
            this.timer = timeInSeconds;

            RectTransform rt = background.GetComponent(typeof(RectTransform)) as RectTransform;
            rt.sizeDelta = new Vector2(width, height);
        }
    }
}
