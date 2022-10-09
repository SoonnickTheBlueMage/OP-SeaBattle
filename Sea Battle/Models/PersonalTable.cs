using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sea_Battle.Models;

public enum CellStatus
{
    Empty, // only for _myBoard
    NoInfo, // only for _enemyBoard
    Hit, // for both
    Miss, // for both
    PartOfMyShip // only for _myBoard
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
            _myBoard[i, j] = CellStatus.Empty;
            _enemyBoard[i, j] = CellStatus.NoInfo;
        }
    }

    public CellStatus MyCell(int row, int column)
    {
        Debug.Assert(row is >= 0 and < 10 && column is >= 0 and <= 10);

        return _myBoard[row, column];
    }

    public CellStatus EnemyCell(int row, int column)
    {
        Debug.Assert(row is >= 0 and < 10 && column is >= 0 and <= 10);

        return _enemyBoard[row, column];
    }

    public void PreparationStep(int row, int column)
    {
        if (MyCell(row, column) == CellStatus.Empty)
            _myBoard[row, column] = CellStatus.PartOfMyShip;

        else if (MyCell(row, column) == CellStatus.PartOfMyShip)
            _myBoard[row, column] = CellStatus.Empty;
    }

    public bool CheckThatPlacementIsCorrect()
    {
        for (var rowIterator = 0; rowIterator < 9; rowIterator++)
        for (var columnIterator = 0; columnIterator < 9; columnIterator++)
        {
            var c1 = MyCell(rowIterator, columnIterator);
            var c2 = MyCell(rowIterator + 1, columnIterator);
            var c3 = MyCell(rowIterator, columnIterator + 1);
            var c4 = MyCell(rowIterator + 1, columnIterator + 1);

            if (c1 == c4 && c1 == CellStatus.PartOfMyShip || c2 == c3 && c2 == CellStatus.PartOfMyShip)
                return false; // проверяет, что корабли не стоят вплотную
        }

        return true;
    }

    public bool CheckThatShipsAraFine()
    {
        var checkerArray = new int[10, 10];
        for (var i = 0; i < 10; i++)
        for (var j = 0; j < 10; j++)
            checkerArray[i, j] = 0;

        var counter = 1;

        for (var rowIterator = 0; rowIterator < 10; rowIterator++)
        for (var columnIterator = 0; columnIterator < 10; columnIterator++)
        {
            if (MyCell(rowIterator, columnIterator) != CellStatus.PartOfMyShip) continue;

            if (checkerArray[rowIterator, columnIterator] == 0)
            {
                checkerArray[rowIterator, columnIterator] = counter;

                if (rowIterator - 1 > 0 && MyCell(rowIterator - 1, columnIterator) == CellStatus.PartOfMyShip)
                    checkerArray[rowIterator - 1, columnIterator] = counter;

                if (rowIterator + 1 < 10 && MyCell(rowIterator + 1, columnIterator) == CellStatus.PartOfMyShip)
                    checkerArray[rowIterator + 1, columnIterator] = counter;

                if (columnIterator - 1 > 0 && MyCell(rowIterator, columnIterator - 1) == CellStatus.PartOfMyShip)
                    checkerArray[rowIterator, columnIterator - 1] = counter;

                if (columnIterator + 1 < 10 && MyCell(rowIterator, columnIterator + 1) == CellStatus.PartOfMyShip)
                    checkerArray[rowIterator, columnIterator + 1] = counter;

                counter++;
            }
            else
            {
                var c = checkerArray[rowIterator, columnIterator];

                if (rowIterator - 1 > 0 && MyCell(rowIterator - 1, columnIterator) == CellStatus.PartOfMyShip)
                    checkerArray[rowIterator - 1, columnIterator] = c;

                if (rowIterator + 1 < 10 && MyCell(rowIterator + 1, columnIterator) == CellStatus.PartOfMyShip)
                    checkerArray[rowIterator + 1, columnIterator] = c;

                if (columnIterator - 1 > 0 && MyCell(rowIterator, columnIterator - 1) == CellStatus.PartOfMyShip)
                    checkerArray[rowIterator, columnIterator - 1] = c;

                if (columnIterator + 1 < 10 && MyCell(rowIterator, columnIterator + 1) == CellStatus.PartOfMyShip)
                    checkerArray[rowIterator, columnIterator + 1] = c;
            }
        }

        var max = 0;
        for (var i = 0; i < 10; i++)
        for (var j = 0; j < 10; j++)
            max = max > checkerArray[i, j] ? max : checkerArray[i, j];

        if (max != 10) return false; // проверяет, что кораблей 10 штук

        var comps = new[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

        for (var i = 0; i < 10; i++)
        for (var j = 0; j < 10; j++)
            if (checkerArray[i, j] != 0)
                comps[checkerArray[i, j] - 1] += 1;

        Array.Sort(comps);

        return comps[0] == 1 && comps[1] == 1 && comps[2] == 1 && comps[3] == 1 &&
               comps[4] == 2 && comps[5] == 2 && comps[6] == 2 && comps[7] == 3 &&
               comps[8] == 3 && comps[9] == 4; // проверяет, что корабли нужных размеров
    }

    public void SendStrike(int row, int column, CellStatus info)
    {
        Debug.Assert(row is >= 0 and < 10 && column is >= 0 and <= 10);

        _enemyBoard[row, column] = info == CellStatus.PartOfMyShip ? CellStatus.Hit : CellStatus.Miss;
    }

    public void ReceiveStrike(int row, int column)
    {
        Debug.Assert(row is >= 0 and < 10 && column is >= 0 and <= 10);

        _myBoard[row, column] = _myBoard[row, column] == CellStatus.PartOfMyShip ? CellStatus.Hit : CellStatus.Miss;
    }

    public bool CheckShipDestruction(int row, int column)
    {
        if (MyCell(row, column) != CellStatus.Hit) return false;

        for (var rowIterator = row; rowIterator >= 0; rowIterator--)
        {
            if (MyCell(rowIterator, column) is CellStatus.Miss or CellStatus.Empty) break;
            if (MyCell(rowIterator, column) == CellStatus.PartOfMyShip) return false;
        }

        for (var rowIterator = row; rowIterator < 10; rowIterator++)
        {
            if (MyCell(rowIterator, column) is CellStatus.Miss or CellStatus.Empty) break;
            if (MyCell(rowIterator, column) == CellStatus.PartOfMyShip) return false;
        }

        for (var columnIterator = column; columnIterator >= 0; columnIterator--)
        {
            if (MyCell(row, columnIterator) is CellStatus.Miss or CellStatus.Empty) break;
            if (MyCell(row, columnIterator) == CellStatus.PartOfMyShip) return false;
        }

        for (var columnIterator = column; columnIterator < 10; columnIterator++)
        {
            if (MyCell(row, columnIterator) is CellStatus.Miss or CellStatus.Empty) break;
            if (MyCell(row, columnIterator) == CellStatus.PartOfMyShip) return false;
        }

        return true;
    }

    public Tuple<List<Tuple<int, int>>?, List<Tuple<int, int>>?> MarkingDestruction(int row, int column)
    {
        Debug.Assert(CheckShipDestruction(row, column));

        var markList = new List<Tuple<int, int>>();
        var destroyList = new List<Tuple<int, int>>();

        for (var rowIterator = row; rowIterator >= 0; rowIterator--)
        {
            if (MyCell(rowIterator, column) is CellStatus.Miss or CellStatus.Empty)
            {
                markList.Add(new Tuple<int, int>(rowIterator, column));
                break;
            }

            if (MyCell(rowIterator, column) == CellStatus.Hit)
            {
                destroyList.Add(new Tuple<int, int>(rowIterator, column));

                if (column - 1 > 0 && MyCell(rowIterator, column - 1) == CellStatus.Empty)
                {
                    _myBoard[rowIterator, column - 1] = CellStatus.Miss;
                    markList.Add(new Tuple<int, int>(rowIterator, column - 1));
                }

                if (column + 1 < 10 && MyCell(rowIterator, column + 1) == CellStatus.Empty)
                {
                    _myBoard[rowIterator, column + 1] = CellStatus.Miss;
                    markList.Add(new Tuple<int, int>(rowIterator, column + 1));
                }
            }
        }

        for (var rowIterator = row; rowIterator < 10; rowIterator++)
        {
            if (MyCell(rowIterator, column) is CellStatus.Miss or CellStatus.Empty)
            {
                markList.Add(new Tuple<int, int>(rowIterator, column));
                break;
            }

            if (MyCell(rowIterator, column) == CellStatus.Hit)
            {
                destroyList.Add(new Tuple<int, int>(rowIterator, column));

                if (column - 1 > 0 && MyCell(rowIterator, column - 1) == CellStatus.Empty)
                {
                    _myBoard[rowIterator, column - 1] = CellStatus.Miss;
                    markList.Add(new Tuple<int, int>(rowIterator, column - 1));
                }

                if (column + 1 < 10 && MyCell(rowIterator, column + 1) == CellStatus.Empty)
                {
                    _myBoard[rowIterator, column + 1] = CellStatus.Miss;
                    markList.Add(new Tuple<int, int>(rowIterator, column + 1));
                }
            }
        }

        for (var columnIterator = column; columnIterator >= 0; columnIterator--)
        {
            if (MyCell(row, columnIterator) is CellStatus.Miss or CellStatus.Empty)
            {
                markList.Add(new Tuple<int, int>(row, columnIterator));
                break;
            }

            if (MyCell(row, columnIterator) == CellStatus.Hit)
            {
                destroyList.Add(new Tuple<int, int>(row, columnIterator));

                if (row - 1 > 0 && MyCell(row - 1, columnIterator) == CellStatus.Empty)
                {
                    _myBoard[row - 1, columnIterator] = CellStatus.Miss;
                    markList.Add(new Tuple<int, int>(row - 1, columnIterator));
                }

                if (row + 1 < 10 && MyCell(row + 1, columnIterator) == CellStatus.Empty)
                {
                    _myBoard[row + 1, columnIterator] = CellStatus.Miss;
                    markList.Add(new Tuple<int, int>(row + 1, columnIterator));
                }
            }
        }

        for (var columnIterator = column; columnIterator < 10; columnIterator++)
        {
            if (MyCell(row, columnIterator) is CellStatus.Miss or CellStatus.Empty)
            {
                markList.Add(new Tuple<int, int>(row, columnIterator));
                break;
            }

            if (MyCell(columnIterator, column) == CellStatus.Hit)
            {
                destroyList.Add(new Tuple<int, int>(row, columnIterator));

                if (row - 1 > 0 && MyCell(row - 1, columnIterator) == CellStatus.Empty)
                {
                    _myBoard[row - 1, columnIterator] = CellStatus.Miss;
                    markList.Add(new Tuple<int, int>(row - 1, columnIterator));
                }

                if (row + 1 < 10 && MyCell(row + 1, columnIterator) == CellStatus.Empty)
                {
                    _myBoard[row + 1, columnIterator] = CellStatus.Miss;
                    markList.Add(new Tuple<int, int>(row + 1, columnIterator));
                }
            }
        }

        return new Tuple<List<Tuple<int, int>>?, List<Tuple<int, int>>?>(markList, destroyList);
    }
}