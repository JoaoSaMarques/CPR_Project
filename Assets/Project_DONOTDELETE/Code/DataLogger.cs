using System.IO;
using UnityEngine;

[System.Serializable]
public class GameStats
{
    public int depthGreenHits;
    public int depthRedHits;
    public float bpmGreenTime;
    public float bpmRedTime;
}

public class DataLogger : MonoBehaviour
{
    [Header("References")]
    public DepthBarController depthBar;
    public RadialProgressBPM bpmCounter;

    [Header("Settings")]
    public bool saveOnSceneEnd = true; // automatically save when scene ends
    public bool saveCSV = true;        // save a CSV copy
    public string fileName = "GameStats";

    private void OnDestroy()
    {
        if (saveOnSceneEnd)
        {
            SaveStats();
        }
    }

    public void SaveStats()
    {
        if (depthBar == null || bpmCounter == null)
        {
            Debug.LogWarning("DataLogger: Missing references to DepthBarController or RadialProgressBPM.");
            return;
        }

        GameStats stats = new GameStats
        {
            depthGreenHits = depthBar.greenHits,
            depthRedHits = depthBar.redHits,
            bpmGreenTime = bpmCounter.GreenTime,
            bpmRedTime = bpmCounter.RedTime
        };

        // --- Save JSON ---
        string json = JsonUtility.ToJson(stats, true);
        string jsonPath = Path.Combine(Application.persistentDataPath, fileName + ".json");
        File.WriteAllText(jsonPath, json);

        // --- Optional CSV ---
        if (saveCSV)
        {
            string csv = $"DepthGreen,DepthRed,BPMGreen,BPMRed\n" +
                         $"{stats.depthGreenHits},{stats.depthRedHits},{stats.bpmGreenTime:F2},{stats.bpmRedTime:F2}";
            string csvPath = Path.Combine(Application.persistentDataPath, fileName + ".csv");
            File.WriteAllText(csvPath, csv);

            Debug.Log($"Stats saved:\nJSON: {jsonPath}\nCSV: {csvPath}");
        }
        else
        {
            Debug.Log($"Stats saved:\nJSON: {jsonPath}");
        }
    }
}
