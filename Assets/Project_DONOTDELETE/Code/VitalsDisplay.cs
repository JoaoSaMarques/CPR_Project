using UnityEngine;
using TMPro;

public class VitalsDisplay : MonoBehaviour
{
    public TextMeshProUGUI heartRateText;
    public TextMeshProUGUI spo2Text;
    public TextMeshProUGUI respText;
    public TextMeshProUGUI tempText;

    void Start()
    {
        // Example: Initial values
        UpdateVitals(69, 97, 19, 36.0f);
    }

    public void UpdateVitals(int heartRate, int spo2, int respRate, float temp)
    {
        heartRateText.text = $"HR: {heartRate} BPM";
        spo2Text.text = $"SPO₂: {spo2}%";
        respText.text = $"Resp: {respRate} BrPM";
        tempText.text = $"Temp: {temp:0.0}°C";
    }

    // Optional: Simulate dynamic change
    void Update()
    {
        float time = Time.time;
        int bpm = 60 + Mathf.RoundToInt(Mathf.Sin(time) * 10);
        UpdateVitals(bpm, 97, 19, 36.3f);
    }
}
