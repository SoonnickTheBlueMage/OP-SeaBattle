using System;
using System.Collections.Generic;

namespace Sea_Battle.Models;

public enum DrawingType
{
    Empty,
    DrawHit,
    DrawMiss,
    DrawShipPart,
    EraseShipPart,
    Hide,
    Show,
    DestroyShip,
    EndGame
}

public record Command(Player BoardOwner, DrawingType Draw, int Row = 0, int Column = 0,
    SortedSet<Tuple<int, int>>? PointsToMark = null, SortedSet<Tuple<int, int>>? PointsToDestroy = null)
{
}