using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Mixins;
using Licht.Unity.Objects;
using UnityEngine;

public class Player : TileObject
{
    public ScriptInput MousePosInput;
    public ScriptInput MouseClickInput;

    private ClickDetectionMixin _clickDetection;

    protected override void OnAwake()
    {
        base.OnAwake();
        _clickDetection = new ClickDetectionMixinBuilder(this, MousePosInput, MouseClickInput).Build();
    }

    public void Reset()
    {

    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleMovement());
    }

    private IEnumerable<IEnumerable<Action>> HandleMovement()
    {
        while (isActiveAndEnabled)
        {
            if (GameStateManager.CurrentState != GameStateManager.GameState.PlayerControl)
                yield return TimeYields.WaitOneFrameX;

            if (_clickDetection.WasClickedThisFrame(out var mousePos))
            {
                var gridPos = MapGenerator.Grid.WorldToCell(mousePos);
                if (IsWalkable((Vector2Int)gridPos))
                {
                    GameStateManager.SetState(GameStateManager.GameState.Action);

                    yield return transform.GetAccessor()
                        .Position
                        .ToPosition(MapGenerator.GridToWorld(gridPos))
                        .Over(GameStateManager.ActionDuration)
                        .UsingTimer(GameTimer)
                        .BreakIf(() => !isActiveAndEnabled)
                        .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
                        .WithStep(0.1f)
                        .Build();

                    GameStateManager.SetState(GameStateManager.GameState.PlayerControl);
                }
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
