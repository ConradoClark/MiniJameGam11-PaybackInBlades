using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

public class Player : BaseGameObject
{
    private MapGenerator _mapGenerator;
    private GameStateManager _gameStateManager;

    protected override void OnAwake()
    {
        base.OnAwake();
        _mapGenerator = SceneObject<MapGenerator>.Instance();
        _gameStateManager = SceneObject<GameStateManager>.Instance();
    }

    public void Reset()
    {

    }

    public Vector2Int GetGridPosition()
    {
        return (Vector2Int) _mapGenerator.Grid.WorldToCell(transform.position);
    }

    public bool IsWalkable(Vector2Int targetPos)
    {
        var gridPos = GetGridPosition();

        return Mathf.Abs(Mathf.Abs(gridPos.x) - Mathf.Abs(targetPos.x)) + Mathf.Abs(Mathf.Abs(gridPos.y) - Mathf.Abs(targetPos.y)) == 1;
    }

    private IEnumerable<IEnumerable<Action>> HandleMovement()
    {
        while (isActiveAndEnabled)
        {
            if (_gameStateManager.CurrentState != GameStateManager.GameState.PlayerControl)
                yield return TimeYields.WaitOneFrameX;
            

        }
    }
}
