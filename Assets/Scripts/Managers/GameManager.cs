using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{

    protected override void Awake()
    {
        base.Awake();
    }

    public void OpenLevelScene()
    {
        SceneManager.LoadScene(1);
    }

}
