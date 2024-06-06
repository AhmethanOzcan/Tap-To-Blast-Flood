using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIController : MonoBehaviour
{
    [SerializeField] Sprite _goalDoneTick;
    [SerializeField] PopUpUIController _popUpUI;
    LayoutGroup _goalGroup;
    TextMeshProUGUI[] _goals = new TextMeshProUGUI[3];
    TextMeshProUGUI _remainingMoves;
    

    private void Awake()
    {
        _goalGroup = GetComponentInChildren<LayoutGroup>();
        for(int i = 0; i < _goalGroup.transform.childCount; i++)
        {
            _goals[i] = _goalGroup.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
        }
        _remainingMoves = GameObject.Find("Move Count").GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        SetGoals();
        _remainingMoves.text    = TileManager.Instance._level.move_count.ToString();
    }

    private void SetGoals()
    {
        int[] _goalCount = {0, 0, 0};

        foreach(string _name in TileManager.Instance._level.grid)
        {
            switch(_name)
            {
                case "bo":
                    _goalCount[0]   = _goalCount[0] + 1;
                    break;
                case "s":
                    _goalCount[1]   = _goalCount[1] + 1;
                    break;
                case "v":
                    _goalCount[2]   = _goalCount[2] + 1;
                    break;
                default:
                    break;
            }
        }

        for(int i = 0; i < _goals.Length; i++)
        {
            if(_goalCount[i] == 0)
            {
                _goals[i].text      = "-1";
                _goals[i].transform.parent.gameObject.SetActive(false);
            }
            else
                _goals[i].text      = _goalCount[i].ToString();
        }

    }

    public void DecreaseMoves()
    {
        _remainingMoves.text    = (int.Parse(_remainingMoves.text)-1).ToString();
        CheckLevelFinish();
        
    }

    private void CheckLevelFinish()
    {
        if(_remainingMoves.text == "0")
        {
            if(CheckSucces())
                StartCoroutine(SuccesRoutine());
            else
                StartCoroutine(FailRoutine());
        }
    }

    public int GetMoves()
    {
        return int.Parse(_remainingMoves.text);
    }

    public void DecreaseGoal(TileType _tileType)
    {
        if((int)_tileType < 5)
            return;
        TextMeshProUGUI _goal = _goals[0];
        
        if(_tileType == TileType.bo)
        {
            _goal = _goals[0];
        }
        else if(_tileType == TileType.s)
        {
            _goal = _goals[1];
        }
        else if(_tileType == TileType.v)
        {
            _goal = _goals[2];
        }

        if(int.Parse(_goal.text) != 0)
        {
            _goal.text = (int.Parse(_goal.text)-1).ToString();
            if(_goal.text == "0")
            {
                _goal.text = "<sprite index=0>";
                _goal.ForceMeshUpdate();
                if(CheckSucces())
                {
                    StartCoroutine(SuccesRoutine());
                }
            }
            
        }
    }

    private bool CheckSucces()
    {
        foreach(TextMeshProUGUI _goal in _goals)
        {
            if(_goal.text != "-1" && _goal.text != "<sprite index=0>")
                return false;
        }

        return true;
    }

    private IEnumerator FailRoutine()
    {
        yield return null;
        Button[] _buttons = _popUpUI.GetComponentsInChildren<Button>();
        _buttons[0].onClick.AddListener(() =>GameManager.Instance.OpenLevelScene());
        _buttons[1].onClick.AddListener(() =>GameManager.Instance.OpenMainScene());
        TextMeshProUGUI[] _texts = _popUpUI.GetComponentsInChildren<TextMeshProUGUI>();
        _texts[0].text = "Retry Level!";
        _texts[1].text = "Level Failed!";
        _popUpUI.gameObject.SetActive(true);
    }

    private IEnumerator SuccesRoutine()
    {
        yield return null;
        LevelManager.Instance.NextLevel();
        Button[] _buttons = _popUpUI.GetComponentsInChildren<Button>();
        if(LevelManager.Instance._activeLevel != 0)
            _buttons[0].onClick.AddListener(() =>GameManager.Instance.OpenLevelScene());
        else
        {
            _buttons[0].onClick.AddListener(() =>GameManager.Instance.OpenMainScene());
            _buttons[0].GetComponentInChildren<TextMeshProUGUI>().text = "Finished!";
        }
        
        _buttons[1].onClick.AddListener(() =>GameManager.Instance.OpenMainScene());
        _popUpUI.gameObject.SetActive(true);
    }
}
