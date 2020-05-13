using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomSprite : MonoBehaviour
{

    [SerializeField]
    private Image image = null;

    [SerializeField]
    private Sprite[] possibleSprites = null;

    private int currentSpriteIndex = -1;

    void Awake()
    {
        ChangeSprite();
    }

    public void ChangeSprite()
    {
        int newSpriteIndex = Random.Range(0, possibleSprites.Length);

        int shitAvoidant = 50;

        while (newSpriteIndex == currentSpriteIndex && shitAvoidant > 0)
        {
            newSpriteIndex = Random.Range(0, possibleSprites.Length);
            shitAvoidant--;
        }

        currentSpriteIndex = newSpriteIndex;

        image.sprite = possibleSprites[currentSpriteIndex];
    }
}
