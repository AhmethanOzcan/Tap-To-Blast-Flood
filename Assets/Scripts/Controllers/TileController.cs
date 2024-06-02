using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;
    Vector2Int _coordinates;
    Tile _tile;

    
    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Initialize(Tile _tile)
    {
        this._tile = _tile;
        this._spriteRenderer.sprite = TileManager.Instance._tileSprites[(int)_tile._tileType];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown() {
        Debug.Log("Clicked");
    }
}
