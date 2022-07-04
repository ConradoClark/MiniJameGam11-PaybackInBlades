using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Mixins;
using Licht.Unity.Objects;
using UnityEngine;

public class Player : BaseGameObject
{
    private MapGenerator _mapGenerator;
    private GameStateManager _gameStateManager;

    public ScriptInput MousePosInput;
    public ScriptInput MouseClickInput;

    private ClickDetectionMixin _clickDetection;

    protected override void OnAwake()
    {
        base.OnAwake();
        _mapGenerator = SceneObject<MapGenerator>.Instance();
        _gameStateManager = SceneObject<GameStateManager>.Instance();
        _clickDetection = new ClickDetectionMixinBuilder(this, MousePosInput, MouseClickInput).Build();
    }

    public void Reset()
    {

    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleMovement());
    }

    public Vector2Int GetGridPosition()
    {
        return (Vector2Int)_mapGenerator.Grid.WorldToCell(transform.position);
    }

    public bool IsWalkable(Vector2Int targetPos)
    {
        var gridPos = GetGridPosition();

        return _mapGenerator.IsInBounds((Vector3Int)targetPos) &&
               Mathf.Abs(Mathf.Abs(gridPos.x) - Mathf.Abs(targetPos.x)) + Mathf.Abs(Mathf.Abs(gridPos.y) - Mathf.Abs(targetPos.y)) == 1;
    }

    private IEnumerable<IEnumerable<Action>> HandleMovement()
    {
        while (isActiveAndEnabled)
        {
            if (_gameStateManager.CurrentState != GameStateManager.GameState.PlayerControl)
                yield return TimeYields.WaitOneFrameX;

            if (_clickDetection.WasClickedThisFrame(out var mousePos))
            {
                var gridPos = _mapGenerator.Grid.WorldToCell(mousePos);
                if (IsWalkable((Vector2Int)gridPos))
                {
                    _gameStateManager.SetState(GameStateManager.GameState.Action);

                    yield return transform.GetAccessor()
                        .Position
                        .ToPosition(_mapGenerator.GridToWorld(gridPos))
                        .Over(.5f)
                        .UsingTimer(GameTimer)
                        .BreakIf(() => !isActiveAndEnabled)
                        .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
                        .WithStep(0.1f)
                        .Build();

                    _gameStateManager.SetState(GameStateManager.GameState.PlayerControl);
                }
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
