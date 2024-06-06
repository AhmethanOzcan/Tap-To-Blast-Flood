using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileManager : Singleton<TileManager>
{
    public GameObject _tilePrefab;
    public GameObject _burstParticlePrefab;
    public GameObject _burstObstaclePrefab;
    [SerializeField] float _tileCreationPace;
    [SerializeField] float _clickCooldown = 1f;
    public Sprite[] _tileSprites;
    public Sprite[] _tileTNTSprites;
    public Material[] _burstMaterials;
    public Sprite[] _burstObstacleSprites;
    public Sprite _vaseBreakSprite;
    public Vector3[][] _gridPositions;
    public TileController[][] _tileControllers;
    public List<List<Vector2Int>> _sameTiles = new List<List<Vector2Int>>();
    [HideInInspector] public List<Transform> _spawnPoints= new List<Transform>();
    public Level _level{get; private set;}
    public bool _isStart{get; private set;}
    private bool _isClickable;
    GameplayUIController _gameplayUI;
    Transform _gridTransform;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        _isClickable = true;
        if(LevelManager.Instance._activeLevel > 0)
            _level = LevelManager.Instance._levelList[LevelManager.Instance._activeLevel - 1];
        LevelManager.Instance.OnLevelChanged += OnLevelChanged;
    }


    public void FillGrid(Transform gridTransform)
    {
        this._gridTransform = gridTransform;
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
                if(_tileControllers[_x][_y] == null)
                    continue;
                _tileControllers[_x][_y].ResetNeighbors();
            }
        }

        for(int _y = 0; _y < _level.grid_height; _y++)
        {
            for(int _x = 0; _x < _level.grid_width; _x++)
            {
                if(_tileControllers[_x][_y] == null)
                    continue;
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
        Vector2Int _clickedPos = _tileController._tile._coordinates;
        if((_tileController._tile._tileType != TileType.t && !_tileController._inGroup) || (int)_tileController._tile._tileType > 4)
        {
            _isClickable = true;
            return;
        }
        else if((int)_tileController._tile._tileType < 4)
        {
            // NORMAL BLOCK
            List<Vector2Int> _tilesToPop    = new List<Vector2Int>();

            // ADD ADDITIONAL DAMAGES
            ColleteralDamage(_tileController, _tilesToPop);

            PopEverythingInTheList(_tilesToPop, _sameTiles[_tileController._groupIndex].Count >= 5, _clickedPos);

        }
        else
        {
            //TNT
            List<Vector2Int> _tilesToPop    = new List<Vector2Int>();
            if(_tileController._inGroup)
            {
                foreach(Vector2Int _pos in _sameTiles[_tileController._groupIndex])
                {
                    if (_tileControllers[_pos.x][_pos.y] == null)
                        continue;
                    _tilesToPop.Add(_pos);
                }
            }

            // ADD TNT DAMAGES
            int _blastRadius    = _tileController._inGroup ? 7 : 5;
            for(int _y = _clickedPos.y - _blastRadius / 2; _y <= _clickedPos.y + _blastRadius / 2; _y++)
            {
                for(int _x = _clickedPos.x - _blastRadius / 2; _x <= _clickedPos.x + _blastRadius / 2; _x++)
                {
                    if(_x < 0 || _y < 0 || _x >= _level.grid_width || _y >= _level.grid_height || _tileControllers[_x][_y] == null || (_tileController._inGroup && _tileController._groupIndex == _tileControllers[_x][_y]._groupIndex))
                        continue;
                    
                    _tilesToPop.Add(new Vector2Int(_x, _y));
                }
            }
            
            PopEverythingInTheList(_tilesToPop, false, _clickedPos);
        }
        
        _gameplayUI.DecreaseMoves();
        ReFillSameTiles();
    }

    private void PopEverythingInTheList(List<Vector2Int> _tilesToPop, bool _generateTnt, Vector2Int _clickedPos)
    {
        _tilesToPop = _tilesToPop.OrderByDescending(v => v.y).ToList();
        foreach (Vector2Int _tileToPopPos in _tilesToPop)
        {
            if (_generateTnt && _clickedPos == _tileToPopPos)
            {
                _tileControllers[_tileToPopPos.x][_tileToPopPos.y].TransformToTNT();
            }
            else
            {
                if(_tileControllers[_tileToPopPos.x][_tileToPopPos.y] == null)
                    continue;
                TileType _tileToPopType = _tileControllers[_tileToPopPos.x][_tileToPopPos.y]._tile._tileType;
                if(_tileToPopType == TileType.v && _tileControllers[_tileToPopPos.x][_tileToPopPos.y]._spriteRenderer.sprite != _vaseBreakSprite)
                {
                    _tileControllers[_tileToPopPos.x][_tileToPopPos.y]._spriteRenderer.sprite = _vaseBreakSprite;
                }
                else
                {
                    _gameplayUI.DecreaseGoal(_tileToPopType);
                    BurstParticle(_tileToPopPos, _tileToPopType);
                    _tileControllers[_tileToPopPos.x][_tileToPopPos.y].Pop();
                    if (!BoxAbove(_tileToPopPos))
                    {
                        GenerateTileOnTop(_tileToPopPos.x);
                    }
                    CheckDestroyedBoxDown(_tileToPopPos, _tileToPopType);
                }
                
                
            }
        }
    }

    private void BurstParticle(Vector2Int _tileToPopPos, TileType _tileToPopType)
    {
        if((int)_tileToPopType < 4)
        {
            GameObject particle = Instantiate(_burstParticlePrefab, _tileControllers[_tileToPopPos.x][_tileToPopPos.y].transform.position, Quaternion.identity);

            // Change the material of the particle system
            ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
            renderer.material = _burstMaterials[(int)_tileToPopType];

            renderer.sortingOrder = 10;
        }
        else if((int)_tileToPopType > 4)
        {
            GameObject particle = Instantiate(_burstObstaclePrefab, _tileControllers[_tileToPopPos.x][_tileToPopPos.y].transform.position, Quaternion.identity);
            int _spriteStart    = ((int)_tileToPopType - 5)*3;

            ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
            ParticleSystem particleSystem = particle.GetComponent<ParticleSystem>();
            particleSystem.textureSheetAnimation.RemoveSprite(0);
            particleSystem.textureSheetAnimation.RemoveSprite(0);
            particleSystem.textureSheetAnimation.RemoveSprite(0);
            particleSystem.textureSheetAnimation.AddSprite(_burstObstacleSprites[_spriteStart]);
            particleSystem.textureSheetAnimation.AddSprite(_burstObstacleSprites[_spriteStart+1]);
            particleSystem.textureSheetAnimation.AddSprite(_burstObstacleSprites[_spriteStart+2]);
            renderer.sortingOrder = 10;
        }
    }

    private void ColleteralDamage(TileController _tileController, List<Vector2Int> _tilesToPop)
    {
        foreach (Vector2Int _pos in _sameTiles[_tileController._groupIndex])
        {
            if (_tileControllers[_pos.x][_pos.y] == null)
                continue;
            _tilesToPop.Add(_pos);

            if (_pos.x != 0 && _tileControllers[_pos.x - 1][_pos.y] != null && (_tileControllers[_pos.x - 1][_pos.y]._tile._tileType == TileType.bo || _tileControllers[_pos.x - 1][_pos.y]._tile._tileType == TileType.v) && !_tilesToPop.Contains(new Vector2Int(_pos.x - 1, _pos.y)))
                _tilesToPop.Add(new Vector2Int(_pos.x - 1, _pos.y));
            if (_pos.y != 0 && _tileControllers[_pos.x][_pos.y - 1] != null && (_tileControllers[_pos.x][_pos.y - 1]._tile._tileType == TileType.bo || _tileControllers[_pos.x][_pos.y - 1]._tile._tileType == TileType.v) && !_tilesToPop.Contains(new Vector2Int(_pos.x, _pos.y - 1)))
                _tilesToPop.Add(new Vector2Int(_pos.x, _pos.y - 1));
            if (_pos.x != _level.grid_width - 1 && _tileControllers[_pos.x + 1][_pos.y] != null && (_tileControllers[_pos.x + 1][_pos.y]._tile._tileType == TileType.bo || _tileControllers[_pos.x + 1][_pos.y]._tile._tileType == TileType.v) && !_tilesToPop.Contains(new Vector2Int(_pos.x + 1, _pos.y)))
                _tilesToPop.Add(new Vector2Int(_pos.x + 1, _pos.y));
            if (_pos.y != _level.grid_height - 1 && _tileControllers[_pos.x][_pos.y + 1] != null && (_tileControllers[_pos.x][_pos.y + 1]._tile._tileType == TileType.bo || _tileControllers[_pos.x][_pos.y + 1]._tile._tileType == TileType.v) && !_tilesToPop.Contains(new Vector2Int(_pos.x, _pos.y + 1)))
                _tilesToPop.Add(new Vector2Int(_pos.x, _pos.y + 1));
        }
    }

    private void CheckDestroyedBoxDown(Vector2Int _popMePos, TileType _popMeType)
    {
        if (_popMeType == TileType.bo || _popMeType == TileType.s)
        {
            List<Vector2Int> _emptyBoxes = BoxDown(_popMePos);
            for (int i = 0; i < _emptyBoxes.Count; i++)
            {
                for (int _yTemp = _emptyBoxes[i].y + 1; _yTemp < _level.grid_height; _yTemp++)
                {
                    if (_tileControllers[_popMePos.x][_yTemp] == null)
                        continue;
                    if (_tileControllers[_popMePos.x][_yTemp]._tile._tileType == TileType.bo || _tileControllers[_popMePos.x][_yTemp]._tile._tileType == TileType.s)
                        break;

                    _tileControllers[_popMePos.x][_yTemp].AlertToFall();
                }

                if (!BoxAbove(_popMePos))
                {
                    GenerateTileOnTop(_popMePos.x);
                }
            }
        }
    }

    private bool BoxAbove(Vector2Int _pos)
    {
        int _y = _pos.y + 1;
        while(_y < _level.grid_height)
        {
            if(_tileControllers[_pos.x][_y] == null || _tileControllers[_pos.x][_y]._tile._tileType == TileType.bo || _tileControllers[_pos.x][_y]._tile._tileType == TileType.s)
                return true;
            _y++;
        }
        return false;
    }

    private List<Vector2Int> BoxDown(Vector2Int _pos)
    {
        List<Vector2Int> _returnList = new List<Vector2Int>();
        int _y = _pos.y-1;
        while(_y >= 0)
        {
            if(_tileControllers[_pos.x][_y] == null)
                _returnList.Add(new Vector2Int(_pos.x, _y));
            _y--;
        }
        _returnList = _returnList.OrderByDescending(v => v.y).ToList();
        return _returnList;
    }

}
