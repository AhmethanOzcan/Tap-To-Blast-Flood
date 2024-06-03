using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] float _boardTweenTime = 0.5f;

    SpriteRenderer _spriteRenderer;
    int _width;
    int _height;
    float _tileSize;
    Vector2 _gridSize;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
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

        CreateSpawnPoints();
        TileManager.Instance.FillGrid(this.transform);
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
        _tileSize                       = TileManager.Instance._tilePrefab.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void HandleGridSize()
    {
        _gridSize = new Vector2(0.4f + _tileSize * _width, 0.4f + _tileSize * _height);
        _spriteRenderer.size = _gridSize;
    }


}
