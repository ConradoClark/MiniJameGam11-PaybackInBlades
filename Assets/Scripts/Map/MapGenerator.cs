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
    public Vector2Int MapSize;
    public Vector2 MapTileOffset;
    public Vector2Int PlayerSpawn;

    private MapTilePool _mapTilePool;
    private Player _player;

    protected override void OnAwake()
    {
        base.OnAwake();
        _mapTilePool = SceneObject<MapTileManager>.Instance().GetEffect(MapTilePrefab);
        _player = SceneObject<Player>.Instance();
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(
            GenerateMap().AsCoroutine()
                .Then(SpawnPlayer().AsCoroutine())
        );
    }

    private IEnumerable<IEnumerable<Action>> SpawnPlayer()
    {
        _player.Reset();
        _player.transform.position = Grid.CellToWorld(new Vector3Int(PlayerSpawn.x, PlayerSpawn.y) + (Vector3Int)MapBounds) - (Vector3)MapTileOffset;
        yield break;
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
                    obj.Component.transform.position = worldPos - (Vector3)MapTileOffset;
                    obj.PickSprite(Tiles[0]);
                    yield return obj.ShowSprite().AsCoroutine();
                }
            }
        }
    }
}
