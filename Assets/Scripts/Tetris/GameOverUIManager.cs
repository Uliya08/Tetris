using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameOverUIManager : MonoBehaviour
{
    [SerializeField] private GameObject canvasGameOver; 
    [SerializeField] private Text scoreText;            
    [SerializeField] private Text levelText;            
    [SerializeField] private Button restart;            
    [SerializeField] private Button backMenu;           
    public Board board;                                
    private void Awake() 
    {
        if (restart != null)
            restart.onClick.AddListener(RestartGame);
        
        if (backMenu != null)
            backMenu.onClick.AddListener(BackToMenu);
    }
    private void OnDestroy() 
    {
        if (restart != null)
            restart.onClick.RemoveListener(RestartGame);
        
        if (backMenu != null)
            backMenu.onClick.RemoveListener(BackToMenu);
    }
    public void ShowGameOverUI(int score, int level)
    {
        canvasGameOver.SetActive(true);
        UpdateScoreText(score);
        UpdateLevelText(level);
    }
    private void UpdateScoreText(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}"; 
    }
    private void UpdateLevelText(int level)
    {
        if (levelText != null)
            levelText.text = $"Level: {level}"; 
    }
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        board.RestartGame();
    }
    private void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
