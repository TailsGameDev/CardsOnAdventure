using UnityEngine;
using System.Collections;

public class PreMadeAudioFactory : MonoBehaviour
{
    [SerializeField]
    private AudioRequisitor audioRequisitor = null;

    [SerializeField]
    private AudioHolder audioHolder = null;

    public PreMadeAudioRequest CreateStopAllSFXRequest(GameObject assignor)
    {
        return PreMadeAudioRequest.CreateSTOP_SFXAudioRequest(audioRequisitor, assignor);
    }

    #region Created Using AudioClip
    public PreMadeAudioRequest CreateRandomPlaceCardAudioRequest(GameObject assignor)
    {
        string[] AUDIO_NAMES = { "0 Place Card SFX", "1 Place Card SFX",
            "2 Place Card SFX", "3 Place Card SFX", "4 Place Card SFX" };
        AudioClip placeCardSFX = audioHolder.GetAleatoryClipAmong(AUDIO_NAMES);

        return CreateSFXRequestUsingTheClip(placeCardSFX, assignor);
    }
    public PreMadeAudioRequest CreateCryingAudioRequest(GameObject assignor)
    {
        string[] AUDIO_NAMES = { "Sit And Cry", "Sit And Cry 2" };
        AudioClip cryingSFX = audioHolder.GetAleatoryClipAmong(AUDIO_NAMES);

        return CreateSFXRequestUsingTheClip(cryingSFX, assignor);
    }
    private PreMadeAudioRequest CreateSFXRequestUsingTheClip(AudioClip clip, GameObject assignor)
    {
        return PreMadeAudioRequest.CreateSFXSoundRequest(clip, audioRequisitor, assignor);
    }
    #endregion

    #region Created Using Name
    public PreMadeAudioRequest CreateDrinkAudioRequest(GameObject assignor)
    {
        return CreateSFXRequestUsingTheName(assignor, "Drink");
    }
    public PreMadeAudioRequest CreateFacepalmAudioRequest(GameObject assignor)
    {
        return CreateSFXRequestUsingTheName(assignor, "Facepalm");
    }
    public PreMadeAudioRequest CreateOffendAudioRequest(GameObject assignor)
    {
        return CreateSFXRequestUsingTheName(assignor, "Fuck You");
    }
    private PreMadeAudioRequest CreateSFXRequestUsingTheName(GameObject assignor, string audioName)
    {
        return PreMadeAudioRequest.CreateSFXSoundRequest
            (audioHolder.GetAudioByName(audioName), audioRequisitor, assignor);
    }
    #endregion

    #region Stop BGM
    public PreMadeAudioRequest CreateVictoryAudioRequest(GameObject assignor)
    {
        return CreateSFX_AND_STOP_BGMSoundRequest("VICTORY", assignor);
    }
    public PreMadeAudioRequest CreateDefeatAudioRequest(GameObject assignor)
    {
        return CreateSFX_AND_STOP_BGMSoundRequest("DEFEAT", assignor);
    }
    private PreMadeAudioRequest CreateSFX_AND_STOP_BGMSoundRequest(string audioName, GameObject assignor)
    {
        AudioClip defeatBGM = audioHolder.GetAudioByName("DEFEAT");
        PreMadeAudioRequest preMadeAudioRequest =
            PreMadeAudioRequest.CreateSFX_AND_STOP_BGMSoundRequest(defeatBGM, audioRequisitor, assignor);
        return preMadeAudioRequest;
    }
    #endregion
}
