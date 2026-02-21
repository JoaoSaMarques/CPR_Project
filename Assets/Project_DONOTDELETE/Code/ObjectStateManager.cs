using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneStateManager : MonoBehaviour
{
    [Header("Scene Reference (Drag Scene Here)")]
#if UNITY_EDITOR
    [SerializeField] private SceneAsset nextSceneAsset;
#endif
    private string nextSceneName;

    [Header("Countdown Settings")]
    [SerializeField] private GameObject countdownUI;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private float countdownDuration = 5f;

    [Header("Gameplay Duration")]
    [SerializeField] private float sceneDuration = 60f;

    [Header("Scene Timer UI")]
    [SerializeField] private TMP_Text sceneTimerText;

    [Header("Additional Texts (Show During Scene)")]
    [SerializeField] private TMP_Text[] extraTexts; // Any other texts you want to show temporarily

    [Header("Scene Switching")]
    [SerializeField] private bool switchSceneAfterTimer = true;

    private void Awake()
    {
#if UNITY_EDITOR
        if (nextSceneAsset != null)
            nextSceneName = nextSceneAsset.name;
#endif
    }

    private void Start()
    {
        StartCoroutine(SceneFlow());
    }

    private IEnumerator SceneFlow()
    {
        // Freeze time during countdown
        Time.timeScale = 0f;

        // Show countdown UI and text
        if (countdownUI != null) countdownUI.SetActive(true);
        if (countdownText != null) countdownText.gameObject.SetActive(true);

        float remainingCountdown = countdownDuration;

        while (remainingCountdown > 0)
        {
            if (countdownText != null)
                countdownText.text = Mathf.Ceil(remainingCountdown).ToString();

            yield return new WaitForSecondsRealtime(1f);
            remainingCountdown--;
        }

        // Hide countdown UI and text
        if (countdownUI != null) countdownUI.SetActive(false);
        if (countdownText != null) countdownText.gameObject.SetActive(false);

        // Resume time
        Time.timeScale = 1f;

        // Show extra texts at start of scene
        if (extraTexts != null)
        {
            foreach (TMP_Text t in extraTexts)
            {
                if (t != null) t.gameObject.SetActive(true);
            }
        }

        // Scene timer
        float remainingSceneTime = sceneDuration;

        while (remainingSceneTime > 0)
        {
            if (sceneTimerText != null)
                sceneTimerText.text = Mathf.Ceil(remainingSceneTime).ToString();

            yield return null;
            remainingSceneTime -= Time.deltaTime;
        }

        // Hide extra texts at end of scene
        if (extraTexts != null)
        {
            foreach (TMP_Text t in extraTexts)
            {
                if (t != null) t.gameObject.SetActive(false);
            }
        }

        // Switch scene if needed
        if (switchSceneAfterTimer && !string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
