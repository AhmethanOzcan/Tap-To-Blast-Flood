using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : Singleton<TileManager>
{
    [SerializeField] Sprite[] _tileSprites;
    public static List<TileController> _tileControllers = new List<TileController>();

    protected override void Awake()
    {
        base.Awake();
    }
}
