using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
public class Board : MonoBehaviour
{   
    public Tilemap tilemap { get; private set; } 
    public Piece activePiece { get; private set; } 
    private Piece currentPiece; 
    public TetrominoData[] tetrominoes;  
    public Vector3Int spawnPosition;  
    public Vector2Int boardSize = new Vector2Int(10, 20); 
    private int score = 0;  
    public Text LinesClearedText = null;  
    private int linesCleared = 0; 
    private int level = 1; 
    public Text LevelText = null;  
    private const int linesToLevelUp = 10;   
    public GameOverUIManager gameOverUIManager;  
    private bool isGameOver = false;
    private const string TETRIS_RECORD_FILE = "tetris_best_score";
    public int Level                                       
    {
        get { return level; }
        set
        {
            level = value;
            UpdatePieceSpeed(); 
        }
    }

    public AudioSource gameOverAudio;  
    public AudioClip lineClearSound;   
    private AudioSource audioSource;   
    public AudioClip levelUpSound;     
    public RectInt Bounds 
     {
          get 
          {
               Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this. boardSize.y / 2);
               return new RectInt(position, this.boardSize);
          }
     }
    private void Awake() 
     {
          this.tilemap = GetComponentInChildren<Tilemap>();
          this.activePiece = GetComponentInChildren<Piece>();

          for (int i = 0; i < this.tetrominoes.Length; i++)
          {
               this.tetrominoes[i].Initialize();
          }
          LoadBestScore();
    }
    private void LoadBestScore()
    {
        if (BinaryDataStream.Exist(TETRIS_RECORD_FILE))
        {
            int bestScore = BinaryDataStream.Read<int>(TETRIS_RECORD_FILE);
            GameEvents.CallUpdateTetrisBestScore(0, bestScore);
        }
    }

    private void SaveBestScore()
    {
        BinaryDataStream.Save(score, TETRIS_RECORD_FILE);
        GameEvents.CallUpdateTetrisBestScore(score, score);
    }

    private void Start() 
     {
          SpawnPiece();
          currentPiece = activePiece;
          audioSource = gameObject.AddComponent<AudioSource>();
     }
    public void SpawnPiece()
    {
        int random = Random.Range(0, this.tetrominoes.Length);
        TetrominoData data = this.tetrominoes[random];

        // Проверяем валидность позиции перед инициализацией
        if (!IsValidSpawnPosition(this.spawnPosition))
        {
            GameOver();
            return; // Не создаем новую фигуру при GameOver
        }

        this.activePiece.Initialize(this, this.spawnPosition, data);
    }
    private bool IsValidSpawnPosition(Vector3Int spawnPos)
    {
        // Временно создаем фигуру для проверки
        GameObject tempPiece = new GameObject("TempPiece");
        Piece testPiece = tempPiece.AddComponent<Piece>();
        testPiece.Initialize(this, spawnPos, this.tetrominoes[0]); // Используем любую фигуру для теста

        bool isValid = IsValidPosition(testPiece, spawnPos);
        Destroy(tempPiece);
        return isValid;
    }

    private void GameOver()
     {
        if (!isGameOver)
        {
            isGameOver = true;

            // 1. Деактивируем текущую фигуру
            if (activePiece != null)
            {
                activePiece.enabled = false;
                activePiece.gameObject.SetActive(false);
            }

            // 2. Очищаем призрачную фигуру
            Ghost ghost = GetComponentInChildren<Ghost>();
            if (ghost != null)
            {
                ghost.ClearGhost();
                ghost.enabled = false;
            }

            // 3. Останавливаем игру
            Time.timeScale = 0;
            gameOverUIManager.ShowGameOverUI(score, level);
            gameOverAudio.Play();
        }
    }
    public void PauseGame(bool pause)
    {
        Time.timeScale = pause ? 0 : 1;
        activePiece?.SetPausedState(pause); 
    }
    public void RestartGame()
    {
        // 1. Сбрасываем состояние
        isGameOver = false;
        Time.timeScale = 1;
        score = 0;
        level = 1;
        linesCleared = 0;

        // 2. Очищаем доску
        tilemap.ClearAllTiles();

        // 3. Активируем призрачную фигуру
        Ghost ghost = GetComponentInChildren<Ghost>();
        if (ghost != null)
        {
            ghost.enabled = true;
        }

        // 4. Создаем новую фигуру
        SpawnPiece();

        // 5. Обновляем UI
        LinesClearedText.text = "Score: 0";
        LevelText.text = "Level: 1";
}
    public void Set(Piece piece)
    {
        
        for (int i = 0; i < piece.cells.Length; i++)
        {
            
            Vector3Int tilePosition = piece.cells[i] + piece.position;

            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    public void Clear(Piece piece)
    {
        
        for (int i = 0; i < piece.cells.Length; i++)
        {
           
            Vector3Int tilePosition = piece.cells[i] + piece.position;

            this.tilemap.SetTile(tilePosition, null);
        }
    }
    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        
        RectInt bounds = this.Bounds;
        for (int i = 0; i < piece.cells.Length; i++)
        {
            
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false; 
            }

            if (this.tilemap.HasTile(tilePosition))
            {
                return false; 
            }
        }

        return true;
    }
    public void ClearLines()
    {
        RectInt bounds = this.Bounds;                
        int row = bounds.yMin;                       
        int totalLinesCleared = 0;                   

        while (row < bounds.yMax)                     
        {
            if (IsLineFull(row))                      
            {
                LineClear(row);                       
                totalLinesCleared++;                  
            }
            else
            {
                row++;                                 
            }
        }

        UpdateScore(totalLinesCleared);              
        UpdateLevel(totalLinesCleared);              
    }
    private bool IsLineFull(int row)
    {
        RectInt bounds = this.Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)         
        {
            Vector3Int position = new Vector3Int(col, row, 0);       

            if (!this.tilemap.HasTile(position))                       
            {
                return false;                                         
            }
        }

        return true;                                                 
    }
    private void LineClear(int row)
     {
          RectInt bounds = this.Bounds;        

          for(int col = bounds.xMin; col < bounds.xMax; col++)
          {
               Vector3Int position = new Vector3Int(col, row, 0);
               this.tilemap.SetTile(position, null);
          }
          audioSource.PlayOneShot(lineClearSound);
          while (row < bounds.yMax)
          {
               for (int col = bounds.xMin; col < bounds.xMax; col++)
               {
                    Vector3Int position = new Vector3Int(col, row + 1, 0);        
                    TileBase above = this.tilemap.GetTile(position);              

                    position = new Vector3Int(col, row, 0);                       
                    this.tilemap.SetTile(position, above);                         
               }

               row++;                                                            
          }
     }
     private void UpdateScore(int linesCleared)
     {
        if (linesCleared > 0)
        {
            score += linesCleared;

            LinesClearedText.text = "Score: " + score;

            if (BinaryDataStream.Exist(TETRIS_RECORD_FILE))
            {
                int bestScore = BinaryDataStream.Read<int>(TETRIS_RECORD_FILE);
                if (score > bestScore)
                {
                    BinaryDataStream.Save(score, TETRIS_RECORD_FILE);
                    GameEvents.CallUpdateTetrisBestScore(score, score);
                }
                else
                {
                    GameEvents.CallUpdateTetrisBestScore(score, bestScore);
                }
            }
            else
            {
                BinaryDataStream.Save(score, TETRIS_RECORD_FILE);
                GameEvents.CallUpdateTetrisBestScore(score, score);
            }
        }
        /*if (LinesClearedText != null){
               LinesClearedText.text = "Score: " + score.ToString();    
        }*/
          
     }
     private void UpdateLevel(int linesCleared)
     {
          if (linesCleared > 0)
          {
            this.linesCleared += linesCleared;          
               while (this.linesCleared >= linesToLevelUp)
               {
                    level++;                                
                    UpdatePieceSpeed();                       
                    this.linesCleared -= linesToLevelUp;      
                    audioSource.PlayOneShot(levelUpSound);
                   
               }
          }

          if (LevelText != null){
               LevelText.text = "Level: " + level.ToString();
          }
     }
    private void UpdatePieceSpeed()
    {
       
     currentPiece.Speed = 1 + (level - 1) * 0.5f; 
     currentPiece.UpdateStepDelay();
    }
}
 