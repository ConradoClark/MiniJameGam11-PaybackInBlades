using System;
using Licht.Unity.Objects;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using UnityEngine;

public class MapGenerator : BaseGameObject
{
    public Grid Grid;
    public MapTileType[] Tiles;
    public ScriptPrefab MapTilePrefab;
    public Vector2Int MapBounds;
    public Vector2 MapTileOffset;

    private MapTilePool _mapTilePool;

    protected override void OnAwake()
    {
        base.OnAwake();
        _mapTilePool = SceneObject<MapTileManager>.Instance().GetEffect(MapTilePrefab);
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(GenerateMap());
    }

    private IEnumerable<IEnumerable<Action>> GenerateMap()
    {
        if (_mapTilePool.TryGetManyFromPool(100, out var objs))
        {

            for (var column = 0; column < 10; column++)
            {
                for (var row = 0; row < 10; row++)
                {
                    var worldPos = Grid.CellToWorld(new Vector3Int(row + MapBounds.x, column + MapBounds.y));
                    var obj = objs[row + column * 10];
                    obj.Component.transform.position = worldPos - (Vector3) MapTileOffset;
                    obj.PickSprite(Tiles[0]);
                    yield return obj.ShowSprite().AsCoroutine();
                }
            }
        }

        yield break;
    }
}
