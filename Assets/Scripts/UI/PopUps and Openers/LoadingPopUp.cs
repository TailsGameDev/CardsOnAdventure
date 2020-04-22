using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingPopUp : MonoBehaviour
{
    [SerializeField]
    private GameObject loadingPopUp = null;

    void Awake()
    {
        SceneManager.sceneLoaded += OnLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnLoaded;
    }

    void OnLoaded(Scene scene, LoadSceneMode mode)
    {
        loadingPopUp.SetActive(false);
    }
}
