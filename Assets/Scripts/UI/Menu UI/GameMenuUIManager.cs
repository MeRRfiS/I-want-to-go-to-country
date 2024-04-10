using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuUIManager : MonoBehaviour
{
    private const string MAIN_MENU = "Main Menu";

    private bool BlockerPlayerInputSystem() => true;
    private BlockUIEnum BlockerUIInputSystem() => BlockUIEnum.GameMenu;

    private void OnEnable()
    {
        PlayerInputSystem.BlockInputSystem += BlockerPlayerInputSystem;
        UIInputSystem.BlockInputSystem += BlockerUIInputSystem;
    }

    private void OnDisable()
    {
        PlayerInputSystem.BlockInputSystem -= BlockerPlayerInputSystem;
        UIInputSystem.BlockInputSystem -= BlockerUIInputSystem;
    }

    public void ReturnToGameButton()
    {
        UIController.GetInstance().SwitchActiveGameMenu();
    }

    public void ExitButton()
    {
        SceneManager.LoadScene(MAIN_MENU);
    }
}
