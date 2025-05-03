using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    private void SetVolume(float volume)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetVolume(volume);
        }
    }
}
