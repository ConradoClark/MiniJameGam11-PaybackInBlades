using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public class TileHoverObject : EffectPoolable
{
    public Sprite TileHoverRed;
    public Sprite TileBorderRed;

    public Sprite TileHoverGreen;
    public Sprite TileBorderGreen;

    public Sprite TileHoverBlue;
    public Sprite TileBorderBlue;

    public SpriteRenderer TileHoverBorder;
    public SpriteRenderer TileHoverImage;

    private Player _player;
    private Vector2Int _position;

    public override void OnActivation()
    {
        TileHoverImage.enabled = TileHoverBorder.enabled = true;
    }

    public void Hide()
    {
        TileHoverImage.enabled = TileHoverBorder.enabled = false;
        IsEffectOver = true;
    }

    public void UpdateTile(Vector2Int currentPos)
    {
        _position = currentPos;
        if (_player.GetGridPosition() == _position)
        {
            TileHoverImage.sprite = TileHoverBlue;
            TileHoverBorder.sprite = TileBorderBlue;
            return;
        }

        if (_player.IsWalkable(currentPos))
        {
            TileHoverImage.sprite = TileHoverGreen;
            TileHoverBorder.sprite = TileBorderGreen;
            return;
        }

        TileHoverImage.sprite = TileHoverRed;
        TileHoverBorder.sprite = TileBorderRed;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _player = SceneObject<Player>.Instance();
    }

    public override bool IsEffectOver { get; protected set; }
}
