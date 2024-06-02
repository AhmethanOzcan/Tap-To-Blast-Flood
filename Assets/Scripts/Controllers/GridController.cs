using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] float _boardTweenTime = 0.5f;
    [SerializeField] float _colliderPreventSquishValue = 0.01f;
    SpriteRenderer _spriteRenderer;
    EdgeCollider2D[] _borders;
    int _width;
    int _height;
    float _tileSize;


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _borders        = GetComponentsInChildren<EdgeCollider2D>();
    }

    private void Start() {
        StartCoroutine(TileCreationRoutine());
    }

    private IEnumerator TileCreationRoutine()
    {
        transform.localScale = Vector2.zero;
        SetSizes();
        HandleGridSize();
        transform.LeanScale(Vector2.one, _boardTweenTime).setEaseInOutBounce();
        yield return new WaitForSeconds(_boardTweenTime);
        HandleBorders();
        CreateSpawnPoints();
        TileManager.Instance.FillGrid();
    }

    private void CreateSpawnPoints()
    {
        Vector3 _screenPosition = new Vector3(Screen.width / 2, Screen.height, Camera.main.nearClipPlane);
        Vector3 _worldPosition = Camera.main.ScreenToWorldPoint(_screenPosition);
        _worldPosition.x -= (_width /2) * _tileSize;
        if(_width % 2 == 0) 
            _worldPosition.x += _tileSize/2f;
        TileManager.Instance._spawnPoints.Clear();
        for(int i = 0; i < _width; i++)
        {
            GameObject _spawnPoint = new GameObject(i.ToString());
            _spawnPoint.transform.position = _worldPosition;
            _spawnPoint.transform.parent = GameObject.Find("Spawn Points").transform;
            _worldPosition.x += _tileSize;
            TileManager.Instance._spawnPoints.Add(_spawnPoint.transform);
        }
    }

    private void SetSizes()
    {
        _width                          = LevelManager.Instance._levelList[LevelManager.Instance._activeLevel-1].grid_width;
        _height                         = LevelManager.Instance._levelList[LevelManager.Instance._activeLevel-1].grid_height;
        BoxCollider2D _prefabCollider   = TileManager.Instance._tilePrefab.GetComponent<BoxCollider2D>();
        _tileSize                       = _prefabCollider.size.x + _colliderPreventSquishValue;
    }

    private void HandleBorders()
    {
        Vector2 _borderSizeLeftV        = new Vector2(-(_tileSize-_colliderPreventSquishValue) * _height, 0);
        Vector2 _borderSizeRightV       = new Vector2((_tileSize-_colliderPreventSquishValue) * _height, 0);
        Vector2 _borderSizeLeftH        = new Vector2(-_tileSize * _width, 0);
        Vector2 _borderSizeRightH       = new Vector2(_tileSize * _width, 0);
        Vector2 _offsetV                = new Vector2(0, -_tileSize * _width);
        Vector2 _offsetH                = new Vector2(0, -(_tileSize-_colliderPreventSquishValue/2) * _height);
        foreach(EdgeCollider2D _border in _borders)
        {
            if(_border.gameObject.transform.localEulerAngles.z == 0 || _border.gameObject.transform.localEulerAngles.z == 180)
            {
                _border.points  = new Vector2[] { _borderSizeLeftH, _borderSizeRightH };
                _border.offset  = _offsetH;
            }
            else
            {
                _border.points  = new Vector2[] { _borderSizeLeftV, _borderSizeRightV };
                _border.offset  = _offsetV;
            }
            
        }
    }

    private void HandleGridSize()
    {
        Vector2 _gridSize = new Vector2(0.4f + _tileSize * _width, 0.4f + (_tileSize-_colliderPreventSquishValue/2) * _height);
        _spriteRenderer.size = _gridSize;
    }


}
