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
        // Volumen
        float savedVolume = PlayerPrefs.GetFloat("volumen", 0.75f);
        volumenSlider.value = savedVolume;
        SetVolume(savedVolume);

        // Calidad
        calidadDropdown.ClearOptions();
        List<string> options = new List<string>(QualitySettings.names);
        calidadDropdown.AddOptions(options);
        calidadDropdown.value = QualitySettings.GetQualityLevel();
        calidadDropdown.RefreshShownValue();

        // Pantalla Completa
        pantallaCompletaToggle.isOn = Screen.fullScreen;

        // Resoluciones
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
        resolucionDropdown.value = currentResolutionIndex;
        resolucionDropdown.RefreshShownValue();
    }

    public void OnVolumeChange(float volume)
    {
        SetVolume(volume);
        PlayerPrefs.SetFloat("volumen", volume);
    }

    private void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1)) * 20);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution res = resoluciones[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
}
