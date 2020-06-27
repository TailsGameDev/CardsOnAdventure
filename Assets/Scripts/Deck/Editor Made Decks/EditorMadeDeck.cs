using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorMadeDeck : MonoBehaviour
{
    //deck name is the game object name.
    static Dictionary<string, EditorMadeDeck> allCardCollecitions = null;

    [SerializeField]
    private Classes[] classesToAdd = null;

    public static Dictionary<string, EditorMadeDeck> AllCardCollecitions { get => allCardCollecitions; }

    private void Start()
    {
        allCardCollecitions.Add(gameObject.name, this);
    }

}
