using UnityEngine;


public class Tile
{
    public Vector2Int  _position;
    public TileType _tileType;

    public Tile(Vector2Int _position, TileType _tileType)
    {
        this._position  = _position;
        this._tileType  = _tileType;
    }
    
}