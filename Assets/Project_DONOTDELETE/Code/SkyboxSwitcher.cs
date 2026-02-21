using UnityEngine;
using UnityEngine.InputSystem;

public class SkySphereSwitcher : MonoBehaviour
{
    [Header("Sky Spheres")]
    public GameObject[] skySpheres; // assign 3 spheres in the inspector
    private int currentIndex = 0;

    void Start()
    {
        UpdateSpheres();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.yKey.wasPressedThisFrame)
        {
            NextSphere();
        }
    }

    void NextSphere()
    {
        currentIndex = (currentIndex + 1) % skySpheres.Length;
        UpdateSpheres();
    }

    void UpdateSpheres()
    {
        for (int i = 0; i < skySpheres.Length; i++)
        {
            if (skySpheres[i] != null)
                skySpheres[i].SetActive(i == currentIndex); // enable only the active one
        }
    }
}
