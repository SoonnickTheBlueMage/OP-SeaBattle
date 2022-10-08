using System.Collections.Generic;

namespace Sea_Battle.Models;

public enum Player
{
    First,
    Second
}

public class Game
{
    private Player _turn;
    private Dictionary<Player, PersonalTable> _tables;

    public Game()
    {
        _turn = Player.First;
        _tables = new Dictionary<Player, PersonalTable>
        {
            {Player.First, new PersonalTable()},
            {Player.Second, new PersonalTable()}
        };
    }
    
    
}