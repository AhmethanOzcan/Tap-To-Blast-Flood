using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] float _fallDownSpeed = 2f;

    SpriteRenderer _spriteRenderer;
    public Tile _tile;

    
    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Tile _tile)
    {
        this._tile = _tile;
        name = string.Format("T:{0},{1}", _tile._coordinates.x.ToString(),  _tile._coordinates.y.ToString());
        this._spriteRenderer.sprite = TileManager.Instance._tileSprites[(int)_tile._tileType];
        FallDown(this._tile._coordinates.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FallDown(int _y)
    {
        int _x = this._tile._coordinates.x;
        float _targetY = TileManager.Instance._gridPositions[_x][_y].y;
        float _time = Math.Abs(_targetY - this.transform.position.y) / _fallDownSpeed;
        this.transform.LeanMoveLocalY(_targetY, _time).setEaseOutSine();
        this._tile._coordinates = new Vector2Int(_x, _y);
        this._spriteRenderer.sortingOrder   = _y + 1;
        TileManager.Instance._tileControllers[_x][_y] = this;
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
            TileManager.Instance._tileControllers[_x][_y].AlertToFall();
        }
        Destroy(this.gameObject);
    }
}
