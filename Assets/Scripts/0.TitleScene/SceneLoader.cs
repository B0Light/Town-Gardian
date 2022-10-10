using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Name of the scene to load
    /// </summary>
    public string sceneToLoadName = "LevelSelect";

    private UiManager _uiManager;

    /// <summary>
    /// Loads the scene from <see cref="sceneToLoadName" />
    /// if a scene with that name exists
    /// </summary>
    public void Start()
    {
        _uiManager = FindObjectOfType<UiManager>();
    }

    public void LoadScene()
    {
        _uiManager.levelPanel.SetActive(false);
        SceneManager.LoadScene(sceneToLoadName);
    }

    /// <summary>
    /// Restarts the current scene
    /// </summary>
    public void RestartCurrentScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
}