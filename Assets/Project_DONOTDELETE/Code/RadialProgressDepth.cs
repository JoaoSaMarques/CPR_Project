using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RadialProgressDepth : MonoBehaviour
{
    // ===============================
    // DEPTH DISPLAY
    // ===============================
    [Header("Depth Counter Settings")]
    [SerializeField] private TextMeshProUGUI depthText;
    [SerializeField] private Color normalColor = Color.red;
    [SerializeField] private Color goodColor = Color.green;

    [Header("Depth Values (cm)")]
    [SerializeField] private float minDepth = 4f; // maps to 0.75
    [SerializeField] private float maxDepth = 6f; // maps to 1.0

    // ===============================
    // PROGRESS / VISUALS (SAME AS BPM)
    // ===============================
    [Header("Progress Settings")]
    [SerializeField] private Image greenRing;
    [SerializeField] private Transform rotatingObject;
    [SerializeField] private float rotationAngle = 360f;
    [SerializeField] private float releaseSpeed = 25f;

    private float progress = 0f;
    private float rotationAngleAccum = 0f;

    // ===============================
    // RED / BLUE FX
    // ===============================
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

    // ===============================
    // INPUT
    // ===============================
    [Header("Input Settings")]
    [SerializeField] private Key depthKey = Key.Z;
    [SerializeField] private float depthIncreaseSpeed = 15f;

    private float currentDepth = 0f;

    void Update()
    {
        // ===============================
        // DEPTH INPUT (IMMEDIATE RESPONSE)
        // ===============================
        if (Keyboard.current != null && Keyboard.current[depthKey].isPressed)
        {
            currentDepth += depthIncreaseSpeed * Time.deltaTime;
        }
        else
        {
            currentDepth -= releaseSpeed * Time.deltaTime;
        }

        currentDepth = Mathf.Clamp(currentDepth, 0f, maxDepth);

        // ===============================
        // DEPTH to PROGRESS MAPPING
        // 0–4   to 0–0.75
        // 4–6   to 0.75–1
        // ===============================
        if (currentDepth <= minDepth)
        {
            progress = Mathf.Lerp(0f, 0.75f, currentDepth / minDepth);
        }
        else
        {
            float t = (currentDepth - minDepth) / (maxDepth - minDepth);
            progress = Mathf.Lerp(0.75f, 1f, t);
        }

        progress = Mathf.Clamp01(progress);

        // ===============================
        // UPDATE GREEN RING
        // ===============================
        if (greenRing != null)
            greenRing.fillAmount = progress;

        // ===============================
        // ROTATE POINTER
        // ===============================
        if (rotatingObject != null)
        {
            float angle = Mathf.Lerp(0f, rotationAngle, progress);
            rotatingObject.localRotation = Quaternion.Euler(0f, 0f, -angle);
        }

        // ===============================
        // RED / BLUE STATE SWITCH
        // ===============================
        if (progress >= 0.75f)
        {
            dashedCircleBlue.SetActive(true);
            dashedCircleRed.SetActive(false);
            dashedCircleRed2.SetActive(false);

            rotationAngleAccum -= blueRotationSpeed * Time.deltaTime;
            dashedCircleBlue.transform.localRotation =
                Quaternion.Euler(0f, 0f, rotationAngleAccum);
        }
        else
        {
            dashedCircleBlue.SetActive(false);
            dashedCircleRed.SetActive(true);
            dashedCircleRed2.SetActive(true);

            if (redCanvasGroup != null)
                redCanvasGroup.alpha =
                    Mathf.PingPong(Time.time * blinkSpeed, 1f);
        }

        if (insideFillCircleRed != null)
        {
            float targetAlpha = (progress >= 0.75f) ? 0f : 1f;
            insideFillCircleRed.alpha =
                Mathf.MoveTowards(
                    insideFillCircleRed.alpha,
                    targetAlpha,
                    fadeSpeed * Time.deltaTime
                );
        }

        // ===============================
        // DEPTH TEXT (0–6)
        // ===============================
        float displayedDepth = Mathf.Round(currentDepth * 10f) / 10f;
        depthText.text = displayedDepth.ToString("0");

        depthText.color =
            (currentDepth >= minDepth && currentDepth <= maxDepth)
            ? goodColor
            : normalColor;
    }
}
