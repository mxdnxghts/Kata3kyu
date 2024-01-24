int[,] _field = new int[10, 10]
                     {{1, 0, 0, 0, 0, 1, 1, 0, 0, 0},
                      {1, 0, 1, 0, 0, 0, 0, 0, 1, 0},
                      {1, 0, 1, 0, 1, 1, 1, 0, 1, 0},
                      {1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                      {0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
                      {0, 0, 0, 0, 1, 1, 1, 0, 0, 0},
                      {0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
                      {0, 0, 0, 1, 0, 0, 0, 0, 0, 0},
                      {0, 0, 0, 0, 0, 0, 0, 1, 0, 0},
                      {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}};

Console.WriteLine(ValidateBattlefield(_field));

static bool ValidateBattlefield(int[,] field)
{
    var exclusionIndexes = new List<Index>();
    var ships = new Dictionary<int, int>()
    {
        { 1 , 0 },
        { 2 , 0 },
        { 3 , 0 },
        { 4 , 0 }
    };

    for (int columnIndex = 0; columnIndex < field.GetLength(0); columnIndex++)
    {
        for (int rowIndex = 0; rowIndex < field.GetLength(1); rowIndex++)
        {
            var cell = field[columnIndex, rowIndex];
            if (cell == 0 || exclusionIndexes.Exists(x => x.Y == columnIndex && x.X == rowIndex))
                continue;

            var severalNearest = AreSeveralNearest(field, columnIndex, ref rowIndex, ref exclusionIndexes);
            if (severalNearest.Item1)
                return false;

            var cellsSequence = severalNearest.Item2;
            if (cellsSequence < 1 && cellsSequence > 4)
                return false;

            var ship = ships.FirstOrDefault(x => x.Key == cellsSequence);

            // Update ship's count
            ships[ship.Key] += 1;
        }
    }

    return ValidateShips(ships);
}

// first - are there nearest several cells
// second - if condition is false it means length of the ship
// third - go next element in row
static (bool, int, bool) AreSeveralNearest(int[,] field, int columnIndex, ref int rowIndex, ref List<Index> exclusionIndexes)
{
    var rowCount = FindInRow(field, columnIndex, rowIndex, ref exclusionIndexes);
    var columnCount = FindInColumn(field, columnIndex, rowIndex, ref exclusionIndexes);
    var diagonalCount = FindInDiagonal(field, columnIndex, rowIndex);

    return (rowCount > 1 && columnCount > 1 || diagonalCount > 0,
        new List<int> { rowCount, columnCount }.Max(), rowCount > columnCount);
}

static int FindInDiagonal(int[,] field, int columnIndex, int rowIndex)
{
    for (int i = 0; i < field.GetLength(0) - columnIndex; i++)
    {
        var cell = field[columnIndex + i, rowIndex];
        if (cell == 0)
            return 0;
        var topLeft = false;
        var topRight = false;

        var bottomLeft = false;
        var bottomRight = false;


        if (columnIndex > 0 && rowIndex > 0)
            topLeft = field[columnIndex - 1, rowIndex - 1] == 1;

        if (columnIndex > 0 && rowIndex < field.GetLength(1) - 1)
            topRight = field[columnIndex - 1, rowIndex + 1] == 1;

        if (columnIndex < field.GetLength(0) - 1 && rowIndex > 0)
            bottomLeft = field[columnIndex + 1, rowIndex - 1] == 1;

        if(columnIndex < field.GetLength(0) && rowIndex < field.GetLength(1) - 1)
            bottomRight = field[columnIndex + 1, rowIndex + 1] == 1;

        if (topLeft || topRight || bottomLeft || bottomRight)
            return 1;
    }

    return 0;
}

static int FindInColumn(int[,] field, int columnIndex, int rowIndex, ref List<Index> exclusionIndexes)
{
    var shipLength = 0;
    for (int i = 0; i < field.GetLength(0) - columnIndex; i++)
    {
        var cell = field[columnIndex + i, rowIndex];
        if (cell == 0)
            return shipLength;

        shipLength++;
        exclusionIndexes.Add(new Index(rowIndex, columnIndex + i));
    }

    return shipLength;
}

static int FindInRow(int[,] field, int columnIndex, int rowIndex, ref List<Index> exclusionIndexes)
{
    var shipLength = 0;
    for (int i = 0; i < field.GetLength(1) - rowIndex; i++)
    {
        var cell = field[columnIndex, rowIndex + i];
        if (cell == 0)
            return shipLength;

        shipLength++;
        exclusionIndexes.Add(new Index(rowIndex + i, columnIndex));
    }

    return shipLength;
}

static bool ValidateShips(Dictionary<int, int> ships)
{
    return ships[4] == 1 && ships[3] == 2 && ships[2] == 3 && ships[1] == 4;
}

record Index(int X, int Y);
