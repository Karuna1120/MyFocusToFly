// Updated FlankerTaskManager.cs with Practice + Test Phases
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class FlankerTaskManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject rulePanel;
    public TMP_Text stimulusText;
    public TMP_Text feedbackText;
    public GameObject summaryPanel;
    public TMP_Text summaryText;
    public Button button0;
    public Button button1;

    [Header("Settings")]
    public int practiceTrialsCount = 5;
    public int testTrialsCount = 35;
    public float feedbackDuration = 0.5f;
    public float ruleReminderDuration = 2f;
    public void BackToMenu() => SceneManager.LoadScene("MainMenu");

    private class TrialData
    {
        public string stimulus;
        public char correctAnswer; // '0' or '1'
        public bool isCongruent;
        public bool isPractice;
    }

    private class ResultData
    {
        public bool isCorrect;
        public float reactionTime;
        public bool isCongruent;
        public bool isPractice;
    }

    private List<TrialData> trials = new List<TrialData>();
    private List<ResultData> results = new List<ResultData>();

    private int currentTrialIndex = 0;
    private float trialStartTime;
    private bool awaitingInput = false;
    private bool gameStarted = false;

    void Start()
    {
        rulePanel.SetActive(true);
        feedbackText.gameObject.SetActive(false);
        summaryPanel.SetActive(false);
        stimulusText.gameObject.SetActive(false);

        button0.onClick.AddListener(() => OnAnswer('0'));
        button1.onClick.AddListener(() => OnAnswer('1'));
    }

    void Update()
    {
        if (!gameStarted && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            StartTask();
        }

        if (awaitingInput)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0)) OnAnswer('0');
            else if (Input.GetKeyDown(KeyCode.Alpha1)) OnAnswer('1');
        }
    }

    void StartTask()
    {
        rulePanel.SetActive(false);
        stimulusText.gameObject.SetActive(true);
        gameStarted = true;
        GenerateTrials();
        ShowNextTrial();
    }

    void GenerateTrials()
    {
        char[] group0 = { 'C', 'S' };
        char[] group1 = { 'H', 'K' };

        for (int i = 0; i < practiceTrialsCount + testTrialsCount; i++)
        {
            bool isPractice = i < practiceTrialsCount;
            bool isCongruent = Random.value > 0.5f;
            char center = Random.value > 0.5f ? group0[Random.Range(0, 2)] : group1[Random.Range(0, 2)];
            char flank = isCongruent ? center : (System.Array.Exists(group0, c => c == center) ? group1[Random.Range(0, 2)] : group0[Random.Range(0, 2)]);

            string stim = $"{flank}{flank}{center}{flank}{flank}";
            char correctAnswer = (System.Array.Exists(group1, c => c == center)) ? '1' : '0';

            trials.Add(new TrialData { stimulus = stim, correctAnswer = correctAnswer, isCongruent = isCongruent, isPractice = isPractice });
        }
    }

    void ShowNextTrial()
    {
        if (currentTrialIndex >= trials.Count)
        {
            ShowSummary();
            return;
        }

        var trial = trials[currentTrialIndex];
        stimulusText.text = trial.stimulus;
        trialStartTime = Time.time;
        awaitingInput = true;
    }

    public void OnAnswer(char answer)
    {
        if (!awaitingInput) return;

        awaitingInput = false;
        var trial = trials[currentTrialIndex];
        float rt = Time.time - trialStartTime;
        bool isCorrect = (answer == trial.correctAnswer);

        results.Add(new ResultData
        {
            isCorrect = isCorrect,
            reactionTime = rt,
            isCongruent = trial.isCongruent,
            isPractice = trial.isPractice
        });

        if (isCorrect)
        {
            StartCoroutine(ShowFeedback("Correct", Color.green, feedbackDuration));
        }
        else
        {
            StartCoroutine(ShowRuleReminder());
        }
    }

    IEnumerator ShowFeedback(string text, Color color, float duration)
    {
        feedbackText.text = text;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        feedbackText.gameObject.SetActive(false);

        currentTrialIndex++;
        ShowNextTrial();
    }

    IEnumerator ShowRuleReminder()
    {
        rulePanel.SetActive(true);
        yield return new WaitForSeconds(ruleReminderDuration);
        rulePanel.SetActive(false);

        currentTrialIndex++;
        ShowNextTrial();
    }

    void ShowSummary()
    {
        // Show back button after summary
        Button backButton = summaryPanel.GetComponentInChildren<Button>();
        if (backButton != null) backButton.gameObject.SetActive(true);
        int correct = 0;
        float totalRT = 0f, congruentRT = 0f, incongruentRT = 0f;
        int totalCount = 0, congruentCount = 0, incongruentCount = 0;

        foreach (var r in results)
        {
            if (r.isPractice) continue;

            totalCount++;
            if (r.isCorrect) correct++;
            totalRT += r.reactionTime;
            if (r.isCongruent)
            {
                congruentRT += r.reactionTime;
                congruentCount++;
            }
            else
            {
                incongruentRT += r.reactionTime;
                incongruentCount++;
            }
        }

        float avgRT = totalCount > 0 ? totalRT / totalCount : 0f;
        float avgCongRT = congruentCount > 0 ? congruentRT / congruentCount : 0f;
        float avgIncongRT = incongruentCount > 0 ? incongruentRT / incongruentCount : 0f;
        float diffRT = avgIncongRT - avgCongRT;
        float accuracy = totalCount > 0 ? (float)correct / totalCount * 100f : 0f;

        summaryText.text =
            $"Flanker Task Summary\n" +
            "-------------------------\n" +
            $"Practice Trials: {practiceTrialsCount} (not scored)\n\n" +
            $"Test Trials: {testTrialsCount}\n" +
            $"Correct: {correct}\n" +
            $"Incorrect: {testTrialsCount - correct}\n" +
            $"Accuracy: {accuracy:F1}%\n\n" +
            $"Avg RT: {avgRT * 1000f:F0} ms\n" +
            $"Congruent RT: {avgCongRT * 1000f:F0} ms\n" +
            $"Incongruent RT: {avgIncongRT * 1000f:F0} ms\n" +
            $"RT Difference: {diffRT * 1000f:F0} ms";

        summaryPanel.SetActive(true);
    }
}
