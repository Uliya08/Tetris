using System.Collections.Generic;
using UnityEngine;

public class MyGrid : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public int columns = 9;
    public int rows = 9;
    public float squaresGap = 0.1f;
    public GameObject gridSquare;
    public Vector2 startPosition = new Vector2(x: 0.0f, y: 0.0f);
    public float squareScale = 0.5f;
    public float everySquareOffset = 0.0f;
    public SquareTextureData squareTextureData;

    private Vector2 _offset = new Vector2(x: 0.0f, y: 0.0f);
    private List<GameObject> _gridSquares = new List<GameObject>();


    private LineIndicator _lineIndicator;
    private Config.SquareColor currentActiveSquareColor_ = Config.SquareColor.NotSet;
    private List<Config.SquareColor> colorsInTheGrid_ = new List<Config.SquareColor>();
    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
        GameEvents.UpdateSquareColor += OnUpdateSquareColor;
    }

    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
        GameEvents.UpdateSquareColor -= OnUpdateSquareColor;
    }

    void Start()
    {
        _lineIndicator = GetComponent<LineIndicator>();
        CreateGrid();
        currentActiveSquareColor_ = squareTextureData.activeSquareTextures[0].squareColor;
    }

    private void OnUpdateSquareColor(Config.SquareColor color)
    {
        currentActiveSquareColor_ = color;
    }

    private List<Config.SquareColor> GetAllSquareColorsInTheGrid()
    {
        var colors = new List<Config.SquareColor>();
        foreach(var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if (gridSquare.SquareOccupied)
            {
                var color = gridSquare.GetCurrentColor();
                if(colors.Contains(color) == false)
                {
                    colors.Add(color);
                }
            }
        }

        return colors;
    }

    private void CreateGrid()
    {
        SpawnGridSquares();
        SetGridSquarePositions();
    }

    private void SpawnGridSquares()
    {
        int square_index = 0;
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                _gridSquares.Add(Instantiate(gridSquare, transform));
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SquareIndex = square_index;
                _gridSquares[_gridSquares.Count - 1].transform.localScale = new Vector3(squareScale, squareScale, squareScale);
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SetImage(_lineIndicator.GetGridSquareIndex(square_index) % 2 == 0);
                square_index++;
            }
        }
    }

    private void SetGridSquarePositions()
    {
        int column_number = 0;
        int row_number = 0;
        Vector2 square_gap_number = Vector2.zero;
        bool row_moved = false;

        var square_rect = _gridSquares[0].GetComponent<RectTransform>();
        _offset.x = square_rect.rect.width * square_rect.transform.localScale.x + everySquareOffset;
        _offset.y = square_rect.rect.height * square_rect.transform.localScale.y + everySquareOffset;

        foreach (GameObject square in _gridSquares)
        {
            if (column_number + 1 > columns)
            {
                square_gap_number.x = 0;
                column_number = 0;
                row_number++;
                row_moved = false;
            }

            var pos_x_offset = _offset.x * column_number + (square_gap_number.x * squaresGap);
            var pos_y_offset = _offset.y * row_number + (square_gap_number.y * squaresGap);

            if (column_number > 0 && column_number % 3 == 0)
            {
                square_gap_number.x++;
                pos_x_offset += squaresGap;
            }

            if (row_number > 0 && row_number % 3 == 0 && !row_moved)
            {
                row_moved = true;
                square_gap_number.y++;
                pos_y_offset += squaresGap;
            }

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPosition.x + pos_x_offset, startPosition.y - pos_y_offset);
            column_number++;
        }
    }

    private void CheckIfShapeCanBePlaced()
    {
        var squareIndexes = new List<int>();

        foreach (var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if (gridSquare.Selected && !gridSquare.SquareOccupied)
            {
                squareIndexes.Add(gridSquare.SquareIndex);
                gridSquare.Selected = false;
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if (currentSelectedShape == null) return;

        if (currentSelectedShape.TotalSquareNumber == squareIndexes.Count && ShapesMatch(currentSelectedShape, squareIndexes))
        {
            PlaceShape(currentSelectedShape, squareIndexes);
            CheckGameState();
        }
        else
        {
            GameEvents.MoveShapeToStartPosition();
        }
    }
    private bool ShapesMatch(Shape shape, List<int> squareIndexes)
    {
        var shapeData = shape.CurrentShapeData;
        List<Vector2Int> originalShape = new List<Vector2Int>();

        for (int y = 0; y < shapeData.rows; y++)
        {
            for (int x = 0; x < shapeData.columns; x++)
            {
                if (shapeData.board[y].column[x])
                {
                    originalShape.Add(new Vector2Int(x, y));
                }
            }
        }

        List<Vector2Int> placedCoords = new List<Vector2Int>();
        foreach (var index in squareIndexes)
        {
            placedCoords.Add(new Vector2Int(index % columns, index / columns));
        }

        return CompareShapes(originalShape, placedCoords);
    }
    private bool CompareShapes(List<Vector2Int> shape1, List<Vector2Int> shape2)
    {
        if (shape1.Count != shape2.Count) return false;

        // Ќаходим смещение дл€ центрировани€
        var offset = shape2[0] - shape1[0];

        foreach (var coord in shape1)
        {
            if (!shape2.Contains(coord + offset))
                return false;
        }

        return true;
    }
    private void PlaceShape(Shape shape, List<int> squareIndexes)
    {
        foreach (var squareIndex in squareIndexes)
        {
            _gridSquares[squareIndex].GetComponent<GridSquare>().PlaceShapeOnBoard(currentActiveSquareColor_);
        }
    }
    private void CheckGameState()
    {
        var shapeLeft = 0;
        foreach (var shape in shapeStorage.shapeList)
        {
            if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
            {
                shapeLeft++;
            }
        }

        if (shapeLeft == 0)
        {
            GameEvents.RequestNewShapes();
        }
        else
        {
            GameEvents.SetShapeInactive();
        }

        CheckIfAnyLineIsCompleted();
    }

    void CheckIfAnyLineIsCompleted()
    {
        List<int[]> lines = new List<int[]>();

        //columns
        foreach (var column in _lineIndicator.columnIndexes)
        {
            lines.Add(_lineIndicator.GetVerticalLine(column));
        }

        //rows
        for (var row = 0; row < 9; row++)
        {
            List<int> data = new List<int>(9);
            for (var index = 0; index < 9; index++)
            {
                data.Add(_lineIndicator.line_data[row, index]); 
            }

            lines.Add(data.ToArray());
        }

        //squares
        for(var square = 0; square < 9; square++)
        {
            List<int> data = new List<int>(9);
            for(var index = 0; index < 9; index++)
            {
                data.Add(_lineIndicator.square_data[square, index]);
            }

            lines.Add(data.ToArray());
        }

        colorsInTheGrid_ = GetAllSquareColorsInTheGrid();

        var completedLines = CheckIfSquaresAreCompleted(lines);

        if(completedLines >= 2)
        {
            GameEvents.ShowCongratulationWritings();
        }

        var totalScores = 10 * completedLines;
        var bonusScores = ShouldPlayColorBonusAnimation();
        GameEvents.AddScores(totalScores + bonusScores);
        CheckIfPlayerLost();
    } 

    private int ShouldPlayColorBonusAnimation()
    {
        var colorsInTheGridAfterLineRemoved = GetAllSquareColorsInTheGrid();
        Config.SquareColor colorToPlayBonusFor = Config.SquareColor.NotSet;

        foreach(var squareColor in colorsInTheGrid_)
        {
            if(colorsInTheGridAfterLineRemoved.Contains(squareColor) == false)
            {
                colorToPlayBonusFor = squareColor;
            }
        }

        if(colorToPlayBonusFor == Config.SquareColor.NotSet)
        {
            Debug.Log("Cannot find Color for bonus");
            return 0;
        }

        //Ѕонус никогда не должен примен€тьс€ к текущему цвету.
        if(colorToPlayBonusFor == currentActiveSquareColor_)
        {
            return 0;
        }

        GameEvents.ShowBonusScreen(colorToPlayBonusFor);

        return 50;
    }

    private int CheckIfSquaresAreCompleted(List<int[]> data) 
    {
        List<int[]> completedLines = new List<int[]>();

        var linesCompleted = 0;

        foreach(var line in data)
        {
            var lineCompleted = true;

            foreach(var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                if(comp.SquareOccupied == false)
                {
                    lineCompleted = false;
                }
            }

            if(lineCompleted) 
            {
                completedLines.Add(line);   
            }
        }

        foreach(var line in completedLines)
        {
            var completed = false;

            foreach(var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.Deactivate();
                completed = true;
            }

            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.ClearOccupied();
            }

            if (completed)
            {
                linesCompleted++;
            }
        }
        return linesCompleted;
    }

    private void CheckIfPlayerLost()
    {
        var validShapes = 0;
        foreach (var shape in shapeStorage.shapeList)
        {
            if (shape.IsAnyOfShapeSquareActive() && CheckIfShapeCanBePlacedOnGrid(shape))
            {
                validShapes++;
            }
        }

        if (validShapes == 0)
        {
            var scoresManager = FindFirstObjectByType<Scores>();
            bool isNewBest = scoresManager != null && scoresManager.IsNewBestScore();
            GameEvents.GameOver?.Invoke(isNewBest);
        }
    }

    private bool CheckIfShapeCanBePlacedOnGrid(Shape currentShape)
    {
        var currentShapeData = currentShape.CurrentShapeData;
        var shapeColumns = currentShapeData.columns;
        var shapeRows = currentShapeData.rows;
        List<int> originalShapeFilledUpSquares = new List<int>();
        var squareIndex = 0;
        for (var rowIndex = 0; rowIndex < shapeRows; rowIndex++)
        {
            if (rowIndex >= currentShapeData.board.Length)
            {
                Debug.LogError($"rowIndex out of bounds: {rowIndex}, board length: {currentShapeData.board.Length}");
                break; 
            }

            for (var columnIndex = 0; columnIndex < shapeColumns; columnIndex++)
            {
                if (columnIndex >= currentShapeData.board[rowIndex].column.Length)
                {
                    Debug.LogError($"columnIndex out of bounds for board row {rowIndex}: {columnIndex}");
                    break; // «авершить выполнение цикла
                }

                if (currentShapeData.board[rowIndex].column[columnIndex])
                {
                    originalShapeFilledUpSquares.Add(squareIndex);
                }
                squareIndex++;
            }
        }

        if (currentShape.TotalSquareNumber != originalShapeFilledUpSquares.Count)
        {
            Debug.LogError(" оличество заполненных квадратов не совпадает с исходным количеством квадратов");
        }

        var squareList = GetAllSquaresCombination(shapeColumns, shapeRows);
        bool canBePlaced = false;

        foreach (var number in squareList)
        {
            bool shapeCanBePlacedOnTheBoard = true;

            foreach (var squareIndexToCheck in originalShapeFilledUpSquares)
            {
                if (squareIndexToCheck < 0 || squareIndexToCheck >= number.Length)
                {
                    shapeCanBePlacedOnTheBoard = false;
                    break; 
                }

                var gridIndex = number[squareIndexToCheck];
                var comp = _gridSquares[gridIndex].GetComponent<GridSquare>();
                if (comp.SquareOccupied)
                {
                    shapeCanBePlacedOnTheBoard = false;
                    break; 
                }
            }

            if (shapeCanBePlacedOnTheBoard)
            {
                canBePlaced = true; 
                break; 
            }
        }

        return canBePlaced;
    }


    private List<int[]> GetAllSquaresCombination(int columns, int rows)
    {
        var squareList = new List<int[]>();
        var lastColumnIndex = 0; 
        var lastRowIndex = 0;

        int safetyIndex = 0;
        while (lastRowIndex + (rows - 1) < 9) 
        {
            var rowData = new List<int>();

            for (var row = lastRowIndex; row < lastRowIndex + rows; row++) 
            { 
                for (var column = lastColumnIndex; column < lastColumnIndex + columns; column++)
                {
                    rowData.Add(_lineIndicator.line_data[row, column]);
                }
            }

            squareList.Add(rowData.ToArray());
            lastColumnIndex++;
            if (lastColumnIndex + (columns - 1) >= 9)
            {
                lastRowIndex++;
                lastColumnIndex = 0;
            }

            safetyIndex++;
            if (safetyIndex > 100)
            {
                break;
            }

        }
        return squareList;

    }

}
