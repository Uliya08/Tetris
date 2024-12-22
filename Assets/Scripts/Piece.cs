using UnityEngine;

/*! \brief Класс для управления игрой.
 * 
 * Этот класс управляет поведением и взаимодействием тетромино с игровым полем,
 * включая его движение, вращение и блокировку.
 */
public class Piece : MonoBehaviour
{
    public Board board { get; private set; }            //!< Ссылка на игровое поле, с которым взаимодействует данный тетромино
    public TetrominoData data { get; private set; }     //!< Данные о тетромино, включая его форму и цвет
    public Vector3Int[] cells { get; private set; }     //!< Массив ячеек, занимаемых тетромино на игровом поле
    public Vector3Int position { get; private set; }    //!< Текущая позиция тетромино на игровом поле
    public int rotationIndex { get; private set; }      //!< Индекс текущего вращения тетромино

    private float stepDelay;                            //!< Задержка между шагами тетромино
    public float lockDelay = 0.5f;                      //!< Задержка перед блокировкой тетромино

    private float stepTime;                             //!< Время следующего шага
    private float lockTime;                             //!< Время, прошедшее с момента последней блокировки
    private float baseStepDelay = 1f;                   //!< Базовая задержка между шагами
    public float Speed { get; set; } = 1f;              //!< Публичное свойство для скорости

    public AudioSource rotationAudio, hardDropAudio;    //!< Звуковые эффекты для вращения и жесткого падения

    public float moveSpeed = 1f;                        //!< Скорость перемещения
    private float moveInterval = 0.1f;                  //!< Интервал между перемещениями
    private float nextMoveTime = 0f;                    //!< Время следующего перемещения


    /*! \brief Метод инициализации тетромино.
     * 
     * Этот метод устанавливает ссылку на игровое поле, устанавливает
     * начальную позицию тетромино, данные о тетромино (форма, цвет и т.д.),
     * инициализирует индекс вращения, устанавливает время для следующего шага и сбрасывает время блокировки.
     */
public void Initialize(Board board, Vector3Int position, TetrominoData data)  
   {
        this.board = board;                               
        this.position = position;                         
        this.data = data;                                 
        this.rotationIndex = 0;                           
        this.stepTime = Time.time + this.stepDelay;       
        this.lockTime = 0f;                               

        if (this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        for(int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
   }
   
   /*! \brief Метод обновления, вызывается каждый кадр.
    * 
    * Этот метод очищает предыдущие позиции тетромино на игровом поле,
    * увеличивает время блокировки на основе прошедшего времени и обрабатывает
    * управление фигурами.
    */
   private void Update() 
   {
        this.board.Clear(this);                    

        this.lockTime += Time.deltaTime;           

            if(Input.GetMouseButtonDown(1)) 
            {
                rotationAudio.Play();
                Rotate(-1);                
            }else if (Input.GetMouseButtonDown(0))  
            {
                rotationAudio.Play();
                Rotate(1);                          
            }

            if(Input.GetKey(KeyCode.A) && Time.time >= nextMoveTime)        
            {
                Move(Vector2Int.left);   
                nextMoveTime = Time.time + moveInterval;
            }else if(Input.GetKey(KeyCode.D) && Time.time >= nextMoveTime)  
            {
                Move(Vector2Int.right);
                nextMoveTime = Time.time + moveInterval;
            }

            if(Input.GetKey(KeyCode.S) && Time.time >= nextMoveTime)          
            {
                Move(Vector2Int.down);
                nextMoveTime = Time.time + moveInterval;    
            }
         
            if(Input.GetKeyDown(KeyCode.Space))        
            {
                hardDropAudio.Play();
                HardDrop();    
            }

            if(Time.time >= this.stepTime)
            {
                Step();                
            }

        this.board.Set(this);         
    }
    /*! \brief Конструктор класса Piece.
     * 
     * \details Инициализация задержки на основе заданной скорости.
     */
    public Piece()
    {
        UpdateStepDelay(); 
    }

    /*! \brief Метод для обновления задержки между шагами на основе скорости.
     */
    public void UpdateStepDelay()
    {
        stepDelay = Mathf.Max(baseStepDelay / Speed, 0.1f); 
    }

    /*! \brief Метод для выполнения одного шага (перемещение вниз).
     * 
     * Этот метод проверяет, достаточно ли времени прошло для следующего шага,
     * устанавливает время для следующего шага, осуществляет перемещение фигурки вниз 
     * и блокирует тетромино на игровом поле.
     */
    private void Step()
    {
        if (Time.time >= this.stepTime)
        {
            
            this.stepTime = Time.time + this.stepDelay;

            if (Move(Vector2Int.down)) 
            {
                this.lockTime = 0f; 
            }
            else
            {
                Lock(); 
            }
        }

        
        if (this.lockTime < this.lockDelay)
        {
            this.lockTime += Time.deltaTime; 
        }
    }

    /*! \brief Метод для жесткого падения тетромино (перемещение вниз до упора).
     * 
     * Этот метод позволяет фигурке перемещаться вниз до тех пор, пока
     * это возможно и блокирует тетромино после достижения границы.
     */
    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;                
        }

        Lock();                  
    }

    /*! \brief Метод для блокировки тетромино на игровом поле.
     * 
     * Этот метод устанавливает текущее состояние тетромино на игровом поле,
     * очищает заполненные линии на игровом поле и вызывает метод спавна новых фигур.
     */
    private void Lock()
    {
        this.board.Set(this);                     
        this.board.ClearLines();                  
        this.board.SpawnPiece();
    }

/*! \brief Метод, который передвигает объект на игровом поле.
 *
 * Этот метод принимает вектор смещения и пытается переместить объект
 * на новую позицию, проверяя, является ли данная позиция допустимой
 * на игровом поле.
 *
 * \param translation Вектор смещения, который указывает новое направление
 * и расстояние перемещения объекта.
 * \return true если перемещение допустимо, иначе false.
 */
    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = this.position;       
        newPosition.x += translation.x;               
        newPosition.y += translation.y;               

        bool valid = this.board.IsValidPosition(this, newPosition);     

        if(valid)
        {
            this.position = newPosition;
            this.lockTime = 0f;
        } 
        return valid;                    
    }

/*! \brief Метод, который вращает тетромино в заданном направлении.
 *
 * Этот метод изменяет индекс вращения тетромино и применяет соответствующую матрицу вращения к ячейкам тетромино.
 * Перед применением вращения выполняется проверка возможностей выполнения вращения
 * с учетом конфликтов со стенами (wall kicks). Если вращение невозможно, состояние тетромино откатывается
 * к оригинальному индексу вращения.
 *
 * \param direction Направление вращения: 
 *                  положительное (обычно 1) означает вращение по часовой стрелке, 
 *                  отрицательное (обычно -1) означает вращение против часовой стрелки.
 */
    private void Rotate(int direction)
    {
        int originalRotation = this.rotationIndex;             
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4); 

        ApplyRotationMatrix(direction);                       
        if(!TestWallKicks(this.rotationIndex, direction))
        {
            this.rotationIndex = originalRotation; 
            ApplyRotationMatrix(-direction);         
        }
    }

/*! \brief Метод применяет матрицу вращения к ячейкам тетромино.
 *
 * Этот метод изменяет координаты каждой ячейки тетромино в зависимости от направления 
 * вращения и типа тетромино. Он использует матрицу вращения для расчетов, которые позволяют 
 * корректно вращать тетромино, учитывая возможные особенности для определённых типов.
 *
 * \param direction Направление вращения: положительное значение (например, +1) означает 
 *                  вращение по часовой стрелке, а отрицательное (например, -1) — вращение 
 *                  против часовой стрелки.
 */
    private void ApplyRotationMatrix(int direction)
    {
        for(int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cell = this.cells[i];

            int x,y;

            switch(this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                cell.x -= 0.5f;
                cell.y -= 0.5f;
                
                x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y* Data.RotationMatrix[1] * direction));
                y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y* Data.RotationMatrix[3] * direction));  
                break;
            default:
                
                x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y* Data.RotationMatrix[1] * direction));
                y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y* Data.RotationMatrix[3] * direction));
                break;
            }

            this.cells[i] = new Vector3Int(x, y, 0);   
        }
    }

/*! \brief Проверяет возможность смещения тетромино (wall kicks) после его вращения.
 * 
 * Этот метод определяет, можно ли сместить тетромино при вращении, чтобы предотвратить 
 * столкновение с другими блоками или стенами. Он использует массив данных wall kicks,
 * основанный на текущем индексе вращения и направлении вращения, чтобы попытаться 
 * выполнить смещения. Успех одного из смещений будет означать, что вращение 
 * возможно.
 *
 * \param rotationIndex Индекс текущего вращения тетромино, используемый для определения
 *                     поисковых смещений в массиве wall kicks.
 * \param rotationDirection Направление вращения: положительное значение (например, +1)
 *                         означает вращение по часовой стрелке, а отрицательное (например, -1)
 *                         — вращение против часовой стрелки.
 * 
 * \return Возвращает true, если смещение тетромино успешно, иначе возвращает false.
 */
    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for(int i = 0; i < this.data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = this.data.wallKicks[wallKickIndex, i];

            if(Move(translation))
            {
                return true;      
            }
        }

        return false;                  
    }

/*! \brief Получает индекс для wall kick на основе текущего индекса вращения и направления.
 * 
 * Данный метод вычисляет нужный индекс для смещений wall kicks, учитывая направление
 * вращения. Это важно для определения правильных значений смещения для заталкивания 
 * тетромино в допустимое положение.
 *
 * \param rotationIndex Текущий индекс вращения тетромино.
 * \param rotationDirection Направление вращения: положительное (1) или отрицательное (-1).
 * 
 * \return Возвращает индекс wall kick, обернутый в пределы допустимого диапазона.
 */
    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;    

        if(rotationDirection < 0)
        {
            wallKickIndex--;                     
        }

        return Wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0));      
    }

/*! \brief Метод преобразует значение input в заданном диапазоне от min до max.
 * 
 * Этот метод обеспечивает, чтобы значение input оставалось в пределах указанного диапазона.
 * Если значение меньше минимального, оно будет преобразовано в максимальное значение. Если больше 
 * максимального, оно превратится в минимальное значение.
 *
 * \param input Значение для преобразования.
 * \param min Минимальное значение диапазона.
 * \param max Максимальное значение диапазона.
 * 
 * \return Возвращает преобразованное значение input, находящееся в пределах диапазона.
 */
    private int Wrap(int input, int min, int max)
    {
        if(input < min)
        {
            return max - (min - input) % (max - min);
        } else 
        {
            return min + (input - min) % (max - min);
        }
    }
}
