using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/*! \brief Класс Board отвечает за управление игровым полем и логикой игры.
 * 
 *
 * Он осуществляет появление фигур на игровом поле и устанавливает их, определяет границы игрового поля, позволяет вести счёт очищенных линий,
 * повышает уровень игры и вместе с этим скорость падения фигур, обрабатывает логику окончания игры.
 *
 */
public class Board : MonoBehaviour
{   
    public Tilemap tilemap { get; private set; } //!< Ссылка на Tilemap, который используется для отображения блоков на игровом поле.
    public Piece activePiece { get; private set; } //!< Текущая активная фигура (тетромино).
    private Piece currentPiece; //!< Текущая фигура (тетромино), которая находится на игровом поле.
    public TetrominoData[] tetrominoes;   //!< Массив данных о доступных тетромино.
    public Vector3Int spawnPosition;  //!< Позиция спавна (появления) для новых фигур.
    public Vector2Int boardSize = new Vector2Int(10, 20); //!< Размер игрового поля (ширина и высота).
    private int score = 0;  //!< Текущий счет игрока в игре.
    public Text LinesClearedText = null;   //!< Текстовое поле для отображения количества очищенных линий.
    private int linesCleared = 0; //!< Количество очищенных линий.
    private int level = 1;   //!< Уровень игры.
    public Text LevelText = null;  //!< Текстовое поле для отображения текущего уровня.
    private const int linesToLevelUp = 10;   //!< Количество линий, необходимых для повышения уровня.
    public GameOverUIManager gameOverUIManager;  //!< Ссылка на класс GameOverUIManager для отображения окна во время проигрыша.
    private bool isGameOver = false;  //!< Указывает, завершена ли игра.
    /// <summary>
    /// Уровень текущей игры.
    /// </summary>
    /// <remarks>
    /// При изменении уровня автоматически обновляется скорость тетромино.
    /// </remarks>
    public int Level                                       
    {
        get { return level; }
        set
        {
            level = value;
            UpdatePieceSpeed(); /// Обновляем скорость тетромино при повышении уровня
        }
    }

    public AudioSource gameOverAudio;  //!< Звук для окончания игры.
    public AudioClip lineClearSound;   //!< Звук для очищенной линии.
    private AudioSource audioSource;   //!< Позволяет проигрывать звук один раз.
    public AudioClip levelUpSound;     //!< Звук для повышения уровня.

    /*! \brief Свойство для получения границ игрового поля.
    *
    * Это свойство возвращает объект типа RectInt, который представляет границы игрового поля. Границы определяются на основе заданного
    * размера поля (boardSize) и вычисляются относительно центра игрового поля.
    *
    * \return Возвращает RectInt, представляющий границы игрового поля.
    *         Границы начинаются с центральной точки поля и имеют размеры, определенные свойством boardSize.
    */
    public RectInt Bounds 
     {
          get 
          {
               Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this. boardSize.y / 2);
               return new RectInt(position, this.boardSize);
          }
     }

/*! \brief Метод вызывается при инициализации объекта.
*
* Этот метод вызывается автоматически Unity при создании объекта. 
* Он предназначен для инициализации компонентов, необходимых для работы данного объекта. В частности, он получает ссылки 
* на компоненты Tilemap и Piece из дочерних объектов, а также инициализирует массив тетромино.
*
* - Получает компонент Tilemap из дочерних объектов для управления плитками на игровом поле.
* - Получает компонент Piece из дочерних объектов, который отвечает за активные тетромино.
* - Инициализирует каждый элемент в массиве тетромино, вызывая метод Initialize() для каждого из них.
*
* \note Этот метод вызывается до метода Start() и используется для подготовки объекта к использованию.
*/
    private void Awake() 
     {
          this.tilemap = GetComponentInChildren<Tilemap>();
          this.activePiece = GetComponentInChildren<Piece>();

          for (int i = 0; i < this.tetrominoes.Length; i++)
          {
               this.tetrominoes[i].Initialize();
          }
     }

/*! \brief Метод вызывается при старте игры.
*
* Этот метод инициализирует игровую сцену, создавая новый игровой объект (piece) и устанавливая его как текущий активный объект. 
* Также добавляется компонент AudioSource для воспроизведения звуковых эффектов в игре.
*
* \details
* - Вызывает метод \ref SpawnPiece() для создания нового игрового объекта.
* - Устанавливает переменную \ref currentPiece в значение переменной \ref activePiece, чтобы отслеживать текущий активный объект.
* - Добавляет компонент AudioSource к текущему объекту для управления 
*   звуковыми эффектами.
*
* \note Убедитесь, что переменная \ref activePiece инициализирована перед вызовом этого метода.
*/
    private void Start() 
     {
          SpawnPiece();
          currentPiece = activePiece;
          audioSource = gameObject.AddComponent<AudioSource>();
     }

/*! \brief Метод для спавна новой фигуры (тетромино).
 *
 * Этот метод генерирует случайный индекс для выбора тетромино из массива доступных фигур, инициализирует активную фигуру с заданными данными и 
 * проверяет, можно ли разместить её в начальной позиции.
 *
 * \details
 * - Генерирует случайный индекс для выбора тетромино из массива \ref tetrominoes.
 * - Инициализирует активную фигуру с помощью метода \ref Initialize() с передачей текущего объекта, позиции спавна и данных тетромино.
 * - Проверяет, можно ли разместить активную фигуру в начальной позиции,вызывая метод \ref IsValidPosition().
 * - Если позиция действительна, устанавливает фигуру на поле, вызывая метод \ref Set(). В противном случае вызывает метод \ref GameOver().
 *
 * \note Убедитесь, что массив \ref tetrominoes и переменная \ref spawnPosition правильно инициализированы перед вызовом этого метода.
 */    
    public void SpawnPiece() 
    {
          int random = Random.Range(0, this.tetrominoes.Length);
          TetrominoData data = this.tetrominoes[random];

          this.activePiece.Initialize(this, this.spawnPosition, data);         

          if(IsValidPosition(this.activePiece, this.spawnPosition)){
               Set(this.activePiece); 
          } else {
               GameOver();
          }
    }
 /*! \brief Обрабатывает завершение игры.
 *
 * Этот метод устанавливает флаг завершения игры и останавливает игровой процесс,
 * если игра еще не завершена. Он также отображает экран завершения игры и воспроизводит звуковой эффект.
 *
 * \details
 * - Проверяет, не завершена ли игра, используя флаг \ref isGameOver.
 * - Если игра не завершена, устанавливает флаг \ref isGameOver в true.
 * - Останавливает игровой процесс, устанавливая \ref Time.timeScale в 0.
 * - Вызывает метод \ref ShowGameOverUI() у объекта \ref gameOverUIManager для 
 *   отображения пользовательского интерфейса завершения игры, передавая текущий счет и уровень.
 * - Воспроизводит звуковой эффект завершения игры с помощью метода \ref Play() у объекта \ref gameOverAudio.
 *
 * \note Убедитесь, что все необходимые компоненты (UI менеджер и аудио) правильно инициализированы перед вызовом этого метода.
 */
     private void GameOver()
     {
        if (!isGameOver) 
        {
                isGameOver = true; 
                Time.timeScale = 0; 
                gameOverUIManager.ShowGameOverUI(score, level);
                gameOverAudio.Play();
        }
     }
    /*! \brief Перезапускает игру.
     *
     * Этот метод сбрасывает состояние игры, позволяя игроку начать новую партию. 
     * Он устанавливает флаг завершения игры в false, восстанавливает временные 
     * параметры, сбрасывает счет и уровень, а также очищает игровое поле и 
     * создает новую игровую фигуру.
     *
     * \details
     * - Устанавливает флаг \ref isGameOver в false, чтобы указать, что игра не завершена.
     * - Восстанавливает \ref Time.timeScale до 1, чтобы возобновить игровой процесс.
     * - Сбрасывает \ref score до 0, чтобы начать новый подсчет очков.
     * - Сбрасывает \ref level до 1, чтобы начать с первого уровня.
     * - Вызывает метод \ref ClearAllTiles() у объекта \ref tilemap для очистки игрового поля от всех фигур.
     * - Запускает новую игру, вызывая метод \ref SpawnPiece(), который создает новую фигуру для игры.
     *
     * \note Убедитесь, что все необходимые компоненты правильно инициализированы 
     *       перед вызовом этого метода. Этот метод может быть вызван после 
     *       завершения игры, чтобы предоставить игроку возможность начать заново.
     */
    public void RestartGame()
    {
        isGameOver = false; 
        Time.timeScale = 1; 
        score = 0; 
        level = 1; 
        this.tilemap.ClearAllTiles(); 
        SpawnPiece(); 
    }

    /*! \brief Устанавливает фигуру на игровое поле.
     *
     * Этот метод принимает объект фигуры и устанавливает его на игровом поле, 
     * добавляя соответствующие тайлы в заданные позиции.
     *
     * \param piece Фигура, которую необходимо установить.
     *
     * \details
     * - Метод проходит по всем ячейкам фигуры и определяет их позиции на игровом поле.
     * - Для каждой ячейки устанавливается соответствующий тайл на игровом поле.
     */
    public void Set(Piece piece)
    {
        
        for (int i = 0; i < piece.cells.Length; i++)
        {
            
            Vector3Int tilePosition = piece.cells[i] + piece.position;

            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    /*! \brief Удаляет фигуру с игрового поля.
     *
     * Этот метод принимает объект фигуры и удаляет его с игрового поля,
     * очищая соответствующие тайлы из заданных позиций.
     *
     * \param piece Фигура, которую необходимо удалить.
     *
     * \details
     * - Метод проходит по всем ячейкам фигуры и определяет их позиции на игровом поле.
     * - Для каждой ячейки удаляется соответствующий тайл с игрового поля.
     */
    public void Clear(Piece piece)
    {
        
        for (int i = 0; i < piece.cells.Length; i++)
        {
           
            Vector3Int tilePosition = piece.cells[i] + piece.position;

            this.tilemap.SetTile(tilePosition, null);
        }
    }

    /*! \brief Проверяет, является ли заданная позиция допустимой для указанной фигуры.
     *
     * Этот метод проверяет, помещается ли фигура в указанную позицию на игровом поле,
     * не выходя за его границы и не перекрываясь с уже установленными тайлами.
     *
     * \param piece Фигура, для которой проверяется позиция.
     * \param position Позиция, которую необходимо проверить для размещения фигуры.
     * \return Возвращает true, если позиция допустима; иначе false.
     */
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

    /*! \brief Очищает заполненные линии на игровом поле.
  *
  * Этот метод проходит по всем строкам игрового поля и проверяет,
  * заполнены ли они полностью. Если строка заполнена, она очищается,
  * и счетчик очищенных линий увеличивается.
  *
  * \details
  * - Метод начинает с нижней строки и проходит до верхней.
  * - При обнаружении заполненной строки вызывается метод LineClear для ее очистки.
  * - После завершения проверки всех строк обновляется счет игрока и уровень игры.
  *
  * \note
  * Убедитесь, что метод вызывается после каждой полной линии, чтобы
  * гарантировать корректное обновление состояния игры.
  */
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

    /*! \brief Проверяет, заполнена ли строка полностью.
     *
     * Этот метод проверяет, содержит ли указанная строка тайлы во всех колонках.
     *
     * \param row Номер строки, которую необходимо проверить.
     * \return Возвращает true, если строка заполнена полностью; иначе false.
     *
     * \details
     * - Метод проходит по всем колонкам в заданной строке.
     * - Если хотя бы одна колонка не содержит тайла, метод возвращает false.
     *
     * \note
     * Этот метод предполагает, что строки находятся в пределах границ игрового поля.
     */
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

    /*! \brief Метод отвечает за очистку указанной строки (линии) на игровом поле.
     * 
     * Этот метод получает границы игрового поля, удаляет тайлы в заполненной строке и 
     * сдвигает все тайлы выше вниз на одну позицию.
     * 
     * \param row Номер строки, к которой переходим.
     *
     */
    private void LineClear(int row)
     {
          RectInt bounds = this.Bounds;        

          for(int col = bounds.xMin; col < bounds.xMax; col++)
          {
               Vector3Int position = new Vector3Int(col, row, 0);
               this.tilemap.SetTile(position, null);
          }
          audioSource.PlayOneShot(lineClearSound);

          /// Сдвигаем все тайлы выше вниз на одну позицию
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

     /*! \brief Метод для ведения счета.
      * 
      * Этот метод позволяет корректно вести счёт игры. Счёт - это количество очищенных линий.
      */
     private void UpdateScore(int linesCleared)
     {
          if(linesCleared > 0){
               score += linesCleared;         
          }
 
          if(LinesClearedText != null){
               LinesClearedText.text = "Score: " + score.ToString();    
          }
          
     }

     /*! \brief Метод для повышения уровня.
      * 
      * Этот метод проверяет, достаточно ли линий очистилось с поля (а  именно 10) для повышения уровня.
      * Если да, то уровень повышается, увеличивается скорость падения фигур, и уменьшается счётчик очищенных линий 
      * для корректной работы.
      * 
      */
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
    /*! \brief Метод для повышения скорости.
     * 
     * Этот метод увеличивает скорость на 0.5f на каждом уровне.
     */
    private void UpdatePieceSpeed()
    {
       
     currentPiece.Speed = 1 + (level - 1) * 0.5f; 
     currentPiece.UpdateStepDelay();
    }
}
 