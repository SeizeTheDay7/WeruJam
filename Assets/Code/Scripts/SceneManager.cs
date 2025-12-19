using UnityEngine;


public class SceneManager : Singleton<SceneManager>
{
    [SerializeField] string mainSceneName = "Main";
    [SerializeField] string gameSceneName = "Game";

    [Button]
    public void GoToMain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainSceneName);
    }

    [Button]
    public void GoToGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
