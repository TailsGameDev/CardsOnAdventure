using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrazyColor : MonoBehaviour
{
    int r = 0, g= 0, b=255;

    int control = 0;

    Image crazyColorImage = null;

    void Update()
    {
        switch (control)
        {
            case 0:
                g++;
                if (g == 255) control++;
                break;
            case 1:
                b--;
                if (b == 0) control++;
                break;
            case 2:
                r++;
                if (r == 255) control++;
                break;
            case 3:
                g--;
                if (g == 0) control++;
                break;
            case 4:
                b++;
                if (b == 255) control++;
                break;
            case 5:
                r--;
                if (r == 0) control = 0;
                break;
        }

        crazyColorImage.color = new Color(r,g,b);
    }
}
