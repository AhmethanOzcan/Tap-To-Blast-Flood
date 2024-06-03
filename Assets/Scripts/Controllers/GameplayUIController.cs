using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIController : MonoBehaviour
{
    [SerializeField] Sprite _goalDoneTick;
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
                _goals[i].transform.parent.gameObject.SetActive(false);
            }
            else
                _goals[i].text      = _goalCount[i].ToString();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
