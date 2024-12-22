using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*! \brief  Основной класс меню, управляет навигацией и подтверждением выхода из игры.
 */
public class MainMenu : MonoBehaviour
{
    public GameObject exit; //!< Ссылка на панель подтверждения выхода

    private void Update()
    {
        // Проверяем, нажата ли клавиша Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowExitConfirmation();
        }
    }

    /*! \brief Метод для начала новой игры.
     */
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /*! \brief Метод показывает панель подтверждения выхода.
     */
    public void ShowExitConfirmation()
    {
        exit.SetActive(true); 
    }

    /*! \brief Метод закрытия приложения.
     */
    public void ExitGame()
    {
        Application.Quit();
    }

    /*! \brief Обработчик нажатия кнопки "Да" в панели подтверждения.
     * 
     * Выходит из игры.
     */
    public void OnYesButtonClicked()
    {
        ExitGame(); 
    }

    /*! \brief Обработчик нажатия кнопки "Нет" в панели подтверждения.
     * 
     * Скрывает панель подтверждения.
     */
    public void OnNoButtonClicked()
    {
        exit.SetActive(false); 
    }
}
