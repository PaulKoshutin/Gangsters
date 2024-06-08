using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroButtons : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }
    public void ToLoad()
    {
        SaveLoader.Instance.loading = true;
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }
    public void ToNewGame()
    {
        //SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        //transform.Find("Event Panel").gameObject.SetActive(false);
        transform.parent.Find("New Game Panel").gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
