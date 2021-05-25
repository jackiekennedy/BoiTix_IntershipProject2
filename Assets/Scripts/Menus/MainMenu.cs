using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Canvas mainMenuCanvas;
    [SerializeField] private Canvas levelsCanvas;

    public void StartButtonClick()
    {
        MenuLevelsProgress.Instance.SetLevelButtonsStatus();
        mainMenuCanvas.enabled = false;
        levelsCanvas.enabled = true;
    }

    public void BackButtonClick()
    {
        mainMenuCanvas.enabled = true;
        levelsCanvas.enabled = false;
    }

    public void LevelButtonLoad(int levelID)
    {
        SceneManager.LoadScene(levelID + "_Level");
    }
}
