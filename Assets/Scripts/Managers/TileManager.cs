using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileManager : Singleton<TileManager>
{
    public GameObject _tilePrefab;
    [SerializeField] float _tileCreationPace;

    public Sprite[] _tileSprites;
    public Vector3[][] _gridPositions;
    public TileController[][] _tileControllers;
    [HideInInspector] public List<Transform> _spawnPoints= new List<Transform>();
    public Level _level{get; private set;}
    public bool _isStart{get; private set;}

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        
        _level = LevelManager.Instance._levelList[LevelManager.Instance._activeLevel - 1];
        LevelManager.Instance.OnLevelChanged += OnLevelChanged;
    }


    public void FillGrid(Transform _gridTransform)
    {
        _isStart = true;
        _tileControllers = new TileController[_level.grid_width][];
        for (int i = 0; i < _level.grid_width; i++)
        {
            _tileControllers[i] = new TileController[_level.grid_height];
        }
        _gridPositions = new Vector3[_level.grid_width][];

        for (int i = 0; i < _level.grid_width; i++)
        {
            _gridPositions[i] = new Vector3[_level.grid_height];
        }

        FillGridTransforms(_gridTransform);
        StartCoroutine(TileCreationRoutine());
        
    }

    private void FillGridTransforms(Transform _gridTransform)
    {
        float _tileSize = this._tilePrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        float _yPoint   =  _gridTransform.position.y - _spawnPoints[0].position.y - _tileSize * _level.grid_height/2 + (_level.grid_height%2)*_tileSize/2 + 0.2f;
        for (int i = 0; i < _level.grid_height; i++)
        {
           for (int j = 0; j < _level.grid_width; j++)
           {
                float _xPoint           = _spawnPoints[j].position.x;
                _gridPositions[j][i]    = new Vector3(_xPoint, _yPoint);
           } 

           _yPoint += _tileSize;
        }
    }

    private IEnumerator TileCreationRoutine()
    {
        int _x = 0;
        int _y = 0;
        for (int i = 0; i < _level.grid.Count; i++)
        {
            if (_x == _level.grid_width)
            {
                _x = 0;
                _y++;
                yield return new WaitForSeconds(_tileCreationPace);
            }

            GenerateTile(StringToTileType(_level.grid[i]), _x, _y);

            _x++;
        }
        _isStart = false;
    }

    private void GenerateTile(TileType _type, int _x, int _y)
    {
        GameObject _tile = Instantiate(_tilePrefab, _spawnPoints[_x]);
        Tile _tileInfo = new Tile(new Vector2Int(_x, _y), _type);
        TileController _tileController = _tile.GetComponent<TileController>();
        _tileController.Initialize(_tileInfo);
        _tileControllers[_x][_y] = _tileController;
    }


    private TileType StringToTileType(string _color)
    {
        switch(_color)
        {
            case "r":
                return TileType.r;
            case "g":
                return TileType.g;
            case "b":
                return TileType.b;
            case "y":
                return TileType.y;
            case "t":
                return TileType.t;
            case "bo":
                return TileType.bo;
            case "s":
                return TileType.s;
            case "v":
                return TileType.v;
            case "rand":
                return (TileType)Random.Range(0, 4);
            default:
                return TileType.r;
        }
    }


    private void OnLevelChanged()
    {
        _level = LevelManager.Instance._levelList[LevelManager.Instance._activeLevel-1];
    }

    private void OnApplicationQuit() {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelChanged -= OnLevelChanged;
        }
    }

    private void OnDisable() 
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelChanged -= OnLevelChanged;
        }
    }

    public void TileClicked(TileController _tileController)
    {
        Vector2Int _coordinates = _tileController._tile._coordinates;
        _tileController.Pop();
        
        GenerateTile(StringToTileType("rand"), _coordinates.x, _level.grid_height-1);
    }
}
