using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapTile : EffectPoolable
{
    public SpriteRenderer Sprite;


    public void PickSprite(MapTileType tileType)
    {
        Sprite.sprite = tileType.Variations[Random.Range(0, tileType.Variations.Length)];
    }

    public IEnumerable<IEnumerable<Action>> ShowSprite()
    {
        Sprite.enabled = true;
        yield break;
    }

    public override void OnActivation()
    {
        Sprite.enabled = false;
    }

    public override bool IsEffectOver { get; protected set; }
}
