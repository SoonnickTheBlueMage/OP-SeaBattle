using System.Collections.Generic;
using System.Windows;

namespace Sea_Battle.Models;

public enum Player
{
    First,
    Second
}

/*
 * Preparation Phase:
 * FirstPlayerPreparation -> Hide + SecondPlayerPreparation -> Hide + WaitingFirstTake -> Show + FirstGoes
 *
 * Game Phase:
 * FirstGoes (turn) -> Hide + WaitingFirstLeave -> WaitingSecondTake -> Show + SecondGoes (turn) ->
 * -> Hide + WaitingSecondLeave -> WaitingFirstTake -> Show + FirstGoes -> ... 
 */

internal enum TurnStatus
{
    FirstPlayerPreparation,
    SecondPlayerPreparation,
    FirstGoes,
    SecondGoes,
    WaitingFirstLeave,
    WaitingSecondLeave,
    WaitingFirstTake,
    WaitingSecondTake,
    GameOverFirstWin,
    GameOverSecondWin
}

internal enum Ship
{
    Carrier,
    Battleship,
    Frigate,
    AssaultBoat
}

/*
 * Этот класс отслеживает все стадии игры и все ходы.
 * Осуществляет взаимосвязь логики игры и отрисовки игры в окне.
 * Делает запросы классу PersonalTable и отправляет команду поведения в MainWindow.
 */

public class Game
{
    private TurnStatus _turn;
    private readonly Dictionary<Player, PersonalTable> _tables;

    private readonly Dictionary<Player, List<Ship>> _fleets = new()
    {
        {
            Player.First, new List<Ship>
            {
                Ship.AssaultBoat, Ship.AssaultBoat, Ship.AssaultBoat, Ship.AssaultBoat,
                Ship.Frigate, Ship.Frigate, Ship.Frigate, Ship.Battleship, Ship.Battleship, Ship.Carrier
            }
        },

        {
            Player.Second, new List<Ship>
            {
                Ship.AssaultBoat, Ship.AssaultBoat, Ship.AssaultBoat, Ship.AssaultBoat,
                Ship.Frigate, Ship.Frigate, Ship.Frigate, Ship.Battleship, Ship.Battleship, Ship.Carrier
            }
        }
    };

    private Command Preparation()
    {
        var currentPlayer = _turn == TurnStatus.FirstPlayerPreparation ? Player.First : Player.Second;

        if (_tables[currentPlayer].CheckThatPlacementIsCorrect() &&
            _tables[currentPlayer].CheckThatShipsAraFine())
        {
            if (currentPlayer == Player.First)
            {
                _turn = TurnStatus.SecondPlayerPreparation;
                return new Command(currentPlayer, DrawingType.Hide);
            }

            _turn = TurnStatus.WaitingFirstTake;
            return new Command(currentPlayer, DrawingType.Hide);
        }

        MessageBox.Show("Расстановка не удовлетворяет правилам");

        return new Command(currentPlayer, DrawingType.Empty);
    }

    private Command PlayerEndsTurn()
    {
        var currentPlayer = _turn == TurnStatus.WaitingFirstLeave ? Player.First : Player.Second;

        _turn = _turn == TurnStatus.WaitingFirstLeave
            ? TurnStatus.WaitingSecondTake
            : TurnStatus.WaitingFirstTake;

        return new Command(currentPlayer, DrawingType.Hide);
    }

    private Command PlayerStartsTurn()
    {
        var currentPlayer = _turn == TurnStatus.WaitingFirstTake ? Player.First : Player.Second;

        _turn = _turn == TurnStatus.WaitingFirstTake ? TurnStatus.FirstGoes : TurnStatus.SecondGoes;

        return new Command(currentPlayer, DrawingType.Show);
    }

    private Command PlayerPreparationStep(Player cellOwner, int row, int column)
    {
        if (_turn == TurnStatus.FirstPlayerPreparation && cellOwner != Player.First ||
            _turn == TurnStatus.SecondPlayerPreparation && cellOwner != Player.Second)
            return new Command(cellOwner, DrawingType.Empty);

        if (_tables[cellOwner].MyCell(row, column) == CellStatus.Empty)
        {
            _tables[cellOwner].PreparationStep(row, column);
            return new Command(cellOwner, DrawingType.DrawShipPart, row, column);
        }

        if (_tables[cellOwner].MyCell(row, column) == CellStatus.PartOfMyShip)
        {
            _tables[cellOwner].PreparationStep(row, column);
            return new Command(cellOwner, DrawingType.EraseShipPart, row, column);
        }

        return new Command(cellOwner, DrawingType.Empty); // zaglushka
    }

    private Command Turn(Player cellOwner, int row, int column)
    {
        if (_turn == TurnStatus.FirstGoes && cellOwner == Player.First ||
            _turn == TurnStatus.SecondGoes && cellOwner == Player.Second)
            return new Command(cellOwner, DrawingType.Empty);

        var currentPlayer = _turn == TurnStatus.FirstGoes ? Player.First : Player.Second;

        if (_tables[currentPlayer].EnemyCell(row, column) != CellStatus.NoInfo)
            return new Command(cellOwner, DrawingType.Empty);

        _tables[currentPlayer].SendStrike(row, column, _tables[cellOwner].MyCell(row, column));
        _tables[cellOwner].ReceiveStrike(row, column);

        // если уничтожили корабль
        if (_tables[cellOwner].CheckShipDestruction(row, column))
        {
            var args = _tables[cellOwner].MarkingDestruction(row, column);

            switch (args.Item2!.Count)
            {
                case 4:
                {
                    _fleets[cellOwner].Remove(Ship.Carrier);
                    break;
                }
                case 3:
                {
                    _fleets[cellOwner].Remove(Ship.Battleship);
                    break;
                }
                case 2:
                {
                    _fleets[cellOwner].Remove(Ship.Frigate);
                    break;
                }
                case 1:
                {
                    _fleets[cellOwner].Remove(Ship.AssaultBoat);
                    break;
                }
            }

            if (_fleets[cellOwner].Count == 0)
            {
                _turn = currentPlayer == Player.First ? TurnStatus.GameOverFirstWin : TurnStatus.GameOverSecondWin;
                return new Command(cellOwner, DrawingType.EndGame, row, column, args.Item1, args.Item2);
            }

            return new Command(cellOwner, DrawingType.DestroyShip, row, column, args.Item1, args.Item2);
        }

        if (_tables[currentPlayer].EnemyCell(row, column) == CellStatus.Hit)
            return new Command(cellOwner, DrawingType.DrawHit, row, column);

        _turn = _turn == TurnStatus.FirstGoes ? TurnStatus.WaitingFirstLeave : TurnStatus.WaitingSecondLeave;

        return new Command(cellOwner, DrawingType.DrawMiss, row, column);
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
        if (_turn is TurnStatus.GameOverFirstWin or TurnStatus.GameOverSecondWin)
        {
            var currentPlayer = _turn == TurnStatus.GameOverFirstWin ? Player.First : Player.Second;
            MessageBox.Show($"Игра окончена, победил {currentPlayer} Player");
            return new Command(cellOwner, DrawingType.Empty);
        }

        if (row == -1 && column == -1)
        {
            if (_turn is TurnStatus.FirstPlayerPreparation or TurnStatus.SecondPlayerPreparation) return Preparation();
            if (_turn is TurnStatus.WaitingFirstLeave or TurnStatus.WaitingSecondLeave) return PlayerEndsTurn();
            if (_turn is TurnStatus.WaitingFirstTake or TurnStatus.WaitingSecondTake) return PlayerStartsTurn();

            return new Command(Player.First, DrawingType.Empty);
        }

        if (_turn is TurnStatus.FirstPlayerPreparation or TurnStatus.SecondPlayerPreparation)
            return PlayerPreparationStep(cellOwner, row, column);

        if (_turn is TurnStatus.FirstGoes or TurnStatus.SecondGoes)
            return Turn(cellOwner, row, column);

        return new Command(Player.First, DrawingType.Empty); // zaglushka
    }
}