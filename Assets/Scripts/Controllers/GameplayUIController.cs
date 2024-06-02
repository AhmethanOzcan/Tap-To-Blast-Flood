using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIController : MonoBehaviour
{
    [SerializeField] Sprite _goalDoneTick;

    TextMeshProUGUI _boxGoal;
    TextMeshProUGUI _stoneGoal;
    TextMeshProUGUI _vaseGoal;
    TextMeshProUGUI _remainingMoves;


    private void Awake()
    {
        SetGoals();
    }

    private void SetGoals()
    {

        LayoutGroup _goalGroup = GetComponentInChildren<LayoutGroup>();
        for (int i = 0; i < _goalGroup.transform.childCount; i++)
        {
            Debug.Log(_goalGroup.transform.GetChild(i).gameObject.name);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
