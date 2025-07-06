using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    
    public Player player;

    
    public TMP_Text scoreText;              

    public GameObject titleText;        
    public GameObject playButton;

    public GameObject gameOverPanel;
    public GameObject restartButton;
    public GameObject summaryPanel;
    public TMP_Text summaryScoreText;
    public GameObject fakePlayerPrefab;
    private bool hasSpawnedFakes = false;

    private int score = 0;
    private int highScore = 0;

    private int lastFakeGroupScore = 0;


    private void Awake()
    {
        Application.targetFrameRate = 60;
        highScore = PlayerPrefs.GetInt("highScore", 0);
        Pause();                         // freeze on load
    }

    

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

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();

        if (score % 10 == 0 && score != lastFakeGroupScore)
        {
            lastFakeGroupScore = score;
            StartCoroutine(SpawnFakePlayers());
        }
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
    private IEnumerator SpawnFakePlayers()
    {
        float minY = Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).y;
        float maxY = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height)).y;

        for (int i = 0; i < 10; i++)
        {
            float randomY = Random.Range(minY, maxY);
            Vector3 spawnPos = new Vector3(-9f, randomY, 0);

            GameObject fake = Instantiate(fakePlayerPrefab, spawnPos, Quaternion.identity);
            

            yield return new WaitForSeconds(0.2f);
        }
    }

}
