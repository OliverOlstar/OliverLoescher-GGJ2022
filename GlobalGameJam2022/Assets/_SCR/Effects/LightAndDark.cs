using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAndDark : MonoBehaviour
{
    public enum Mode
    {
        Light, Dark
    }

    [SerializeField] private Material lightSkybox = null;
    [SerializeField] private GameObject[] lightObjects = new GameObject[0];
    [SerializeField] private Material darkSkybox = null;
    [SerializeField] private GameObject[] darkObjects = new GameObject[0];
    [HideInInspector] public Mode mode;

    private void Start() 
    {
        mode = Mode.Dark;
        Switch(Mode.Light);
    }

    [SerializeField] private Mode dark;
    // private void Update() 
    // {
    //     Switch(dark);
    // }
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying == false)
            Switch(dark);
    }

    public void SwitchLight() => Switch(Mode.Light);
    public void SwitchDark() => Switch(Mode.Dark);
    public void Switch(Mode pMode)
    {
        if (pMode == mode)
            return;

        // Exit
        switch (mode)
        {
            case Mode.Light:
                foreach (GameObject o in lightObjects)
                    o.SetActive(false);
                break;

            case Mode.Dark:
                foreach (GameObject o in darkObjects)
                    o.SetActive(false);
                break;
        }

        mode = pMode;

        // Enter
        switch (mode)
        {
            case Mode.Light:
                foreach (GameObject o in lightObjects)
                    o.SetActive(true);
                RenderSettings.skybox = lightSkybox;
                RenderSettings.ambientIntensity = 1.0f;
                break;

            case Mode.Dark:
                foreach (GameObject o in darkObjects)
                    o.SetActive(true);
                RenderSettings.skybox = darkSkybox;
                RenderSettings.ambientIntensity = 0.0f;
                break;
        }
    }
}
