using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

public class GameStart : BaseGameObject
{
    private GameStateManager _gameStateManager;
    protected override void OnAwake()
    {
        base.OnAwake();
        _gameStateManager = SceneObject<GameStateManager>.Instance();
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(Start());
    }

    private IEnumerable<IEnumerable<Action>> Start()
    {
        _gameStateManager.SetState(GameStateManager.GameState.PlayerControl);
        yield break;
    }
}
