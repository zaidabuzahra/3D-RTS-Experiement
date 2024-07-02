using Signals;
using Signals.Core;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameState gameState;

    private void OnEnable()
    {
        CoreGameSignals.Instance.onChangeGameState += OnChangeGameState;
    }

    private void OnChangeGameState(GameState state)
    {
        if (gameState == state) return;
        CoreGameSignals.Instance.onStateLeave?.Invoke(gameState);
        gameState = state;
        CoreGameSignals.Instance.onStateEnter?.Invoke(gameState);
    }

    private void OnDisable()
    {
        if (CoreGameSignals.Instance == null) return;
        CoreGameSignals.Instance.onChangeGameState -= OnChangeGameState;
    }

    public void SwitchGameState(int state)
    {
        GameState tempState;
        if (state == 0) { tempState = GameState.Manage; }
        else { tempState = GameState.Build; }
        CoreGameSignals.Instance.onChangeGameState?.Invoke(tempState);
    }
}