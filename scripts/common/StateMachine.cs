using Godot;

namespace Game.Common;

public interface IState
{
    void Enter();
    void Update(double delta);
    void PhysicsUpdate(double delta);
    void Exit();
}

public class StateMachine
{
    private IState _currentState;
    private readonly System.Collections.Generic.Dictionary<string, IState> _states = new();

    public void AddState(string name, IState state)
    {
        _states[name] = state;
    }

    public void ChangeState(string name)
    {
        if (!_states.ContainsKey(name))
        {
            GD.PrintErr($"StateMachine: State '{name}' not found!");
            return;
        }

        _currentState?.Exit();
        _currentState = _states[name];
        _currentState.Enter();
    }

    public bool IsCurrentState(string name)
    {
        return _states.TryGetValue(name, out var state) && ReferenceEquals(_currentState, state);
    }

    public void Update(double delta)
    {
        _currentState?.Update(delta);
    }

    public void PhysicsUpdate(double delta)
    {
        _currentState?.PhysicsUpdate(delta);
    }
}