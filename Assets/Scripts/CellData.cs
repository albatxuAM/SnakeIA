using UnityEngine;

public struct CellData
{
    public CellType cellType;
    public GameObject gameObject;

    // Constructor
    public CellData(CellType cellType, GameObject gameObject)
    {
        this.cellType = cellType;
        this.gameObject = gameObject;
    }
}
