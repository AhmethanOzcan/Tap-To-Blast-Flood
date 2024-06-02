using UnityEngine;


public class Tile
{
    public Vector2Int  _coordinates;
    public TileType _tileType;

    public Tile(Vector2Int _coordinates, TileType _tileType)
    {
        this._coordinates  = _coordinates;
        this._tileType  = _tileType;
    }
    
}