using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Core.UI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Ship: MainMenu
{
    //public LevelSelectScreen levelSelectMenu;
    private UiManager _uiManager;

    private void Awake()
    {
        _uiManager = FindObjectOfType<UiManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Scene scene = SceneManager.GetActiveScene();
        if (other.tag == "Player")
        {
            //_uiManager.levelPanel.SetActive(true);
            //Enter();
            if (scene.buildIndex+1 == 2)
            {
                other.transform.position = new Vector3(0, 1, -16);
            }
            SceneManager.LoadScene(scene.buildIndex + 1);
        }
            
    }
    /*
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
            Exit();
    }
    public void Enter()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Debug.Log("Player Get Ship");
        ShowLevelSelectMenu();
    }

    public void ShowLevelSelectMenu()
    {
        ChangePage(levelSelectMenu);
    }
     
    public void Exit()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        DeactivateCurrentPage();
    }
    */
}
