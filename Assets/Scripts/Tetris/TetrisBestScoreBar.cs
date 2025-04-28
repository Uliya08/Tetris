using UnityEngine;
using UnityEngine.UI;

public class TetrisBestScoreBar : MonoBehaviour
{
    public Image fillInImage;        
    public Text bestScoreText;            

    private void OnEnable()
    { 
        GameEvents.UpdateTetrisBestScore += UpdateBestScoreBar;
    }

    private void OnDisable()
    {
        GameEvents.UpdateTetrisBestScore -= UpdateBestScoreBar;
    }

    private void UpdateBestScoreBar(int currentScore, int bestScore)
    {
        float currentPercentage = (float)currentScore / Mathf.Max(1, bestScore);
        fillInImage.fillAmount = currentPercentage;
        bestScoreText.text = bestScore.ToString();

    }
}