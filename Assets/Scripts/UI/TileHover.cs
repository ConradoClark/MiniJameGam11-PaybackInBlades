using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileHover : BaseGameObject
{
    public Camera Camera;
    public ScriptInput MousePosInput;

    private PlayerInput _playerInput;
    private GameStateManager _gameStateManager;
    private InputAction _mousePos;
    private MapGenerator _mapGenerator;

    public ScriptPrefab TileHoverObjectPrefab;
    private TileHoverObjectPool _tileHoverObjectPool;
    private TileHoverObject _currentTileHoverObject;

    protected override void OnAwake()
    {
        base.OnAwake();
        _playerInput = PlayerInput.GetPlayerByIndex(0);
        _gameStateManager = SceneObject<GameStateManager>.Instance();
        _mapGenerator = SceneObject<MapGenerator>.Instance();
        _mousePos = _playerInput.actions[MousePosInput.ActionName];
        _tileHoverObjectPool = SceneObject<TileHoverObjectManager>.Instance().GetEffect(TileHoverObjectPrefab);
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleHover());
    }

    private IEnumerable<IEnumerable<Action>> HandleHover()
    {
        while (isActiveAndEnabled)
        {
            if (_gameStateManager.CurrentState != GameStateManager.GameState.PlayerControl && _currentTileHoverObject != null)
            {
                _currentTileHoverObject.Hide();
            }

            while (_gameStateManager.CurrentState != GameStateManager.GameState.PlayerControl)
            {
                yield return TimeYields.WaitOneFrameX;
            }

            var worldMousePos = Camera.ScreenToWorldPoint(_mousePos.ReadValue<Vector2>());
            var gridPos = _mapGenerator.Grid.WorldToCell(worldMousePos);

            if (_mapGenerator.IsInBounds(gridPos))
            {
                if ((_currentTileHoverObject == null || !_currentTileHoverObject.IsActive) && _tileHoverObjectPool.TryGetFromPool(out var obj))
                {
                    _currentTileHoverObject = obj;
                }

                if (_currentTileHoverObject == null)
                {
                    yield return TimeYields.WaitOneFrameX;
                    continue;
                }

                _currentTileHoverObject.transform.position = _mapGenerator.GridToWorld(gridPos);
                _currentTileHoverObject.UpdateTile((Vector2Int) gridPos);
            }
            else if (_currentTileHoverObject != null && _currentTileHoverObject.IsActive)
            {
                _currentTileHoverObject.Hide();
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private static void DrawEllipse(Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, int segments, Color color, float duration = 0)
    {
        var angle = 0f;
        var rot = Quaternion.LookRotation(forward, up);
        var lastPoint = Vector3.zero;
        var thisPoint = Vector3.zero;

        for (var i = 0; i < segments + 1; i++)
        {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
            thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;

            if (i > 0)
            {
                UnityEngine.Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
            }

            lastPoint = thisPoint;
            angle += 360f / segments;
        }
    }
}
