namespace Sea_Battle.Models;

public enum CellStatus
{
    NoInfo,
    Hit,
    Miss,
    PartOfDestroyedShip
}

public class PersonalTable
{
    private readonly CellStatus[,] _myBoard = new CellStatus[10, 10];
    private readonly CellStatus[,] _enemyBoard = new CellStatus[10, 10];

    public PersonalTable()
    {
        for (var i = 0; i < 10; i++)
        for (var j = 0; j < 10; j++)
        {
            _myBoard[i, j] = CellStatus.NoInfo;
            _enemyBoard[i, j] = CellStatus.NoInfo;
        }
    }

    public CellStatus MyCell(int row, int column) => _myBoard[row, column];
    
    public CellStatus EnemyCell(int row, int column) => _myBoard[row, column];


}