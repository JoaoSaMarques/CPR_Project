using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RadialProgressBPM : MonoBehaviour
{
    [Header("Counter Settings")]
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private TextMeshProUGUI superCounterText;
    [SerializeField] private Color normalColor = Color.red;
    [SerializeField] private Color greenColor = Color.green;
    [SerializeField] private int maxCounterValue = 120;

    [SerializeField] private int hiddenCounter = 0;
    private float superCounter = 0f;

    [Header("Progress Settings")]
    [SerializeField] private Image greenRing;
    [SerializeField] private Transform rotatingObject;
    [SerializeField] private float rotationAngle = 360f;
    [SerializeField] private float decreaseSpeed = 0.1f;

    [Header("Red Blink & Fade Settings")]
    [SerializeField] private CanvasGroup redCanvasGroup;
    [SerializeField] private CanvasGroup insideFillCircleRed;
    [SerializeField] private GameObject dashedCircleRed;
    [SerializeField] private GameObject dashedCircleRed2;
    [SerializeField] private float blinkSpeed = 2f;
    [SerializeField] private float fadeSpeed = 5f;

    [Header("Blue Settings")]
    [SerializeField] private GameObject dashedCircleBlue;
    [SerializeField] private float blueRotationSpeed = 100f;

    [Header("Input Settings")]
    [SerializeField] private Key increaseKey = Key.Z;

    [Header("SuperCounter Reset")]
    [SerializeField] private float superResetDelay = 2f;
    [SerializeField] private float superResetSpeed = 200f;

    [Header("Audio")]
    [SerializeField] private AudioSource greenLoopSource;
    [SerializeField] private AudioClip greenLoopClip;

    private bool isInGreenZone = false;

    private float lastTapTime = -1f;
    private float bpm = 0f;
    [SerializeField] private float minInterval = 0.15f;
    [SerializeField] private float maxInterval = 1.2f;

    private float progress = 0f;
    private float rotationAngleAccum = 0f;
    private float timeSinceLastTap = 0f;

    // --- New tracking fields ---
    private float greenTime = 0f;
    private float redTime = 0f;

    public float GreenTime => greenTime;
    public float RedTime => redTime;

    void Update()
    {
        float deltaTime = Time.deltaTime;
        timeSinceLastTap += deltaTime;

        // Input + BPM
        if (Keyboard.current != null && Keyboard.current[increaseKey].wasPressedThisFrame)
        {
            float now = Time.time;

            if (lastTapTime < 0f)
            {
                lastTapTime = now;
                return;
            }

            float interval = now - lastTapTime;
            lastTapTime = now;

            if (interval < minInterval || interval > maxInterval)
                return;

            bpm = 60f / interval;

            progress = Mathf.Lerp(progress, Mathf.Clamp(bpm, 0f, 120f) / 120f, 0.5f);
            superCounter = Mathf.Lerp(superCounter, bpm, 0.5f);
            hiddenCounter = Mathf.RoundToInt(superCounter);
            timeSinceLastTap = 0f;
        }
        else if (timeSinceLastTap >= superResetDelay)
        {
            superCounter = Mathf.Max(superCounter - superResetSpeed * deltaTime, 0f);
        }

        // Natural decay
        progress = Mathf.Clamp01(progress - decreaseSpeed * deltaTime);

        // Effective progress
        float effectiveProgress = (superCounter > 120f) ? 0f : progress;

        if (greenRing != null)
            greenRing.fillAmount = effectiveProgress;

        // Rotation
        if (rotatingObject != null)
        {
            float angle = Mathf.Lerp(0f, rotationAngle, effectiveProgress);
            rotatingObject.localRotation = Quaternion.Euler(0f, 0f, -angle);
        }

        // Inside red fade
        if (insideFillCircleRed != null)
        {
            float targetAlpha = (effectiveProgress >= 0.75f) ? 0f : 1f;
            insideFillCircleRed.alpha = Mathf.MoveTowards(
                insideFillCircleRed.alpha,
                targetAlpha,
                fadeSpeed * deltaTime
            );
        }

        // Blue / red visuals
        if (effectiveProgress >= 0.75f)
        {
            if (dashedCircleBlue != null)
            {
                dashedCircleBlue.SetActive(true);
                rotationAngleAccum -= blueRotationSpeed * deltaTime;
                dashedCircleBlue.transform.localRotation =
                    Quaternion.Euler(0f, 0f, rotationAngleAccum);
            }

            if (dashedCircleRed != null) dashedCircleRed.SetActive(false);
            if (dashedCircleRed2 != null) dashedCircleRed2.SetActive(false);
        }
        else
        {
            if (dashedCircleBlue != null) dashedCircleBlue.SetActive(false);
            if (dashedCircleRed != null) dashedCircleRed.SetActive(true);
            if (dashedCircleRed2 != null) dashedCircleRed2.SetActive(true);
        }

        // 🔁 Green zone audio loop
        bool currentlyGreen = effectiveProgress >= 0.75f;
        bool currentlyRed = superCounter > 121f || bpm < 100f;

        // --- Track time ---
        if (currentlyGreen) greenTime += deltaTime;
        if (currentlyRed) redTime += deltaTime;

        if (currentlyGreen && !isInGreenZone)
        {
            if (greenLoopSource != null && greenLoopClip != null)
            {
                greenLoopSource.clip = greenLoopClip;
                greenLoopSource.Play();
            }
        }
        else if (!currentlyGreen && isInGreenZone)
        {
            if (greenLoopSource != null)
                greenLoopSource.Stop();
        }

        isInGreenZone = currentlyGreen;

        // Red blink
        bool showRed = (superCounter > 121f) || (bpm < 100f);
        if (redCanvasGroup != null)
            redCanvasGroup.alpha = showRed
                ? Mathf.PingPong(Time.time * blinkSpeed, 1f)
                : 0f;

        // Counter display
        if (superCounter > 121f)
        {
            if (counterText != null) counterText.gameObject.SetActive(false);
            if (superCounterText != null)
            {
                superCounterText.gameObject.SetActive(true);
                superCounterText.text = Mathf.RoundToInt(superCounter).ToString();
                superCounterText.color = normalColor;
            }
        }
        else
        {
            if (counterText != null)
            {
                counterText.gameObject.SetActive(true);
                int visibleCounter =
                    (progress <= 0.75f)
                        ? Mathf.RoundToInt((progress / 0.75f) * 100f)
                        : 100 + Mathf.RoundToInt((progress - 0.75f) / 0.25f * 20f);

                visibleCounter = Mathf.Clamp(visibleCounter, 0, maxCounterValue);
                counterText.text = visibleCounter.ToString();
                counterText.color = (visibleCounter >= 100) ? greenColor : normalColor;
            }

            if (superCounterText != null)
                superCounterText.gameObject.SetActive(false);
        }
    }
}
