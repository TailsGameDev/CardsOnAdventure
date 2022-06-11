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
    private TransformWrapper speechesLayoutTransformWrapper;

    [SerializeField] private StorySpeech speechPrototype = null;

    [FormerlySerializedAs("characterSprites")]
    [SerializeField] private CharacterSpeechData[] characterSpeechDataArray = null;

    [SerializeField]
    private Scrollbar scrollbar = null;

    [SerializeField] private Text tapToContinueText = null;
    [SerializeField] private Animator tapToContinueAnimator = null;
    [SerializeField] private string tapToContinueMessage = null;
    [SerializeField] private string endStoryMessage = null;
    private float lastTapDownTime;

    private Dictionary<string, CharacterSpeechData> characterSpeechDataDictionary = new Dictionary<string, CharacterSpeechData>();

    private List<StorySpeech> cachedStorySpeeches = new List<StorySpeech>();

    private int nextIndexToDisplay;

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

            speechesLayoutTransformWrapper = new TransformWrapper(speechesLayout.transform);
        }
    }

    public void Open(string story)
    {
        this.nextIndexToDisplay = 0;

        tapToContinueText.text = tapToContinueMessage;
        tapToContinueAnimator.enabled = true;

        // Destroy speeches from last time openned
        for (int s = cachedStorySpeeches.Count-1; s >= 0; s--)
        {
            StorySpeech speech = cachedStorySpeeches[s];
            cachedStorySpeeches.RemoveAt(s);
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
            speech.Initialize(characterSprite, message, backgroundColor, speechesLayoutTransformWrapper);
            cachedStorySpeeches.Add(speech);
        }

        ActivateNextSpeechAndConfigureUI();

        openerOfPopUpsMadeInEditor.OpenStoryPopUp();
    }

    public void OnCloseButtonClick()
    {
        openerOfPopUpsMadeInEditor.CloseAllPopUpsExceptLoading();
        sceneOpener.OpenMapScene();
    }

    public void OnTapDown()
    {
        lastTapDownTime = Time.time;
    }
    public void OnTapUp()
    {
        const float TAP_MAX_TIME_INTERVAL = 0.2f;
        bool isQuickTap = Time.time - lastTapDownTime < TAP_MAX_TIME_INTERVAL;
        if (isQuickTap)
        {
            ActivateNextSpeechAndConfigureUI();
        }
    }

    private void ActivateNextSpeechAndConfigureUI()
    {
        if (nextIndexToDisplay < cachedStorySpeeches.Count)
        {
            cachedStorySpeeches[nextIndexToDisplay].SetGameObjectActive(true);
            nextIndexToDisplay++;
        }

        // Comunicate to the player that the story has ended if needed
        bool isTheLastSpeech = nextIndexToDisplay == (cachedStorySpeeches.Count);
        if (isTheLastSpeech)
        {
            tapToContinueText.text = endStoryMessage;
            // Animate the text so we drag player's attention to it
            tapToContinueAnimator.enabled = true;
        }
        else if (nextIndexToDisplay > 2)
        {
            // As it's not the first nor the second tap anymore let's presume the player does not need the
            // animation on the tapToContinueText that teaches him to tap to continue!
            tapToContinueAnimator.enabled = false;
            tapToContinueText.color = Color.white;
        }

        // Scroll to the end of the story
        scrollbar.value = 0;
    }
}
