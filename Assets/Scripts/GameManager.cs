using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    // ——— Gameplay reference ————————————
    public Player player;

    // ——— UI references ————————————————
    public TMP_Text scoreText;              // running score

    public GameObject titleText;        // <<< new (set it in Inspector)
    public GameObject playButton;

    public GameObject gameOverPanel;
    public GameObject restartButton;
    public GameObject summaryPanel;
    public TMP_Text summaryScoreText;

    // ——— Internal state ————————————————
    private int score = 0;
    private int highScore = 0;

    // ?????????????????????????????????????????
    private void Awake()
    {
        Application.targetFrameRate = 60;
        highScore = PlayerPrefs.GetInt("highScore", 0);
        Pause();                         // freeze on load
    }

    /*?????????????????????????????????????????
     *  BUTTON EVENTS
     *????????????????????????????????????????*/

    public void Play()
    {
        // Reset score & UI
        score = 0;
        scoreText.text = score.ToString();

        titleText.SetActive(false);     // hide title
        playButton.SetActive(false);
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        summaryPanel.SetActive(false);

        // Remove leftover towers
        foreach (Towers t in FindObjectsOfType<Towers>())
            Destroy(t.gameObject);

        // Start gameplay
        Time.timeScale = 1f;
        player.enabled = true;
    }

    public void GameOver()
    {
        Debug.Log("GameOver Triggered");
        Pause();

        restartButton.SetActive(true);
        gameOverPanel.SetActive(true);
        summaryPanel.SetActive(true);

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("highScore", highScore);
        }

        summaryScoreText.text = $"Score: {score}  |  High Score: {highScore}";
    }

    public void Restart() => Play();

    public void BackToMenu() =>
        SceneManager.LoadScene("MainMenu");

    /*?????????????????????????????????????????
     *  Helpers
     *????????????????????????????????????????*/

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        highScore = 0;
        Debug.Log("PlayerPrefs reset.");
    }

    private void Pause()
    {
        Time.timeScale = 0f;
        player.enabled = false;

        // Make sure title and Play button are visible when paused
        if (titleText) titleText.SetActive(true);
        playButton.SetActive(true);
    }
}
