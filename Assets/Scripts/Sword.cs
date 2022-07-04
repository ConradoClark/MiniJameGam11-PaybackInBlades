using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

public class Sword : BaseGameObject
{
    private GameStateManager _gameStateManager;
    protected override void OnAwake()
    {
        base.OnAwake();
        _gameStateManager = SceneObject<GameStateManager>.Instance();
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(Float());
    }

    private IEnumerable<IEnumerable<Action>> Float()
    {
        while (isActiveAndEnabled)
        {
            yield return transform.GetAccessor()
                .LocalPosition
                .Y
                .Increase(0.15f)
                .Over(0.75f)
                .WithStep(0.05f)
                .PauseIf(() => _gameStateManager.CurrentState == GameStateManager.GameState.Action)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(GameTimer)
                .Build();

            yield return transform.GetAccessor()
                .LocalPosition
                .Y
                .Decrease(0.15f)
                .Over(0.75f)
                .WithStep(0.05f)
                .PauseIf(() => _gameStateManager.CurrentState == GameStateManager.GameState.Action)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(GameTimer)
                .Build();
        }

    }
}
