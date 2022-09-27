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
    public LevelSelectScreen levelSelectMenu;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
            Enter();
    }
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
