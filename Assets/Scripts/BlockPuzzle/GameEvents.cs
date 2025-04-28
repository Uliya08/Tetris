using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static Action<bool> GameOver;
    public static Action<int> AddScores; 
    public static Action CheckIfShapeCanBePlaced;
    public static Action MoveShapeToStartPosition;
    public static Action RequestNewShapes;
    public static Action SetShapeInactive;
    public static Action<int, int> UpdateBestScoreBar;
    public static Action<Config.SquareColor> UpdateSquareColor;
    public static Action ShowCongratulationWritings;
    public static Action<Config.SquareColor> ShowBonusScreen;

    // Для тетриса
    public static event System.Action<int, int> UpdateTetrisBestScore;
    // Методы для безопасного вызова событий
    public static void CallUpdateBestScoreBar(int currentScore, int bestScore)
    {
        UpdateBestScoreBar?.Invoke(currentScore, bestScore);
    }

    public static void CallUpdateTetrisBestScore(int currentScore, int bestScore)
    {
        UpdateTetrisBestScore?.Invoke(currentScore, bestScore);
    }
}
