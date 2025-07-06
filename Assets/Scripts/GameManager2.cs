using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager2 : MonoBehaviour
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
    public GameObject fakeShadowPrefab;

    private int score = 0;
    private int highScore = 0;

    private Coroutine groupSpawnRoutine;
    private Coroutine singleSpawnRoutine;
    private Coroutine mirrorRoutine;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        highScore = PlayerPrefs.GetInt("highScore", 0);
        Pause();
    }

    public void Play()
    {
        score = 0;
        scoreText.text = score.ToString();

        titleText.SetActive(false);
        playButton.SetActive(false);
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        summaryPanel.SetActive(false);

        foreach (Towers t in FindObjectsOfType<Towers>())
            Destroy(t.gameObject);

        Time.timeScale = 1f;
        player.enabled = true;

        // Start all fake spawn loops
        groupSpawnRoutine = StartCoroutine(LoopSpawnFakeGroup());
        singleSpawnRoutine = StartCoroutine(LoopSpawnFakeSingle());
        mirrorRoutine = StartCoroutine(LoopSpawnFakeShadow());
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

        // Stop coroutines
        if (groupSpawnRoutine != null) StopCoroutine(groupSpawnRoutine);
        if (singleSpawnRoutine != null) StopCoroutine(singleSpawnRoutine);
        if (mirrorRoutine != null) StopCoroutine(mirrorRoutine);
    }

    public void Restart() => Play();
    public void BackToMenu() => SceneManager.LoadScene("MainMenu");

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

        if (titleText) titleText.SetActive(true);
        playButton.SetActive(true);
    }

    // ?? Loop: spawn 10 fake players every 5 seconds
    private IEnumerator LoopSpawnFakeGroup()
    {
        while (true)
        {
            yield return new WaitForSeconds(6f);
            StartCoroutine(SpawnFakePlayers());
        }
    }

    // ?? Loop: spawn 1 fake player near player every 8 seconds
    private IEnumerator LoopSpawnFakeSingle()
    {
        while (true)
        {
            yield return new WaitForSeconds(8f);
            SpawnTargetFakePlayer();
        }
    }

    // ?? Loop: spawn 1 mirror fake near player every 3 seconds, lasting 5 seconds
    private IEnumerator LoopSpawnFakeShadow()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            GameObject fake = Instantiate(fakeShadowPrefab, player.transform.position, Quaternion.identity);

            var controller = fake.AddComponent<FakeShadowPlayer>();
            controller.player = player.transform;
            controller.sprites = player.GetComponent<Player>().sprites;

            yield return new WaitForSeconds(3f);
            if (fake != null) Destroy(fake);
        }
    }

    // ?? Spawn 10 fake players at random heights
    private IEnumerator SpawnFakePlayers()
    {
        float minY = Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).y;
        float maxY = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height)).y;

        for (int i = 0; i < 25; i++)
        {
            float randomY = Random.Range(minY, maxY);
            Vector3 spawnPos = new Vector3(-9f, randomY, 0);

            GameObject fake = Instantiate(fakePlayerPrefab, spawnPos, Quaternion.identity);
            fake.GetComponent<FakePlayerMover>().speed = Random.Range(1f, 5f);

            yield return new WaitForSeconds(0.2f);
        }
    }

    // ?? Spawn 1 fake player at the real player's position
    private void SpawnTargetFakePlayer()
    {
        Vector3 spawnPos = player.transform.position;
        GameObject fake = Instantiate(fakePlayerPrefab, spawnPos, Quaternion.identity);
        fake.GetComponent<FakePlayerMover>().speed = Random.Range(1f, 5f);
    }
}
