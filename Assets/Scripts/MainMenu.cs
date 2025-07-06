using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void FlyGame()
    {
       
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameSceneReal");
        Debug.Log("Start Game");
    }
    public void FlankerGame()
    {

        UnityEngine.SceneManagement.SceneManager.LoadScene("FlankerScene");
        Debug.Log("Flanker Game");
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
