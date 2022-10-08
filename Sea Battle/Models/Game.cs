using System.Collections.Generic;
using System.Windows;

namespace Sea_Battle.Models;

public enum Player
{
    First,
    Second
}

internal enum TurnStatus
{
    FirstPlayerPreparation,
    SecondPlayerPreparation,
    FirstGoes,
    SecondGoes
}

public class Game
{
    private TurnStatus _turn;
    private readonly Dictionary<Player, PersonalTable> _tables;

    private Command FirstPlayerPreparationStep(Player cellOwner, int row, int column)
    {
        if (cellOwner != Player.First) return new Command(Player.First, DrawingType.Empty, 0, 0);

        if (_tables[Player.First].MyCell(row, column) == CellStatus.Empty)
        {
            _tables[Player.First].PreparationStep(row, column);
            return new Command(Player.First, DrawingType.DrawShipPart, row, column);
        }

        if (_tables[Player.First].MyCell(row, column) == CellStatus.PartOfMyShip)
        {
            _tables[Player.First].PreparationStep(row, column);
            return new Command(Player.First, DrawingType.EraseShipPart, row, column);
        }

        return new Command(Player.First, DrawingType.Empty, 0, 0); // zaglushka
    }

    public Game()
    {
        _turn = TurnStatus.FirstPlayerPreparation;
        _tables = new Dictionary<Player, PersonalTable>
        {
            {Player.First, new PersonalTable()},
            {Player.Second, new PersonalTable()}
        };
    }

    public Command Click(Player cellOwner, int row, int column)
    {
        if (row == -1 && column == -1)
            if (_turn == TurnStatus.FirstPlayerPreparation)
            {
                if (_tables[Player.First].CheckThatPlacementIsCorrect() && _tables[Player.First].CheckThatShipsAraFine())
                {
                    MessageBox.Show("ok");
                }
                
                return new Command(Player.First, DrawingType.Empty, 0, 0); // zaglushka
            }

        if (_turn == TurnStatus.FirstPlayerPreparation) return FirstPlayerPreparationStep(cellOwner, row, column);

        if (_turn == TurnStatus.SecondPlayerPreparation)
        {
            //Todo
        }

        if (_turn == TurnStatus.FirstGoes)
        {
            //Todo
        }

        if (_turn == TurnStatus.SecondGoes)
        {
            //Todo
        }

        return new Command(Player.First, DrawingType.Empty, 0, 0); // zaglushka
    }
}