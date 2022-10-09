namespace Sea_Battle.Models;

public enum DrawingType
{
    Empty,
    DrawHit,
    DrawMiss,
    DrawShipPart,
    EraseShipPart,
    Hide,
    Show
}

public record Command(Player BoardOwner, DrawingType Draw, int Row, int Column)
{
}