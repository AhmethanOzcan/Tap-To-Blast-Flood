using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        for (int i = 0; i < _level.grid.Count; i++)
        {
            if (_column == _level.grid_width)
                _column = 0;
            
            GameObject _tile = Instantiate(_tilePrefab, _spawnPoints[_column]);
            yield return new WaitForSeconds(_tileCreationPace);
            _column++;
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


}
