using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalTile : Tile
{
    [SerializeField] private Collider2D normalTileCollider;
    [SerializeField] private Color _painterColor;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GridManager _gridManager;

    bool isPainted = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (isPainted)
                player.Death();
            else
            {
                isPainted = true;
            }
        }
    }

    public void Paint()
    {
        isPainted = true;
    }


    void Update()
    {
        if (isPainted)
        {
            _spriteRenderer.color = _painterColor;
        }
    }
}
