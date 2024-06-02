using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    TextMeshProUGUI _startButtonText;

    private void Awake() {
        GameObject _startButton = GameObject.Find("Start Button");
        _startButtonText        = _startButton.GetComponentInChildren<TextMeshProUGUI>();
    }
    void Start()
    {
        ChangeStartButtonText();
    }

    private void ChangeStartButtonText()
    {
        int _currentLevel = LevelManager.Instance._activeLevel;
        if(_currentLevel > 0)
            _startButtonText.text   = "Level "+ _currentLevel;
        else
            _startButtonText.text   = "Finished";
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
