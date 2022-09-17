using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public string sceneToLoadName = "LevelSelect";

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoadName);
    }

    public void RestartCurrentScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
}