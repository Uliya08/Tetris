using UnityEngine;
using System.Collections.Generic;

/*! \brief Статический класс Data, который содержит данные о тетромино и их поведении.
 *
 * Этот класс включает в себя предварительно вычисленные значения для тригонометрических функций,
 * матрицы вращения и информацию о различных типах тетромино и их поведении.
 */
public static class Data
{
    
    /* Предварительно вычисленные значения косинуса и синуса для 90 градусов (PI / 2)*/
    public static readonly float cos = Mathf.Cos(Mathf.PI / 2f);
    public static readonly float sin = Mathf.Sin(Mathf.PI / 2f);

    
    /// Матрица вращения для тетромино (используется для вращения)
    public static readonly float[] RotationMatrix = new float[] { cos, sin, -sin, cos };

/*! \brief Словарь, содержащий координаты ячеек для каждого типа тетромино.
 *
 * Это статическое поле представляет собой словарь, который сопоставляет 
 * каждый тип тетромино его соответствующим координатам в двумерном пространстве. 
 * Координаты задаются с использованием структуры c Vector2Int, где 
 * каждая ячейка представлена как пара целых чисел (x, y).
 *      
 */
    public static readonly Dictionary<Tetromino, Vector2Int[]> Cells = new Dictionary<Tetromino, Vector2Int[]>()
    {
        { Tetromino.I, new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int( 2, 1) } },
        { Tetromino.J, new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
        { Tetromino.L, new Vector2Int[] { new Vector2Int( 1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
        { Tetromino.O, new Vector2Int[] { new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
        { Tetromino.S, new Vector2Int[] { new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0) } },
        { Tetromino.T, new Vector2Int[] { new Vector2Int( 0, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
        { Tetromino.Z, new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int( 0, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
    };

 /*! \brief Массив смещений для вращения тетромино I.
 *
 * Этот статический массив содержит возможные смещения для тетромино I при его вращении. Каждая строка представляет 
 * набор смещений, которые применяются в зависимости от текущего положения тетромино и направления вращения.
 *
 */
    private static readonly Vector2Int[,] WallKicksI = new Vector2Int[,] 
    {
        { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int( 1, 0), new Vector2Int(-2,-1), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 1), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 2), new Vector2Int( 2,-1) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-2, 0), new Vector2Int( 1,-2), new Vector2Int(-2, 1) },
        { new Vector2Int(0, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 1), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int( 1, 0), new Vector2Int(-2,-1), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-2, 0), new Vector2Int( 1,-2), new Vector2Int(-2, 1) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 2), new Vector2Int( 2,-1) },
    };

    /*! \brief Двумерный массив, представляющий значения "wall kicks" для тетромино J, L, O, S, T и Z.
    *
    * Этот статический массив хранит наборы векторов смещения, которые 
    * используются для определения корректного перемещения тетромино при 
    * столкновении с границами игрового поля или другими тетромино. 
    * Значения представляют собой координаты смещения относительно 
    * текущего положения тетромино, позволяя ему корректно "отскакивать" 
    * от стен или других блоков.
    *
    */
    private static readonly Vector2Int[,] WallKicksJLOSTZ = new Vector2Int[,] 
    {
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0,-2), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1,-1), new Vector2Int(0, 2), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1,-1), new Vector2Int(0, 2), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0,-2), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, 1), new Vector2Int(0,-2), new Vector2Int( 1,-2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,-1), new Vector2Int(0, 2), new Vector2Int(-1, 2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,-1), new Vector2Int(0, 2), new Vector2Int(-1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, 1), new Vector2Int(0,-2), new Vector2Int( 1,-2) },
    };

    /*! \brief Статический словарь, содержащий значения "wall kicks" для различных тетромино.
    *
    * Этот статический словарь сопоставляет каждый тип тетромино его 
    * соответствующим значениям "wall kicks", которые представляют собой 
    * двумерные массивы смещений. Эти смещения используются для корректного 
    * перемещения тетромино при столкновениях с границами игрового поля 
    * или другими тетромино.
    * Каждый массив смещений в словаре хранит наборы векторов смещения, 
    * которые определяют, как тетромино должно перемещаться при попытке 
    * повернуться в условиях столкновения. Это позволяет обеспечить 
    * корректное поведение тетромино в игре и предотвратить застревание 
    * в стенах или других блоках.
    *
    */
    public static readonly Dictionary<Tetromino, Vector2Int[,]> WallKicks = new Dictionary<Tetromino, Vector2Int[,]>()
    {
        { Tetromino.I, WallKicksI },
        { Tetromino.J, WallKicksJLOSTZ },
        { Tetromino.L, WallKicksJLOSTZ },
        { Tetromino.O, WallKicksJLOSTZ },
        { Tetromino.S, WallKicksJLOSTZ },
        { Tetromino.T, WallKicksJLOSTZ },
        { Tetromino.Z, WallKicksJLOSTZ },
    };

}
