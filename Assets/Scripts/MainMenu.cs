using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Called by the “Medium 1” and “Medium 2” buttons.
    /// The button’s OnClick passes either 1 or 2.
    /// </summary>
    public void StartFlyGame(int variant)
    {
        PlayerPrefs.SetInt("FakeVariant", variant);   // remember the choice
        SceneManager.LoadScene("GameSceneReal");
    }

    // Existing options you already had
    public void FlankerGame() => SceneManager.LoadScene("FlankerScene");
    public void QuitGame() => Application.Quit();
}
