using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] float _fallDownSpeed = 2f;
    [SerializeField] GameplayUIController _gameplayUI;

    public SpriteRenderer _spriteRenderer;
    public Tile _tile;
    public bool _inGroup;
    public int _groupIndex;
    public bool _canTNT;
    
    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Tile _tile)
    {
        this._tile                  = _tile;
        this._groupIndex            = -1;
        this._inGroup               = false;
        this._canTNT                = false;
        this._spriteRenderer.sprite = TileManager.Instance._tileSprites[(int)_tile._tileType];
        FallDown(this._tile._coordinates.y);
    }

    public void SetName()
    {
        name = string.Format("T:{0},{1}", _tile._coordinates.x.ToString(),  _tile._coordinates.y.ToString());
    }


    private void FallDown(int _y)
    {
        int _x = this._tile._coordinates.x;
        float _targetY = TileManager.Instance._gridPositions[_x][_y].y;
        float _time = Math.Abs(_targetY - this.transform.position.y) / _fallDownSpeed;
        this.transform.LeanMoveLocalY(_targetY, _time).setEaseOutSine();
        this._tile._coordinates = new Vector2Int(_x, _y);
        SetName();
        this._spriteRenderer.sortingOrder   = _y + 1;
        TileManager.Instance._tileControllers[_x][_y] = this;
    }

    public void CheckNeighbors()
    {
        List<TileController> _sameTypeNear = new List<TileController>();
        int _x = this._tile._coordinates.x;
        int _y = this._tile._coordinates.y;

        if(_y != 0 && this._tile._tileType == TileManager.Instance._tileControllers[_x][_y-1]._tile._tileType)
            _sameTypeNear.Add(TileManager.Instance._tileControllers[_x][_y-1]);
        if(_x != 0 && this._tile._tileType == TileManager.Instance._tileControllers[_x-1][_y]._tile._tileType)
            _sameTypeNear.Add(TileManager.Instance._tileControllers[_x-1][_y]);
        if(_x != TileManager.Instance._level.grid_width-1 && this._tile._tileType == TileManager.Instance._tileControllers[_x+1][_y]._tile._tileType)
            _sameTypeNear.Add(TileManager.Instance._tileControllers[_x+1][_y]);
        if(_y != TileManager.Instance._level.grid_height-1 && this._tile._tileType == TileManager.Instance._tileControllers[_x][_y+1]._tile._tileType)
            _sameTypeNear.Add(TileManager.Instance._tileControllers[_x][_y+1]);
        
        if(_sameTypeNear.Count == 0)
            return;

        foreach (TileController _nearTile in _sameTypeNear)
        {
            if(this._inGroup && _nearTile._inGroup && this._groupIndex == _nearTile._groupIndex)
            {
                continue;
            }
            else if(!this._inGroup && _nearTile._inGroup)
            {
                this._inGroup           = true;
                this._groupIndex        = _nearTile._groupIndex;
                TileManager.Instance._sameTiles[this._groupIndex].Add(this._tile._coordinates);
            }
            else if(this._inGroup && !_nearTile._inGroup)
            {
                _nearTile._inGroup      = true;
                _nearTile._groupIndex   = this._groupIndex;
                TileManager.Instance._sameTiles[this._groupIndex].Add(_nearTile._tile._coordinates);
                _nearTile.CheckNeighbors();
            }
            else if(!this._inGroup && !_nearTile._inGroup)
            {
                this._inGroup           = true;
                _nearTile._inGroup      = true;
                
                List<Vector2Int> _listOfCoordinates = new List<Vector2Int>(); 
                _listOfCoordinates.Add(this._tile._coordinates);
                _listOfCoordinates.Add(_nearTile._tile._coordinates);
                TileManager.Instance._sameTiles.Add(_listOfCoordinates);

                int _index = TileManager.Instance._sameTiles.Count-1;
                _nearTile._groupIndex   = _index;
                this._groupIndex        = _index;
                _nearTile.CheckNeighbors();
            }
        }
    }

    private void OnDestroy()
    {
        // _gameplayUI.OnTileDestroyed(_tile._tileType);
    }



    public void AlertToFall()
    {
        int _currentY = this._tile._coordinates.y;
        FallDown(_currentY-1);
    }


    private void OnMouseDown() {
        TileManager.Instance.TileClicked(this);
    }

    public void Pop()
    {
        int _x = this._tile._coordinates.x;
        int _y = this._tile._coordinates.y;
        TileManager.Instance._tileControllers[_x][_y] = null;
        while(_y < TileManager.Instance._level.grid_height-1)
        {
            _y++;
            TileController _tileToAlert = TileManager.Instance._tileControllers[_x][_y];
            if(_tileToAlert._tile._tileType == TileType.bo || _tileToAlert._tile._tileType == TileType.s)
                break;
            _tileToAlert.AlertToFall();
        }
        LeanTween.cancel(this.gameObject);
        Destroy(this.gameObject);
    }

    public void SetCanTNT(bool _value)
    {
        if(_value)
        {
            this._spriteRenderer.sprite = TileManager.Instance._tileTNTSprites[(int)this._tile._tileType];
            this._canTNT = true;
        }
        else
        {
            this._canTNT                = false;
            this._spriteRenderer.sprite = TileManager.Instance._tileSprites[(int)this._tile._tileType];
        }
    }

    public void ResetNeighbors()
    {
        this._groupIndex            = -1;
        this._inGroup               = false;
        SetCanTNT(false);
    }

    public void TransformToTNT()
    {
        this._tile._tileType        = TileType.t;
        this._spriteRenderer.sprite =  TileManager.Instance._tileSprites[(int)this._tile._tileType];
    }

}
