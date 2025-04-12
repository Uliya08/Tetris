using System.Collections;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class BestScoreData
{
    public int score = 0;
}

public class Scores : MonoBehaviour
{
    public SquareTextureData squareTextureData;
    public Text scoreText;
    private bool newBestScore = false;
    private BestScoreData bestScores_ = new BestScoreData();

    private int currentScores_;

    private string bestScoreKey_ = "bsdat";

    private void Awake()
    {
        if (BinaryDataStream.Exist(bestScoreKey_))
        {
            StartCoroutine(ReadDataFile());
        }
    }

    private IEnumerator ReadDataFile()
    {
        bestScores_ = BinaryDataStream.Read<BestScoreData>(bestScoreKey_);
        yield return new WaitForEndOfFrame();
        GameEvents.UpdateBestScoreBar(currentScores_, bestScores_.score);
    }

    void Start()
    {
        currentScores_ = 0;
        newBestScore = false;
        squareTextureData.SetStartColor();
        UpdateScoreText();
    }

    private void OnEnable()
    {
        GameEvents.AddScores += AddScores;
        GameEvents.GameOver += SaveBestScores;
    }

    private void OnDisable()
    {
        GameEvents.AddScores -= AddScores;
        GameEvents.GameOver -= SaveBestScores;
    }

    public void SaveBestScores(bool newBestScores)
    {
        BinaryDataStream.Save<BestScoreData>(bestScores_, bestScoreKey_);
    }
    public bool IsNewBestScore()
    {
        return currentScores_ > bestScores_.score;
    }
    private void AddScores(int scores)
    {
        currentScores_ += scores;

        if (IsNewBestScore())
        {
            bestScores_.score = currentScores_;
            BinaryDataStream.Save<BestScoreData>(bestScores_, bestScoreKey_);
        }

        UpdateSquareColor();
        GameEvents.UpdateBestScoreBar(currentScores_, bestScores_.score);
        UpdateScoreText();
    }

    private void UpdateSquareColor()
    {
        if(GameEvents.UpdateSquareColor != null && currentScores_ >= squareTextureData.tresholdVal)
        {
            squareTextureData.UpdateColors(currentScores_);
            GameEvents.UpdateSquareColor(squareTextureData.currentColor);
        }
    }
    private void UpdateScoreText()
    {
        scoreText.text = currentScores_.ToString();
    }

    public int GetCurrentScore()
    {
        return currentScores_;
    }

    public int GetBestScore()
    {
        return bestScores_.score;
    }

}
