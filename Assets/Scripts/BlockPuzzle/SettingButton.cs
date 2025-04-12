using UnityEngine;
using UnityEngine.UI;

public class SettingButton : MonoBehaviour
{
    public Button openSettingsButton;
    public Button closeSettingsButton;

    public void SettingsOpened()
    {
        openSettingsButton.gameObject.SetActive(false);
        closeSettingsButton.gameObject.SetActive(true);
        closeSettingsButton.interactable = true;
    }

    public void SettingsClosed()
    {
        openSettingsButton.gameObject.SetActive(true);
        openSettingsButton.interactable = true;

        closeSettingsButton.gameObject.SetActive(false);
    }
}
