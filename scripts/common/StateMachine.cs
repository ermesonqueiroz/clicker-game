using System;
using System.Collections.Generic;
using Godot;

namespace Game.Common;

public class StateMachine
{
    private readonly Dictionary<int, Dictionary<string, Action>> _states = [];
    
    public int? CurrentStateKey { get; private set; } = null;

    public void AddState(int key, Action onUpdate = null, Action onEnter = null, Action onExit = null)
    {
        _states[key] = new Dictionary<string, Action>
        {
            { "onUpdate", onUpdate },
            { "onEnter", onEnter },
            { "onExit", onExit }
        };
    }

    public void ChangeState(int key)
    {
        if (!_states.ContainsKey(key))
        {
            GD.PrintErr($"Not found '{key}' state!");
            return;
        }

        Dictionary<string, Action> _currentState = _states[key];
        _currentState.TryGetValue("onExit", out Action exitAction);

        if (CurrentStateKey.HasValue)
        {
            exitAction?.Invoke();
        }

        CurrentStateKey = key;
        _currentState = _states[key];
        _currentState.TryGetValue("onEnter", out Action enterAction);

        enterAction?.Invoke();
    }

    public void Update()
    {
        if (CurrentStateKey.HasValue)
        {
            _states[CurrentStateKey.Value].TryGetValue("onUpdate", out Action updateAction);
            updateAction?.Invoke();
        }
    }
}