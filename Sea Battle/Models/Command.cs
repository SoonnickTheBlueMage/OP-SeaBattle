namespace Sea_Battle.Models;

public enum DrawingType
{
    Empty,
    DrawHit,
    DrawMiss,
    DrawShipPart,
    EraseShipPart
}

public record Command(Player BoardOwner, DrawingType Draw, int Row, int Column)
{
}