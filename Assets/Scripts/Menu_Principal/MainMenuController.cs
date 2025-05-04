using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject settingsUI;

    void Start()
    {
        mainMenuUI.SetActive(true);
        settingsUI.SetActive(false);
    }

    public void PlayFreeMode()
    {
        SceneManager.LoadScene("ModoLibre");
    }

    public void PlayCampaignMode()
    {
        SceneManager.LoadScene("ModoCampaña");
    }

    public void OpenSettings()
    {
        mainMenuUI.SetActive(false);
        settingsUI.SetActive(true);
    }

    public void BackToMainMenu()
    {
        settingsUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
