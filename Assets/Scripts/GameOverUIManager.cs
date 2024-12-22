using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*! \brief Класс предназначен для управления интерфейсом завершения игры.
 * 
 * Во время проигрыша появляется окно GameOver, на котором игрок видит свой конечный счёт и уровень.
 * Кроме того, есть кнопка перезапуска игры и выхода в главное меню.
 * 
 */
public class GameOverUIManager : MonoBehaviour
{
    [SerializeField] private GameObject canvasGameOver; //!< Canvas для отображения интерфейса завершения игры.
    [SerializeField] private Text scoreText;            //!< Текст для отображения счета игрока.
    [SerializeField] private Text levelText;            //!< Текст для отображения текущего уровня игры.
    [SerializeField] private Button restart;            //!< Кнопка для перезапуска игры.
    [SerializeField] private Button backMenu;           //!< Кнопка для возврата на главное меню.
    public Board board;                                 //!< Ссылка на объект Board для управления игровой логикой.


    /*! \brief Метод инициализирует слушатели для кнопок при активации объекта.
     * 
     * C помощью AddListener добавляется слушатель к событию onClick кнопки, чтобы 
     * при нажатии на неё вызывалась определённая функция (RestartGame и BackToMenu).
     */
    private void Awake() 
    {
        if (restart != null)
            restart.onClick.AddListener(RestartGame);
        
        if (backMenu != null)
            backMenu.onClick.AddListener(BackToMenu);
    }

    /*! \brief Метод удаляет слушатели для кнопок при уничтожении объекта.
     */
    private void OnDestroy() 
    {
        if (restart != null)
            restart.onClick.RemoveListener(RestartGame);
        
        if (backMenu != null)
            backMenu.onClick.RemoveListener(BackToMenu);
    }

    /*!
     * \brief Метод позволяет отображать интерфейс завершения игры.
     * 
     * Метод необходим для информирования игрока о его конечном счёте и уровне при проигрыше.
     * 
     * \param score Текущий счет игрока.
     * \param level Текущий уровень игры.
     */
    public void ShowGameOverUI(int score, int level)
    {
        canvasGameOver.SetActive(true);
        UpdateScoreText(score);
        UpdateLevelText(level);
    }

    /*! \brief Метод для вывода на экране GameOver конечного счёта.
     * 
     * \param score Конечный счёт игрока.
     */
    private void UpdateScoreText(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}"; 
    }

    /*! \brief Метод для вывода на экране GameOver конечного уровня.
     * 
     * \param level Конечный уровень игрока.
     */
    private void UpdateLevelText(int level)
    {
        if (levelText != null)
            levelText.text = $"Level: {level}"; 
    }

    /*! \brief Метод перезапускает игру, загружая текущую сцену и перезагружая игровое поле.
     * 
     */
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        board.RestartGame();
    }

    /*! \brief  Метод возвращает игрока в главное меню.
     */
    private void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
