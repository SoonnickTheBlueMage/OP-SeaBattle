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
    DestroyShip
}

public record Command(Player BoardOwner, DrawingType Draw, int Row = 0, int Column = 0,
    List<Tuple<int, int>>? PointsToMark = null, List<Tuple<int, int>>? PointsToDestroy = null)
{
}