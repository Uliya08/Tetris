using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    public GameObject exit; 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowExitConfirmation();
        }
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void ShowExitConfirmation()
    {
        exit.SetActive(true); 
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void OnYesButtonClicked()
    {
        ExitGame(); 
    }
    public void OnNoButtonClicked()
    {
        exit.SetActive(false); 
    }
}
