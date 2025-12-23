using UnityEngine;


public class SceneManager : Singleton<SceneManager>
{
    [SerializeField] string mainSceneName = "Main";
    [SerializeField] string gameSceneName = "Game";
    [SerializeField] string deadSceneName = "Dead";
    [SerializeField] string endingSceneName = "Ending";

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

    public void GoToDead()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(deadSceneName);
    }

    public void GoToEnding()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(endingSceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
