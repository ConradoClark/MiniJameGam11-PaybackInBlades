using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public enum GameState
    {
        GameStart,
        PlayerControl,
        Action
    }

    public GameState CurrentState { get; private set; }
    public float ActionDuration = 0.5f;

    public void SetState(GameState gameState)
    {
        CurrentState = gameState;
    }
}
