using System;
using System.Collections.Generic;
using Godot;

namespace Game.Common;

public class StateMachine<T>
{
    private readonly Dictionary<T, Dictionary<string, Action>> _states = [];
    
    public T CurrentStateKey { get; private set; }

    public void AddState(T key, Action onUpdate = null, Action onEnter = null, Action onExit = null)
    {
        _states[key] = new Dictionary<string, Action>
        {
            { "onUpdate", onUpdate },
            { "onEnter", onEnter },
            { "onExit", onExit }
        };
    }

    public void ChangeState(T key)
    {
        if (!_states.ContainsKey(key))
        {
            GD.PrintErr($"Not found '{key}' state!");
            return;
        }

        Dictionary<string, Action> _currentState = _states[key];
        _currentState.TryGetValue("onExit", out Action exitAction);

        if (CurrentStateKey != null)
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
        if (CurrentStateKey != null)
        {
            _states[CurrentStateKey].TryGetValue("onUpdate", out Action updateAction);
            updateAction?.Invoke();
        }
    }
}