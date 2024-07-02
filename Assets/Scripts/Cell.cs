using UnityEngine;

public struct Cell
{
    public bool Occupied { get; private set; }
    public CellType Type { get; private set; }
    public Cell(bool occupied, CellType type)
    {
        Occupied = occupied;
        Type = type;
    }

    public void ChangeOccupation(bool state)
    {
        Occupied = state;
    }

    public void ChangeType(CellType type)
    {
        Type = type;
    }
}

public enum CellType
{
    water,
    ground,
    mountain
}