using Core.UI;
using UnityEngine;

public class TFSMainMenu : MainMenu
{

    public OptionsMenu optionsMenu;
    public SimpleMainMenuPage titleMenu;
    public LevelSelectScreen levelSelectMenu;
    
    public void ShowOptionsMenu()
    {
        ChangePage(optionsMenu);
    }
    
    public void ShowLevelSelectMenu()
    {
        ChangePage(levelSelectMenu);
    }

    public void ShowTitleScreen()
    {
        Back(titleMenu);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    protected virtual void Awake()
    {
        ShowTitleScreen();
    }

    protected virtual void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
        {
            if ((SimpleMainMenuPage)m_CurrentPage == titleMenu)
            {
                Application.Quit();
            }
            else
            {
                Back();
            }
        }
    }
}
