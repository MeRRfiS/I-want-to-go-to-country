using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIManager : MonoBehaviour
{
    //ToDo: change scene after made a map
    private const string PLAY_SCENE = "Test Scene";

    public void PlayButton()
    {
        SceneManager.LoadScene(PLAY_SCENE);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
