using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;
    public Tile _tile;

    
    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Tile _tile)
    {
        this._tile = _tile;
        name = string.Format("T:{0},{1}", _tile._coordinates.x.ToString(),  _tile._coordinates.y.ToString());
        this._spriteRenderer.sortingOrder = _tile._coordinates.y + 1;
        this._spriteRenderer.sprite = TileManager.Instance._tileSprites[(int)_tile._tileType];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown() {
        TileManager.Instance.TileClicked(this);
        Debug.Log("Clicked on tile at "+ _tile._coordinates.ToString());
    }
}
