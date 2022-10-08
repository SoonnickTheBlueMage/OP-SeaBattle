namespace Sea_Battle.Models;

public enum CellStatus
{
    Empty,
    NoInfo,
    Hit,
    Miss,
    PartOfDestroyedShip,
    PartOfMyShip,
    PartOfEnemyShip
}

public class PersonalTable
{
    private readonly CellStatus[,] _myBoard = new CellStatus[10, 10];
    private readonly CellStatus[,] _enemyBoard = new CellStatus[10, 10];

    private CellStatus MyCell(int row, int column)
    {
        return _myBoard[row, column];
    }

    private CellStatus EnemyCell(int row, int column)
    {
        return _myBoard[row, column];
    }

    public PersonalTable()
    {
        for (var i = 0; i < 10; i++)
        for (var j = 0; j < 10; j++)
        {
            _myBoard[i, j] = CellStatus.Empty;
            _enemyBoard[i, j] = CellStatus.NoInfo;
        }
    }

    public void PreparationStep(int row, int column)
    {
        _myBoard[row, column] = CellStatus.PartOfMyShip;
    }
}