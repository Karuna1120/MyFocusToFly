// GameManager2.cs with Black Flash Panel and Fake Interference
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
    public GameObject blackFlashPanel; // full-screen black panel


    private int score = 0;
    private int highScore = 0;

    private Coroutine groupSpawnRoutine;
    private Coroutine singleSpawnRoutine;
    private Coroutine mirrorRoutine;
    private Coroutine flashRoutine;  


    private void Awake()
    {
        Application.targetFrameRate = 60;
        highScore = PlayerPrefs.GetInt("highScore", 0);
        Pause();


    }

    public void Play()
    {
        // Clean up old fake players
        foreach (var f in FindObjectsOfType<FakePlayerMover>()) Destroy(f.gameObject);
        foreach (var s in FindObjectsOfType<FakeShadowPlayer>()) Destroy(s.gameObject);

        score = 0;
        scoreText.text = score.ToString();

        titleText.SetActive(false);
        playButton.SetActive(false);
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        summaryPanel.SetActive(false);

        foreach (Towers t in FindObjectsOfType<Towers>()) Destroy(t.gameObject);

        Time.timeScale = 1f;
        player.enabled = true;

        // Start all fake spawners
        groupSpawnRoutine = StartCoroutine(LoopSpawnFakeGroup());
        singleSpawnRoutine = StartCoroutine(LoopSpawnFakeSingle());
        mirrorRoutine = StartCoroutine(LoopSpawnFakeShadow());
        flashRoutine = StartCoroutine(LoopRandomBlackFlash());
    }

    public void GameOver()
    {
        Pause();

        restartButton.SetActive(true);
        gameOverPanel.SetActive(true);
        summaryPanel.SetActive(true);
        if (blackFlashPanel != null) blackFlashPanel.SetActive(false);


        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("highScore", highScore);
        }

        summaryScoreText.text = $"Score: {score}  |  High Score: {highScore}";

        // Stop routines
        if (groupSpawnRoutine != null) StopCoroutine(groupSpawnRoutine);
        if (singleSpawnRoutine != null) StopCoroutine(singleSpawnRoutine);
        if (mirrorRoutine != null) StopCoroutine(mirrorRoutine);
        if (flashRoutine != null) StopCoroutine(flashRoutine);
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
    }

    private void Pause()
    {
        Time.timeScale = 0f;
        player.enabled = false;

        if (titleText) titleText.SetActive(true);
        playButton.SetActive(true);
    }

    // Spawn 25 fake players at random heights every few seconds
    private IEnumerator LoopSpawnFakeGroup()
    {
        while (true)
        {
            yield return new WaitForSeconds(6f);
            StartCoroutine(SpawnFakePlayers());
        }
    }

    // Spawn 1 fake near player every 8 seconds
    private IEnumerator LoopSpawnFakeSingle()
    {
        while (true)
        {
            yield return new WaitForSeconds(8f);
            SpawnTargetFakePlayer();
        }
    }

    // Spawn 1 mirror fake that mimics player every 3 seconds for 3 seconds
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

    // Random black screen that flashes and stays for 4 seconds
    private IEnumerator LoopRandomBlackFlash()
    {
        Image img = blackFlashPanel.GetComponent<Image>();
        if (img == null) yield break;

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 10f));
            blackFlashPanel.SetActive(true);



            float flashDuration = 4f;
            float flashInterval = 0.2f;
            float timer = 0f;
            bool visible = true;

            // 4 sec
            while (timer < flashDuration)
            {
                float alpha = visible ? 1f : 0f;  // color alpha value
                img.color = new Color(0f, 0f, 0f, alpha);
                visible = !visible;

                yield return new WaitForSeconds(flashInterval);
                timer += flashInterval;
            }


            // hide the panel after flashing
            blackFlashPanel.SetActive(false);
        }
    }

    // Spawns a group of 25 fake players
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

    // Spawn 1 fake player at player's position
    private void SpawnTargetFakePlayer()
    {
        Vector3 spawnPos = player.transform.position;
        GameObject fake = Instantiate(fakePlayerPrefab, spawnPos, Quaternion.identity);
        fake.GetComponent<FakePlayerMover>().speed = Random.Range(1f, 5f);
    }
}
