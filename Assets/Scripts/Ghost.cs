using UnityEngine;
using UnityEngine.Tilemaps;

/*! \brief Класс предназначен для отображения фигуры-призрака для удобства игры пользователя.
 */
public class Ghost : MonoBehaviour
{
    public Tile tile;                //!< Ссылка на тайл, который будет использоваться для отображения призрачного тетромино
    public Board board;              //!< Ссылка на игровое поле, чтобы проверять допустимость положения
    public Piece trackingPiece;      //!< Ссылка на отслеживаемый тетромино (фигуру), для которой отображается призрак

    public Tilemap tilemap { get; private set; }      //!< Тайловая карта, используемая для отрисовки призрачного тетромино
    public Vector3Int[] cells { get; private set; }   //!< Массив ячеек, представляющий форму тетромино
    public Vector3Int position { get; private set; }  //!< Позиция, где будет отображаться призрак тетромино

    /*! \brief Метод получает компонент Tilemap из дочернего объекта
     * и инициализирует массив ячеек для хранения позиций тетромино.
     */
    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();       
        this.cells = new Vector3Int[4];                         
    }

    /*! \brief Метод очищает предыдущую позицию призрака, копирует текущие ячейки отслеживаемого тетромино,
     * определяет, куда призрак будет "падать" и устанавливает призрак на новую позицию.
    */
    private void LateUpdate() 
    {
        Clear();            
        Copy();             
        Drop();            
        Set();              
    }

    /*! \brief Метод очищает тайлы в предыдущих позициях призрака.
     * 
     * Этот метод проходит по всем ячейкам текущего тетромино и удаляет соответствующие 
     * тайлы из тайлмапа, устанавливая их значение в null. Это полезно для подготовки к дальнейшим 
     * маневрам или изменениям положения тетромино в игровом поле.
    */
    private void Clear()
    {
        foreach (var cell in cells)
        {
            Vector3Int tilePosition = cell + position;   
            tilemap.SetTile(tilePosition, null);          
        }
    }

    /*! \brief Метод копирует текущие ячейки отслеживаемого тетромино в массив cells.
     * 
     * Этот метод проходит по всем ячейкам отслеживаемого тетромино и копирует их значения в 
     * соответствующие ячейки текущего тетромино. Это полезно для обновления состояния 
     * текущего тетромино, чтобы оно совпадало с отслеживаемым.
    */
    private void Copy()
    {
        for(int i = 0; i < this.cells.Length; i++)
        {
            this.cells[i] = this.trackingPiece.cells[i];
        }
    }

    /*! \brief Метод перемещает отслеживаемое тетромино вниз до самой нижней допустимой позиции на игровом поле.
     * 
     * Этот метод осуществляет проверку допустимости каждой позиции вниз от текущей
     * позиции тетромино до нижней границы игрового поля. Если новая позиция является 
     * допустимой, то обновляется положение тетромино. Процесс повторяется, пока не будет 
     * достигнута нижняя граница или не будет найден недопустимый ввод.
    */
    private void Drop()
    {
        Vector3Int position = this.trackingPiece.position;    

        int current = position.y;                              
        int bottom = -this.board.boardSize.y / 2 - 1;          

        this.board.Clear(this.trackingPiece);                  

        for (int row = current; row >= bottom; row--)
        {
            position.y = row;                                
            if (this.board.IsValidPosition(this.trackingPiece, position)){
                this.position = position;                    
            } else {
                break;                                       
            }
        }

        this.board.Set(this.trackingPiece);             
    }

    /*! \brief Устанавливает тайлы для отображения призрачного тетромино на новой позиции.
 *
 * Этот метод проходит по всем ячейкам текущего тетромино и устанавливает соответствующие
 * тайлы в тайловую карту, основываясь на текущей позиции. Это позволяет визуализировать
 * новое положение призрачного тетромино, что помогает игроку принимать решения.
 */
    private void Set()
    {
        foreach (var cell in cells)
        {
            Vector3Int tilePosition = cell + position;         
            tilemap.SetTile(tilePosition, tile);               
        } 
    }
}
 