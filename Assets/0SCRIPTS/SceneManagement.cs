using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public void StartGame()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1) gamemanager.userInterface.ButtonClickAudio();
        SoundManager.Instance.musicSounds.Stop();
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SoundManager.Instance.musicSounds.Stop();
        if (gamemanager.userInterface.score > MultiScene.multiScene.highScore) MultiScene.multiScene.highScore = gamemanager.userInterface.score;
        SceneManager.LoadScene(0);
        SoundManager.Instance.musicSounds2.Stop();
    }

    public void ExitGame()
    {
        gamemanager.userInterface.ButtonClickAudio();
        Application.Quit();
    }
}
