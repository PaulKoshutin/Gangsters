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
        transform.parent.Find("New Game Panel").gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
