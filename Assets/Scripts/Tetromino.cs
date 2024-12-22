using UnityEngine;
using UnityEngine.Tilemaps;

/*! \brief Перечисление всех возможных типов фигурок тетриса/
 */
public enum Tetromino{
    I,           //!< Фигурка в форме "I"
    O,           //!< Фигурка в форме "O"
    T,           //!< Фигурка в форме "T"
    J,           //!< Фигурка в форме "J"
    L,           //!< Фигурка в форме "L"
    S,           //!< Фигурка в форме "S"
    Z            //!< Фигурка в форме "Z"
}
/*! \brief Структура для хранения данных о фигурках тетриса.
 */
[System.Serializable]
public struct TetrominoData{
    public Tetromino tetromino; //!< Тип фигурки тетриса (I, O, T, J, L, S, Z)
    public Tile tile;           //!< Тайл, ассоциированный с данной фигуркой тетриса (графическое представление)
    public Vector2Int[] cells { get; private set; }     //!< Массив ячеек, занимаемых фигурками на сетке
    public Vector2Int[,] wallKicks { get; private set; }  //!< Двумерный массив для хранения данных о стенах и их ударах (wall kicks) для фигурок

    /*! \brief Метод инициализации данных о фигурках.
     */
    public void Initialize(){
        this.cells = Data.Cells[this.tetromino];     
        this.wallKicks = Data.WallKicks[this.tetromino]; 
    }
}
