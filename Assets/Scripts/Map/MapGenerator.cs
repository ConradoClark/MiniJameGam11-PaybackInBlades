using System;
using Licht.Unity.Objects;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using UnityEngine;

public class MapGenerator : BaseGameObject
{
    public Grid Grid;
    public MapTileType[] Tiles;
    public ScriptPrefab MapTilePrefab;
    public Vector2Int MapPosition;
    public Vector2Int MapSize;
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

    public bool IsInBounds(Vector3Int gridPos)
    {
        return new BoundsInt((Vector3Int)MapPosition, new Vector3Int(MapSize.x, MapSize.y, 1)).Contains(new Vector3Int(gridPos.x, gridPos.y));
    }

    private IEnumerable<IEnumerable<Action>> SpawnPlayer()
    {
        _player.Reset();
        _player.transform.position = GridToWorld((Vector3Int)MapPosition + new Vector3Int(PlayerSpawn.x, PlayerSpawn.y));

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
                    var obj = objs[row + column * 10];
                    obj.Component.transform.position =
                        GridToWorld(new Vector3Int(row + MapPosition.x, column + MapPosition.y));
                    obj.PickSprite(Tiles[0]);
                    yield return obj.ShowSprite().AsCoroutine();
                }
            }
        }
    }

    public Vector3 GridToWorld(Vector3Int pos)
    {
        var resultPos = Grid.GetCellCenterWorld(pos); //+ (Vector3)MapTileOffset + new Vector3(0, 0.5f);
        return new Vector3(resultPos.x, resultPos.y);
    }

}
