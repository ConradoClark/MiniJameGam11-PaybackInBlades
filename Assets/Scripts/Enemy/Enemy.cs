using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : TileObject
{
    protected override void OnAwake()
    {
        base.OnAwake();
        var currentPos = MapGenerator.Grid.WorldToCell(transform.position);
        transform.position = MapGenerator.GridToWorld(currentPos);
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(Act());
    }

    private IEnumerable<IEnumerable<Action>> Act()
    {
        while (isActiveAndEnabled)
        {
            while (GameStateManager.CurrentState != GameStateManager.GameState.Action)
            {
                yield return TimeYields.WaitOneFrameX;
            }

            yield return Wander().AsCoroutine();

            while (GameStateManager.CurrentState == GameStateManager.GameState.Action)
            {
                yield return TimeYields.WaitOneFrameX;
            }
        }
    }

    private IEnumerable<IEnumerable<Action>> Wander()
    {
        var currentPos = MapGenerator.Grid.WorldToCell(transform.position);

        var possibleDirections = new[]
        {
            IsWalkable((Vector2Int)currentPos + new Vector2Int(1, 0)) ? (Vector3Int.right, true) : (Vector3Int.right, false),
            IsWalkable((Vector2Int)currentPos + new Vector2Int(-1, 0)) ? (Vector3Int.left, true) : (Vector3Int.left, false),
            IsWalkable((Vector2Int)currentPos + new Vector2Int(0, 1)) ? (Vector3Int.up, true) : (Vector3Int.up, false),
            IsWalkable((Vector2Int)currentPos + new Vector2Int(0, -1)) ? (Vector3Int.down, true) : (Vector3Int.down, false),
        }.Where(dir => dir.Item2).ToArray();

        if (possibleDirections.Length == 0)
        {
            yield break;
        }

        var chosenDir = possibleDirections[Random.Range(0, possibleDirections.Length)];

        yield return transform.GetAccessor()
            .Position
            .ToPosition(MapGenerator.GridToWorld(currentPos + chosenDir.Item1))
            .Over(GameStateManager.ActionDuration)
            .UsingTimer(GameTimer)
            .BreakIf(() => !isActiveAndEnabled)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .WithStep(0.1f)
            .Build();
    }
}
