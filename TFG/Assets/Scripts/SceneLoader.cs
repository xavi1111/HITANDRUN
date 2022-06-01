using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void loadFirstLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void loadTitleScreen()
    {
        SceneManager.LoadScene(0);
    }
    public void loadGameOverScreen()
    {
        SceneManager.LoadScene(2);
    }
    public void exitGame()
    {
        Application.Quit();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            loadGameOverScreen();
        }
    }
}
