using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class FlankerTaskManager : MonoBehaviour
{
    /*?????????????????????????? UI REFERENCES ??????????????????????????*/
    [Header("UI References (drag in Inspector)")]
    public GameObject rulePanel;          // “Press space to start” panel
    public TMP_Text stimulusText;       // Shows 5-arrow stimulus
    public TMP_Text feedbackText;       // Green “Correct”
    public GameObject summaryPanel;       // Final stats screen
    public TMP_Text summaryText;
    public Button button0;            // On-screen “0”  (? Right)
    public Button button1;            // On-screen “1”  (? Left)
    public GameObject errorPanel;         // Wrong-answer reminder
    public GameObject timeoutPanel;       // Slow-response reminder

    /* Crosses that must blink red ? white */
    public TMP_Text errorCrossText;     // Inside ErrorPanel
    public TMP_Text timeoutCrossText;   // Inside TimeoutPanel

    /*?????????????????????????? TASK SETTINGS ??????????????????????????*/
    [Header("Task Settings")]
    public int practiceTrialsCount = 5;
    public int testTrialsCount = 40;
    public float feedbackDuration = 0.5f;   // seconds
    public float ruleReminderDuration = 2f;     // seconds
    public float timeoutSeconds = 3f;     // response deadline
    public float crossBlinkInterval = 0.25f;  // red/white toggle speed

    public void BackToMenu() => SceneManager.LoadScene("MainMenu");

    /*?????????????????????????? INTERNAL DATA ??????????????????????????*/
    private class TrialData
    {
        public string stimulus;
        public char correctAnswer;
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

    private readonly List<TrialData> trials = new();
    private readonly List<ResultData> results = new();
    private int currentTrialIndex = 0;
    private float trialStartTime;
    private bool awaitingInput = false;
    private bool gameStarted = false;

    private Coroutine activeBlinkRoutine;         // handle for blinking coroutine

    /*?????????????????????????? UNITY EVENTS ??????????????????????????*/
    private void Start()
    {
        // initial UI state
        rulePanel.SetActive(true);
        feedbackText.gameObject.SetActive(false);
        summaryPanel.SetActive(false);
        stimulusText.gameObject.SetActive(false);
        errorPanel.SetActive(false);
        timeoutPanel.SetActive(false);

        // on-screen buttons
        button0.onClick.AddListener(() => OnAnswer('0'));   // Right
        button1.onClick.AddListener(() => OnAnswer('1'));   // Left
    }

    private void Update()
    {
        /* ?? Wait for player to start ?? */
        if (!gameStarted &&
            (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            StartTask();
        }

        /* ?? During a trial ?? */
        if (!awaitingInput) return;

        // check timeout
        if (Time.time - trialStartTime > timeoutSeconds)
        {
            StartCoroutine(ShowTimeout());
            return;
        }

        // keyboard answers
        if (Input.GetKeyDown(KeyCode.Alpha0)) OnAnswer('0');
        else if (Input.GetKeyDown(KeyCode.Alpha1)) OnAnswer('1');
    }

    /*?????????????????????????? TASK FLOW ??????????????????????????*/
    private void StartTask()
    {
        rulePanel.SetActive(false);
        stimulusText.gameObject.SetActive(true);
        gameStarted = true;

        GenerateTrials();
        ShowNextTrial();
    }

    private void GenerateTrials()
    {
        string L = "\u2190";                // ?
        string R = "\u2192";                // ?

        string[] stimuli = {
            new string(L[0], 5),            // ?????  (centre left)
            new string(R[0], 5),            // ?????  (centre right)
            R + R + L + R + R,              // ?????  (incongruent left)
            L + L + R + L + L               // ?????  (incongruent right)
        };

        /* 1 = Left, 0 = Right */
        char[] correctAnswers = { '1', '0', '1', '0' };
        bool[] congruentFlags = { true, true, false, false };

        trials.Clear();
        for (int i = 0; i < practiceTrialsCount + testTrialsCount; i++)
        {
            bool isPractice = i < practiceTrialsCount;
            int idx = Random.Range(0, stimuli.Length);

            trials.Add(new TrialData
            {
                stimulus = stimuli[idx],
                correctAnswer = correctAnswers[idx],
                isCongruent = congruentFlags[idx],
                isPractice = isPractice
            });
        }
    }

    private void ShowNextTrial()
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
        bool ok = (answer == trial.correctAnswer);

        results.Add(new ResultData
        {
            isCorrect = ok,
            reactionTime = rt,
            isCongruent = trial.isCongruent,
            isPractice = trial.isPractice
        });

        if (ok)
            StartCoroutine(ShowFeedback("Correct", Color.green, feedbackDuration));
        else
            StartCoroutine(ShowError());
    }

    /*?????????????????????????? COROUTINES ??????????????????????????*/
    private IEnumerator ShowFeedback(string msg, Color col, float dur)
    {
        feedbackText.text = msg;
        feedbackText.color = col;
        feedbackText.gameObject.SetActive(true);

        yield return new WaitForSeconds(dur);

        feedbackText.gameObject.SetActive(false);
        currentTrialIndex++;
        ShowNextTrial();
    }

    private IEnumerator ShowError()
    {
        errorPanel.SetActive(true);
        if (errorCrossText != null)
            activeBlinkRoutine = StartCoroutine(BlinkCross(errorCrossText));

        yield return new WaitForSeconds(ruleReminderDuration);

        if (activeBlinkRoutine != null) StopCoroutine(activeBlinkRoutine);
        if (errorCrossText != null) errorCrossText.color = Color.red;

        errorPanel.SetActive(false);
        currentTrialIndex++;
        ShowNextTrial();
    }

    private IEnumerator ShowTimeout()
    {
        awaitingInput = false;

        /* log timeout as incorrect */
        var trial = trials[currentTrialIndex];
        results.Add(new ResultData
        {
            isCorrect = false,
            reactionTime = timeoutSeconds,
            isCongruent = trial.isCongruent,
            isPractice = trial.isPractice
        });

        timeoutPanel.SetActive(true);
        if (timeoutCrossText != null)
            activeBlinkRoutine = StartCoroutine(BlinkCross(timeoutCrossText));

        yield return new WaitForSeconds(ruleReminderDuration);

        if (activeBlinkRoutine != null) StopCoroutine(activeBlinkRoutine);
        if (timeoutCrossText != null) timeoutCrossText.color = Color.red;

        timeoutPanel.SetActive(false);
        currentTrialIndex++;
        ShowNextTrial();
    }

    private IEnumerator BlinkCross(TMP_Text cross)
    {
        bool red = false;
        while (true)
        {
            cross.color = red ? Color.red : Color.white;
            red = !red;
            yield return new WaitForSeconds(crossBlinkInterval);
        }
    }

    /*?????????????????????????? SUMMARY ??????????????????????????*/
    private void ShowSummary()
    {
        int correct = 0;
        float totalRT = 0, congRT = 0, incongRT = 0;
        int totalN = 0, congN = 0, incongN = 0;

        foreach (var r in results)
        {
            if (r.isPractice) continue;

            totalN++;
            if (r.isCorrect) correct++;

            totalRT += r.reactionTime;
            if (r.isCongruent) { congRT += r.reactionTime; congN++; }
            else { incongRT += r.reactionTime; incongN++; }
        }

        float avgRT = totalN > 0 ? totalRT / totalN : 0;
        float avgCongRT = congN > 0 ? congRT / congN : 0;
        float avgIncong = incongN > 0 ? incongRT / incongN : 0;
        float diffRT = avgIncong - avgCongRT;
        float accuracy = totalN > 0 ? (float)correct / totalN * 100f : 0f;

        summaryText.text =
            $"Flanker Task Summary\n" +
            "------------------------------\n" +
            $"Practice Trials: {practiceTrialsCount} (not scored)\n\n" +
            $"Test Trials: {testTrialsCount}\n" +
            $"Correct:   {correct}\n" +
            $"Incorrect: {testTrialsCount - correct}\n" +
            $"Accuracy:  {accuracy:F1}%\n\n" +
            $"Avg RT:          {avgRT * 1000f:F0} ms\n" +
            $"Congruent RT:    {avgCongRT * 1000f:F0} ms\n" +
            $"Incongruent RT:  {avgIncong * 1000f:F0} ms\n" +
            $"RT Difference:   {diffRT * 1000f:F0} ms";

        summaryPanel.SetActive(true);
    }
}
