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
    public TileController[][] _tileControllers;
    [HideInInspector] public List<Transform> _spawnPoints= new List<Transform>();
    Level _level;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        _level = LevelManager.Instance._levelList[LevelManager.Instance._activeLevel - 1];
        LevelManager.Instance.OnLevelChanged += OnLevelChanged;
    }


    public void FillGrid()
    {
        _tileControllers = new TileController[_level.grid_width][];
        for (int i = 0; i < _level.grid_width; i++)
        {
            _tileControllers[i] = new TileController[_level.grid_height];
        }
        StartCoroutine(TileCreationRoutine());
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
    }

    private void GenerateTile(TileType _type, int _x, int _y)
    {
        GameObject _tile = Instantiate(_tilePrefab, _spawnPoints[_x]);
        Tile _tileInfo = new Tile(new Vector2Int(_x, _y), _type);
        TileController _tileController = _tile.GetComponent<TileController>();
        _tileController.Initialize(_tileInfo);
        _tileControllers[_x][_y] = _tileController;
        Debug.Log(_tileControllers[_x][_y]);
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
        int _x = _coordinates.x;
        int _y = _coordinates.y;
        Destroy(_tileController.gameObject);
        _tileControllers[_x][_y] = null;
        GenerateTile(StringToTileType("rand"), _x, _y);
    }
}
