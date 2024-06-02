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
    public static List<TileController> _tileControllers = new List<TileController>();
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
        StartCoroutine(TileCreationRoutine());
    }

    private IEnumerator TileCreationRoutine()
    {
        int _column = 0;
        int _row    = 0;
        for (int i = 0; i < _level.grid.Count; i++)
        {
            if (_column == _level.grid_width)
            {
               _column = 0;
               _row++;
               yield return new WaitForSeconds(_tileCreationPace);
            }

            GenerateTile(StringToTileType(_level.grid[i]), _row, _column);
            
            _column++;
        }
    }

    private void GenerateTile(TileType _type, int _row, int _column)
    {
        GameObject _tile = Instantiate(_tilePrefab, _spawnPoints[_column]);
        Tile _tileInfo = new Tile(new Vector2Int(_column, _row), _type);
        TileController _tileController =  _tile.GetComponent<TileController>();
        _tileController.Initialize(_tileInfo);
        _tileControllers.Add(_tileController);
    }

    private TileType StringToTileType(string _name)
    {
        switch(_name)
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
        int _column = _tileController._tile._coordinates.x;
        _tileControllers.Remove(_tileController);
        Destroy(_tileController.gameObject);
        GenerateTile(StringToTileType("rand"), _level.grid_height-1, _column);
    }
}
