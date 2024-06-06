using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    Button _startButton;
    TextMeshProUGUI _startButtonText;

    private void Awake() {
        _startButton         = this.GetComponentInChildren<Button>();
        _startButtonText     = _startButton.GetComponentInChildren<TextMeshProUGUI>();
    }
    void Start()
    {
        if(LevelManager.Instance._activeLevel > 0) 
            _startButton.onClick.AddListener(() =>GameManager.Instance.OpenLevelScene());
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
