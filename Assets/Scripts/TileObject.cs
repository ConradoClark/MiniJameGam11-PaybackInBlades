using Licht.Unity.Objects;
using UnityEngine;

public class TileObject : BaseGameObject{
    protected MapGenerator MapGenerator;
    protected GameStateManager GameStateManager;

    protected override void OnAwake()
    {
        base.OnAwake();
        MapGenerator = SceneObject<MapGenerator>.Instance();
        GameStateManager = SceneObject<GameStateManager>.Instance();
    }

    public Vector2Int GetGridPosition()
    {
        return (Vector2Int)MapGenerator.Grid.WorldToCell(transform.position);
    }

    public bool IsWalkable(Vector2Int targetPos)
    {
        var gridPos = GetGridPosition();

        return MapGenerator.IsInBounds((Vector3Int)targetPos) &&
               Mathf.Abs(Mathf.Abs(gridPos.x) - Mathf.Abs(targetPos.x)) + Mathf.Abs(Mathf.Abs(gridPos.y) - Mathf.Abs(targetPos.y)) == 1;
    }
}
