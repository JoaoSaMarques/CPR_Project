using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class DepthBarController : MonoBehaviour
{
    [Header("UI References")]
    public Image yellowBar;
    public RectTransform trianglePivot;
    public TextMeshProUGUI depthText;

    [Header("Depth Settings")]
    public float fillMinGood = 0.5f;
    public float fillMaxGood = 0.6f;
    public float fillSpeed = 0.5f;
    public Key actionKey = Key.Z;

    [Header("Triangle Settings")]
    public float triangleStartY = 50f;
    public float triangleEndY = -20f;

    [Header("Color Settings")]
    public Color defaultColor = Color.red;
    public Color successColor = Color.green;
    public float flashDuration = 0.25f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip greenHitClip;

    private bool buttonHeld;
    private bool prevButtonHeld;
    private bool mustReset;         
    private Coroutine flashRoutine;

    public int greenHits { get; private set; }
    public int redHits { get; private set; }

    void Start()
    {
        if (yellowBar != null)
            yellowBar.color = defaultColor;
    }

    void Update()
    {
        buttonHeld = Keyboard.current != null &&
                     Keyboard.current[actionKey].isPressed;

        // RELEASE CHECK FIRST
        if (prevButtonHeld && !buttonHeld)
        {
            float currentFill = yellowBar.fillAmount;

            // Only allow green if we are NOT waiting for reset
            if (!mustReset &&
                currentFill >= fillMinGood &&
                currentFill <= fillMaxGood)
            {
                if (flashRoutine != null)
                    StopCoroutine(flashRoutine);

                flashRoutine = StartCoroutine(FlashBarColor());

                if (audioSource != null && greenHitClip != null)
                    audioSource.PlayOneShot(greenHitClip);

                greenHits++;

                mustReset = true;   // Lock until drained
            }
            else
            {
                redHits++;
            }
        }

        // APPLY FILL / DRAIN
        if (buttonHeld)
            yellowBar.fillAmount += fillSpeed * Time.deltaTime;
        else
            yellowBar.fillAmount -= fillSpeed * Time.deltaTime;

        yellowBar.fillAmount = Mathf.Clamp01(yellowBar.fillAmount);

        // Unlock when fully drained
        if (mustReset && yellowBar.fillAmount <= 0f)
        {
            mustReset = false;
        }

        // Triangle follows fill
        if (trianglePivot != null)
        {
            float targetY = Mathf.Lerp(triangleStartY, triangleEndY, yellowBar.fillAmount);
            trianglePivot.localPosition = new Vector3(
                trianglePivot.localPosition.x,
                targetY,
                trianglePivot.localPosition.z
            );
        }

        // Depth text
        if (depthText != null)
            depthText.text = MapFillToDepth(yellowBar.fillAmount).ToString();

        prevButtonHeld = buttonHeld;
    }

    IEnumerator FlashBarColor()
    {
        yellowBar.color = successColor;

        yield return new WaitForSeconds(flashDuration);

        yellowBar.color = defaultColor;
    }

    int MapFillToDepth(float fill)
    {
        if (fill <= 0.5f) return 0;
        if (fill <= 0.6f) return 5;
        if (fill < 1f) return 6;
        return 1;
    }
}