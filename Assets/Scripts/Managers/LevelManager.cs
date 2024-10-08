using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public List<Level> _levelList = new List<Level>();
    public int _activeLevel{get; private set;}
    [SerializeField] string _folderPath = "Assets/Levels";
    public delegate void LevelChanged();
    public event LevelChanged OnLevelChanged;

    protected override void Awake() 
    {
        base.Awake();
        GenerateLevelList();
        LoadActiveLevel();
    }

    private void GenerateLevelList()
    {
        string[] _jsonFiles = Directory.GetFiles(_folderPath, "*.json");
        foreach (var _file in _jsonFiles)
        {
            string _jsonContent = File.ReadAllText(_file);
            _levelList.Add(Level.CreateFromJSON(_jsonContent));
        }

    }

    public void NextLevel()
    {
        SetActiveLevel(_activeLevel + 1);
        SaveActiveLevel();
    }

    private void LoadActiveLevel()
    {
        if (PlayerPrefs.HasKey("ActiveLevel"))
        {
            SetActiveLevel(PlayerPrefs.GetInt("ActiveLevel"));
        }
        else
        {
            SetActiveLevel(1);
        }
    }

    private void SaveActiveLevel()
    {
        PlayerPrefs.SetInt("ActiveLevel", _activeLevel);
        PlayerPrefs.Save();
    }

    private void SetActiveLevel(int _level)
    {
        this._activeLevel = _level;
        if(_activeLevel > _levelList.Count)
        {
            _activeLevel = 0;
        }
        else
        {
            OnLevelChanged?.Invoke();
        }

    }

}
