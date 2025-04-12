using UnityEngine;
using UnityEngine.UI;

public class GameOverPopup : MonoBehaviour
{
    public GameObject gameOverPopup;
    public Text currentScoreText;
    public Text bestScoreText;
    public GameObject gameOverSprite;
    public GameObject newBestScoreSprite;

    private void Start()
    {
        gameOverPopup.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameEvents.GameOver -= OnGameOver;
    }

    private void OnGameOver(bool isNewBestScore)
    {
        Scores scores = FindFirstObjectByType<Scores>();
        if (scores == null) return;

        currentScoreText.text = scores.GetCurrentScore().ToString();
        bestScoreText.text = scores.GetBestScore().ToString();

        gameOverPopup.SetActive(true);
        gameOverSprite.SetActive(true);
        newBestScoreSprite.SetActive(scores.GetCurrentScore() == scores.GetBestScore());
    }
}