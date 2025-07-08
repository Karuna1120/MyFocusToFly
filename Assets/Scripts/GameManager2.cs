using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager2 : MonoBehaviour
{
    /*????????????????????????????  PUBLIC UI REFERENCES  ???????????????????????????*/
    public Player player;
    public TMP_Text scoreText;
    public GameObject titleText;
    public GameObject playButton;
    public GameObject gameOverPanel;
    public GameObject restartButton;
    public GameObject summaryPanel;
    public TMP_Text summaryScoreText;

    /*????????????????????????????  VARIANT PREFABS (drag in Inspector) ??????????????*/
    [Header("Variant-1 Prefabs")]
    public GameObject fakePlayer1Prefab;   // moving fake target
    public GameObject fakeShadow1Prefab;   // mirror follower

    [Header("Variant-2 Prefabs")]
    public GameObject fakePlayer2Prefab;
    public GameObject fakeShadow2Prefab;

    /*????????????????????????????  INTERNAL ACTIVE PREFABS  ?????????????????????????*/
    private GameObject fakePlayerPrefab;   // points to *current* variant
    private GameObject fakeShadowPrefab;

    /*????????????????????????????  GAME STATE  ?????????????????????????*/
    private int score = 0;
    private int highScore = 0;

    private Coroutine groupSpawnRoutine;
    private Coroutine singleSpawnRoutine;
    private Coroutine mirrorRoutine;

    /*????????????????????????????  UNITY LIFECYCLE  ????????????????????*/
    private void Awake()
    {
        Application.targetFrameRate = 60;

        // Retrieve the player’s menu choice (default = 1)
        int variant = PlayerPrefs.GetInt("FakeVariant", 1);

        if (variant == 1)
        {
            fakePlayerPrefab = fakePlayer1Prefab;
            fakeShadowPrefab = fakeShadow1Prefab;
        }
        else
        {
            fakePlayerPrefab = fakePlayer2Prefab;
            fakeShadowPrefab = fakeShadow2Prefab;
        }

        highScore = PlayerPrefs.GetInt("highScore", 0);
        Pause();            // start in menu state
    }

    /*????????????????????????????  PUBLIC BUTTON HOOKS  ???????????????????????????*/
    public void Play()
    {
        // Clean up leftovers from previous run
        foreach (var f in FindObjectsOfType<FakePlayerMover>()) Destroy(f.gameObject);
        foreach (var s in FindObjectsOfType<FakeShadowPlayer>()) Destroy(s.gameObject);
        foreach (var t in FindObjectsOfType<Towers>()) Destroy(t.gameObject);

        score = 0;
        scoreText.text = score.ToString();

        titleText.SetActive(false);
        playButton.SetActive(false);
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        summaryPanel.SetActive(false);

        Time.timeScale = 1f;
        player.enabled = true;

        groupSpawnRoutine = StartCoroutine(LoopSpawnFakeGroup());
        singleSpawnRoutine = StartCoroutine(LoopSpawnFakeSingle());
        mirrorRoutine = StartCoroutine(LoopSpawnFakeShadow());
    }

    public void GameOver()
    {
        Pause();                       // stop time & player
        restartButton.SetActive(true);
        gameOverPanel.SetActive(true);
        summaryPanel.SetActive(true);

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("highScore", highScore);
        }

        summaryScoreText.text = $"Score: {score}  |  High Score: {highScore}";

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
    }

    /*????????????????????????????  PRIVATE HELPERS  ???????????????????????????????*/
    private void Pause()
    {
        Time.timeScale = 0f;
        player.enabled = false;

        if (titleText) titleText.SetActive(true);
        playButton.SetActive(true);
    }

    /*??????????????????????????  COROUTINES  ??????????????????????????*/
    private IEnumerator LoopSpawnFakeGroup()
    {
        while (true)
        {
            yield return new WaitForSeconds(6f);
            yield return StartCoroutine(SpawnFakePlayers());
        }
    }

    private IEnumerator LoopSpawnFakeSingle()
    {
        while (true)
        {
            yield return new WaitForSeconds(8f);
            SpawnTargetFakePlayer();
        }
    }

    private IEnumerator LoopSpawnFakeShadow()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            // spawn the prefab at the player’s position
            GameObject fake = Instantiate(fakeShadowPrefab,
                                          player.transform.position,
                                          Quaternion.identity);

            // simply tell its script who to follow
            FakeShadowPlayer sp = fake.GetComponent<FakeShadowPlayer>();
            if (sp != null) sp.player = player.transform;

            yield return new WaitForSeconds(3f);
            if (fake) Destroy(fake);   // vanish after 3 s
        }
    }

    private IEnumerator SpawnFakePlayers()
    {
        float minY = Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).y;
        float maxY = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height)).y;

        for (int i = 0; i < 15; i++)
        {
            float randomY = Random.Range(minY, maxY);
            Vector3 spawnPos = new Vector3(-9f, randomY, 0);

            GameObject fake = Instantiate(fakePlayerPrefab, spawnPos, Quaternion.identity);
            fake.GetComponent<FakePlayerMover>().speed = Random.Range(1f, 5f);

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void SpawnTargetFakePlayer()
    {
        Vector3 spawnPos = player.transform.position;
        GameObject fake = Instantiate(fakePlayerPrefab, spawnPos, Quaternion.identity);
        fake.GetComponent<FakePlayerMover>().speed = Random.Range(1f, 5f);
    }
}
