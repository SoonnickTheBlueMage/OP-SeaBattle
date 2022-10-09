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

    private Command PlayerPreparationStep(Player cellOwner, int row, int column)
    {
        if (_turn == TurnStatus.FirstPlayerPreparation && cellOwner != Player.First ||
            _turn == TurnStatus.SecondPlayerPreparation && cellOwner != Player.Second)
            return new Command(cellOwner, DrawingType.Empty, 0, 0);

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

        return new Command(cellOwner, DrawingType.Empty, 0, 0); // zaglushka
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
            if (_turn is TurnStatus.FirstPlayerPreparation or TurnStatus.SecondPlayerPreparation)
            {
                var currentPlayer = _turn == TurnStatus.FirstPlayerPreparation ? Player.First : Player.Second;

                if (_tables[currentPlayer].CheckThatPlacementIsCorrect() &&
                    _tables[currentPlayer].CheckThatShipsAraFine())
                {
                    if (currentPlayer == Player.First)
                    {
                        _turn = TurnStatus.SecondPlayerPreparation;
                        return new Command(currentPlayer, DrawingType.Hide, 0, 0);
                    }

                    _turn = TurnStatus.FirstGoes;
                    return new Command(currentPlayer, DrawingType.SwitchView, 0, 0);
                }

                MessageBox.Show("Расстановка не удовлетворяет правилам");

                return new Command(Player.First, DrawingType.Empty, 0, 0);
            }

        switch (_turn)
        {
            case TurnStatus.FirstPlayerPreparation or TurnStatus.SecondPlayerPreparation:
                return PlayerPreparationStep(cellOwner, row, column);

            case TurnStatus.FirstGoes:
                //Todo
                break;

            case TurnStatus.SecondGoes:
                //Todo
                break;
        }


        return new Command(Player.First, DrawingType.Empty, 0, 0); // zaglushka
    }
}