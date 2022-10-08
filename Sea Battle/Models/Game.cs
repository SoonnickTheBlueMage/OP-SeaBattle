using System.Collections.Generic;

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
    private Dictionary<Player, PersonalTable> _tables;

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
        if (_turn == TurnStatus.FirstPlayerPreparation)
        {
            //Todo
        }

        if (_turn == TurnStatus.SecondPlayerPreparation)
        {
            //Todo
        }

        if (_turn == TurnStatus.FirstGoes)
        {
        }

        if (_turn == TurnStatus.SecondGoes)
        {
        }

        return new Command(Player.First, DrawingType.Empty, 0, 0);
    }
}