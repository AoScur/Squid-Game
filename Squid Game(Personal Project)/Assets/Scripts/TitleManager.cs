using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void OnOption()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
