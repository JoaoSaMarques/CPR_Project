using UnityEngine;
using UnityEngine.UI;

public class TelemetryHUD : MonoBehaviour
{
    [SerializeField] private Text heartRateText; // UI Text element
    [SerializeField] private LineRenderer heartbeatGraph; // optional line graph
    [SerializeField] private float bpm = 72f; // beats per minute

    private float time;

    void Update()
    {
        // Simulate heartbeat as a sine wave
        time += Time.deltaTime;
        float freq = bpm / 60f;
        float heartbeat = Mathf.Abs(Mathf.Sin(time * freq * Mathf.PI)); // 0–1 oscillation

        // Update text
        heartRateText.text = $"Heart Rate: {bpm:F0} bpm";

        // Optional: visualize wave
        if (heartbeatGraph != null)
        {
            int points = 100;
            heartbeatGraph.positionCount = points;
            for (int i = 0; i < points; i++)
            {
                float x = i / (float)points;
                float y = Mathf.Abs(Mathf.Sin((time + x) * freq * Mathf.PI));
                heartbeatGraph.SetPosition(i, new Vector3(x, y, 0));
            }
        }
    }
}
