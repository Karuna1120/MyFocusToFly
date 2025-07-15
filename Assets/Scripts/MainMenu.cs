using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public void StartFlyGame(int variant)
    {
        PlayerPrefs.SetInt("FakeVariant", variant);   
        SceneManager.LoadScene("GameSceneReal");
    }

    // Existing options you already had
    public void FlankerGame() => SceneManager.LoadScene("FlankerScene");
    public void QuitGame() => Application.Quit();
}
