using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    public Slider volumenSlider;
    public Dropdown calidadDropdown;
    public Toggle pantallaCompletaToggle;
    public Dropdown resolucionDropdown;
    public AudioMixer audioMixer;

    private Resolution[] resoluciones;

    void Start()
    {
        // --- Volumen ---
        float savedVolume = PlayerPrefs.GetFloat("volumen", 0.75f);
        volumenSlider.value = savedVolume;
        SetVolume(savedVolume);
        volumenSlider.onValueChanged.AddListener(OnVolumeChange);

        // --- Calidad ---
        calidadDropdown.ClearOptions();
        List<string> options = new List<string>(QualitySettings.names);
        calidadDropdown.AddOptions(options);
        int savedQuality = PlayerPrefs.GetInt("calidad", QualitySettings.GetQualityLevel());
        calidadDropdown.value = savedQuality;
        calidadDropdown.RefreshShownValue();
        SetQuality(savedQuality);
        calidadDropdown.onValueChanged.AddListener(SetQuality);

        // --- Pantalla completa ---
        bool isFullscreen = PlayerPrefs.GetInt("fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        pantallaCompletaToggle.isOn = isFullscreen;
        SetFullscreen(isFullscreen);
        pantallaCompletaToggle.onValueChanged.AddListener(SetFullscreen);

        // --- Resolución ---
        resoluciones = Screen.resolutions;
        resolucionDropdown.ClearOptions();
        List<string> resolucionOptions = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resoluciones.Length; i++)
        {
            string option = resoluciones[i].width + " x " + resoluciones[i].height;
            resolucionOptions.Add(option);

            if (resoluciones[i].width == Screen.currentResolution.width &&
                resoluciones[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolucionDropdown.AddOptions(resolucionOptions);
        int savedResolutionIndex = PlayerPrefs.GetInt("resolutionIndex", currentResolutionIndex);
        resolucionDropdown.value = savedResolutionIndex;
        resolucionDropdown.RefreshShownValue();
        SetResolution(savedResolutionIndex);
        resolucionDropdown.onValueChanged.AddListener(SetResolution);
    }

    // Volumen
    public void OnVolumeChange(float volume)
    {
        SetVolume(volume);
        PlayerPrefs.SetFloat("volumen", volume);
    }

    private void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1)) * 20);
    }

    // Calidad
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("calidad", qualityIndex);
    }

    // Pantalla completa
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
    }

    // Resolución
    public void SetResolution(int resolutionIndex)
    {
        Resolution res = resoluciones[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
    }
}
