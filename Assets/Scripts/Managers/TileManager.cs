using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileManager : Singleton<TileManager>
{
    public GameObject _tilePrefab;
    [SerializeField] float _tileCreationPace;
    [SerializeField] float _clickCooldown = 1f;
    public Sprite[] _tileSprites;
    public Sprite[] _tileTNTSprites;
    public Vector3[][] _gridPositions;
    public TileController[][] _tileControllers;
    public List<List<Vector2Int>> _sameTiles = new List<List<Vector2Int>>();
    [HideInInspector] public List<Transform> _spawnPoints= new List<Transform>();
    public Level _level{get; private set;}
    public bool _isStart{get; private set;}
    private bool _isClickable;
    GameplayUIController _gameplayUI;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        _isClickable = true;
        _level = LevelManager.Instance._levelList[LevelManager.Instance._activeLevel - 1];
        LevelManager.Instance.OnLevelChanged += OnLevelChanged;
    }


    public void FillGrid(Transform _gridTransform)
    {
        _isStart = true;
        _gameplayUI = FindObjectOfType<GameplayUIController>();
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
        ReFillSameTiles();
        _isStart = false;
    }

    private void GenerateTileOnTop(int _x)
    {
        GenerateTile(StringToTileType("rand"), _x, _level.grid_height-1);
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

    private void ReFillSameTiles()
    {
        _sameTiles.Clear();
        for(int _y = 0; _y < _level.grid_height; _y++)
        {
            for(int _x = 0; _x < _level.grid_width; _x++)
            {
                _tileControllers[_x][_y].ResetNeighbors();
            }
        }

        for(int _y = 0; _y < _level.grid_height; _y++)
        {
            for(int _x = 0; _x < _level.grid_width; _x++)
            {
                TileController _tileC = _tileControllers[_x][_y];
                if(!_tileC._inGroup && (int)_tileC._tile._tileType < 5)
                {
                    _tileC.CheckNeighbors();
                }
                if(_tileC._inGroup && !_tileC._canTNT && (int)_tileC._tile._tileType < 4 && _sameTiles[_tileC._groupIndex].Count >= 5)
                {
                    foreach (Vector2Int _pos in _sameTiles[_tileC._groupIndex])
                    {
                        _tileControllers[_pos.x][_pos.y].SetCanTNT(true);
                    }
                }
            }
        }

        StartCoroutine(ClickableAgainRoutine());
    }

    private IEnumerator ClickableAgainRoutine()
    {
        yield return new WaitForSeconds(_clickCooldown);
        _isClickable = true;
    }

    public void TileClicked(TileController _tileController)
    {
        if(!_isClickable || _gameplayUI.GetMoves() == 0)
            return;
        _isClickable = false;
        Vector2Int _coordinates = _tileController._tile._coordinates;
        if((_tileController._tile._tileType != TileType.t && !_tileController._inGroup) || (int)_tileController._tile._tileType > 4)
        {
            _isClickable = true;
            return;
        }
        else if((int)_tileController._tile._tileType < 4)
        {
            // NORMAL BLOCK
            int _index = _tileController._groupIndex;
            List<Vector2Int> _tilesToPop    = new List<Vector2Int>();
            foreach (Vector2Int _pos in _sameTiles[_tileController._groupIndex])
            {
                _tilesToPop.Add(_pos);
                
                if(_pos.x != 0 && (_tileControllers[_pos.x-1][_pos.y]._tile._tileType == TileType.bo || _tileControllers[_pos.x-1][_pos.y]._tile._tileType == TileType.v))
                    _tilesToPop.Add(new Vector2Int(_pos.x-1, _pos.y));
                if(_pos.y != 0 && (_tileControllers[_pos.x][_pos.y-1]._tile._tileType == TileType.bo || _tileControllers[_pos.x][_pos.y-1]._tile._tileType == TileType.v))
                    _tilesToPop.Add(new Vector2Int(_pos.x, _pos.y-1));
                if(_pos.x != _level.grid_width - 1 && (_tileControllers[_pos.x+1][_pos.y]._tile._tileType == TileType.bo || _tileControllers[_pos.x+1][_pos.y]._tile._tileType == TileType.v))
                    _tilesToPop.Add(new Vector2Int(_pos.x+1, _pos.y));
                if(_pos.y != _level.grid_height - 1 && (_tileControllers[_pos.x][_pos.y+1]._tile._tileType == TileType.bo || _tileControllers[_pos.x][_pos.y+1]._tile._tileType == TileType.v))
                    _tilesToPop.Add(new Vector2Int(_pos.x, _pos.y+1));
            }
            _tilesToPop = _tilesToPop.OrderByDescending(v => v.y).ToList();
            if(_tilesToPop.Count < 5)
            {
                // NO TNT GENERATION
                foreach(Vector2Int _tileToPopPos in _tilesToPop)
                {
                    _tileControllers[_tileToPopPos.x][_tileToPopPos.y].Pop();
                    GenerateTileOnTop(_tileToPopPos.x);
                }
            }
            else
            {
                // TNT GENERATION
                foreach(Vector2Int _tileToPopPos in _tilesToPop)
                {
                    if(_tileController._tile._coordinates == _tileToPopPos)
                    {
                        _tileControllers[_tileToPopPos.x][_tileToPopPos.y].TransformToTNT();
                    }
                    else
                    {
                        _tileControllers[_tileToPopPos.x][_tileToPopPos.y].Pop();
                        GenerateTileOnTop(_tileToPopPos.x);
                    }
                }
            }
            
        }
        else
        {
            //TNT
            int _blastRadius    = _tileController._inGroup ? 7 : 5;
            List<TileController> _tilesToPop = new List<TileController>();
            int _xClicked       = _tileController._tile._coordinates.x;
            int _yClicked       = _tileController._tile._coordinates.y;
            if(_tileController._inGroup)
            {
                foreach(Vector2Int _groupTile in _sameTiles[_tileController._groupIndex])
                {
                    _tilesToPop.Add(_tileControllers[_groupTile.x][_groupTile.y]);
                }
            }

            for(int _y = _yClicked - _blastRadius / 2; _y <= _yClicked + _blastRadius / 2; _y++)
            {
                for(int _x = _xClicked - _blastRadius / 2; _x <= _xClicked + _blastRadius / 2; _x++)
                {
                    if(_x < 0 || _y < 0 || _x >= _level.grid_width || _y >= _level.grid_height || (_tileController._inGroup && _tileController._groupIndex == _tileControllers[_x][_y]._groupIndex))
                        continue;
                    
                    _tilesToPop.Add(_tileControllers[_x][_y]);
                }
            }
            _tilesToPop = _tilesToPop.OrderByDescending(v => v._tile._coordinates.y).ToList();
            foreach(TileController _popMe in _tilesToPop)
            {
                _popMe.Pop();
                GenerateTileOnTop(_popMe._tile._coordinates.x);
            }
        }
        
        _gameplayUI.DecreaseMoves();
        ReFillSameTiles();
    }

}
