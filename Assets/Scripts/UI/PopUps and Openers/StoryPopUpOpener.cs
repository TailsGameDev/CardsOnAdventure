using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class StoryPopUpOpener : OpenersSuperclass
{
    [System.Serializable]
    struct CharacterSpeechData
    {
        [SerializeField] private string characterName;
        [SerializeField] private Sprite characterSprite;
        [SerializeField] private Color backgroundColor;

        public string CharacterName { get => characterName; }
        public Sprite Sprite { get => characterSprite; }
        public Color BackgroundColor { get => backgroundColor; }
    }

    [SerializeField] private VerticalLayoutGroup speechesLayout = null;

    [SerializeField] private StorySpeech speechPrototype = null;

    [FormerlySerializedAs("characterSprites")]
    [SerializeField] private CharacterSpeechData[] characterSpeechDataArray = null;

    private Dictionary<string, CharacterSpeechData> characterSpeechDataDictionary = new Dictionary<string, CharacterSpeechData>();

    private List<StorySpeech> speechesToClearWhenOpenning = new List<StorySpeech>();

    private void Awake()
    {
        if (storyPopUpOpener == null) 
        {
            storyPopUpOpener = this;

            // Populate characterSpeechDataDictionary
            for (int s = 0; s < characterSpeechDataArray.Length; s++)
            {
                CharacterSpeechData characterSprite = characterSpeechDataArray[s];
                characterSpeechDataDictionary[characterSprite.CharacterName] = characterSprite;
            }
        }
    }

    public void Open(string story)
    {
        // Destroy speeches from last time openned
        for (int s = speechesToClearWhenOpenning.Count-1; s >= 0; s--)
        {
            StorySpeech speech = speechesToClearWhenOpenning[s];
            speechesToClearWhenOpenning.RemoveAt(s);
            Destroy(speech.gameObject);
        }

        string[] speechMessages = story.Split('¢');
        for (int s = 0; s < speechMessages.Length; s++)
        {
            string message; string characterName; Sprite characterSprite; Color backgroundColor;
            {
                message = speechMessages[s];
                // Remove ' ' from the beginning of the message
                if (message[0] == ' ')
                {
                    message = message.Remove(startIndex: 0, count: 1);
                }

                characterName = message.Split(':')[0];

                if (characterSpeechDataDictionary.ContainsKey(characterName))
                {
                    characterSprite = characterSpeechDataDictionary[characterName].Sprite;
                    backgroundColor = characterSpeechDataDictionary[characterName].BackgroundColor;
                }
                else
                {
                    L.ogWarning(this, "Couldn't find character sprite "+ characterName);
                    characterSprite = null;
                    backgroundColor = Color.black;
                }
            }

            // Instantiate and configure speech
            StorySpeech speech = Instantiate(speechPrototype);
            speech.Initialize(characterSprite, message, backgroundColor);
            speech.transform.SetParent(speechesLayout.transform);
            speech.gameObject.SetActive(true);
            speechesToClearWhenOpenning.Add(speech);
        }

        openerOfPopUpsMadeInEditor.OpenStoryPopUp();
    }

    public void OnCloseButtonClick()
    {
        openerOfPopUpsMadeInEditor.CloseAllPopUpsExceptLoading();
        sceneOpener.OpenMapScene();
    }
}
