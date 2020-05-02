using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fader
{
    public IEnumerator DieWithAnimation(float fadingDuration, Image[] allChildrenImages, Text[] allChildrenTexts)
    {
        float fadePercentage = 1.0f;
        float countDownFadeTimer = fadingDuration;

        Color[] originalColors = new Color[allChildrenImages.Length];

        float[] r = new float[originalColors.Length]; float[] g = new float[originalColors.Length];
        float[] b = new float[originalColors.Length];

        for (int i = 0; i < allChildrenImages.Length; i++)
        {
            originalColors[i] = allChildrenImages[i].color;
            Color color = originalColors[i];
            r[i] = color.r; g[i] = color.g; b[i] = color.b;
        }

        float[] tr = new float[originalColors.Length]; float[] tg = new float[originalColors.Length];
        float[] tb = new float[originalColors.Length];
        for (int i = 0; i < allChildrenTexts.Length; i++)
        {
            tr[i] = allChildrenTexts[i].color.r;
            tg[i] = allChildrenTexts[i].color.g;
            tb[i] = allChildrenTexts[i].color.b;
        }

        while (countDownFadeTimer > 0.0f)
        {
            fadePercentage = countDownFadeTimer / fadingDuration;

            for (int i = 0; i < allChildrenImages.Length; i++)
            {
                if (allChildrenImages[i] != null)
                {
                    allChildrenImages[i].color = new Color(r[i], g[i], b[i], fadePercentage); ;
                }
            }

            for (int i = 0; i < allChildrenTexts.Length; i++)
            {
                if (allChildrenTexts[i] != null)
                {
                    allChildrenTexts[i].color = new Color(tr[i], tg[i], tb[i], fadePercentage);
                }
            }

            countDownFadeTimer -= Time.deltaTime;
            yield return null;
        }
    }
}
