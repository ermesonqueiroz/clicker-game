using Godot;
using Game.Components;
using Game.Resources.Stats;

namespace Game.Common.Cursors;

public partial class AxeCursor : Node2D
{
    
    [Export]
    private float _contractSpeed = 300.0f;

    [Export]
    private Color _fillColor;

    [Export]
    private Color _borderColor;

    [Export]
    private HitBoxComponent _hitBox;

    [Export]
    private RoundController _roundController;

    [Export]
    private StatsComponent _statsComponent;

    private float _minRadius;
    private float _maxRadius;
    private float _currentRadius = 100.0f;
    private bool _isContracting = true;
    private float _idleClickTimer = 0.0f;
    private bool _isPerformingIdleClick = false;
    private bool _isEnabled;

    private Stat _idleClickRadius;
    private Stat _idleClickCooldown;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        _idleClickRadius = _statsComponent.GetStatByID("idle_click_radius");
        _idleClickCooldown = _statsComponent.GetStatByID("idle_click_cooldown");
        UpdateRadiusFromStat();
        _currentRadius = _maxRadius;
        _roundController.RoundStarted += OnRoundStarted;
        _roundController.RoundEnded += OnRoundEnded;
        SetEnabled(_roundController.IsCollecting && !GetTree().Paused);
    }

    public override void _ExitTree()
    {
        _roundController.RoundStarted -= OnRoundStarted;
        _roundController.RoundEnded -= OnRoundEnded;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    public override void _Process(double delta)
    {
        UpdateRadiusFromStat();
        SetEnabled(_roundController.IsCollecting && !GetTree().Paused);
        if (!_isEnabled)
        {
            return;
        }

        float fDelta = (float)delta;

        GlobalPosition = GetGlobalMousePosition();

        if (!_isPerformingIdleClick)
        {
            _idleClickTimer += fDelta;
            if (_idleClickTimer >= _idleClickCooldown.Value)
            {
                _idleClickTimer = 0.0f;
                _isPerformingIdleClick = true;
                _isContracting = true;
            }
            
            _currentRadius = Mathf.MoveToward(_currentRadius, _maxRadius, _contractSpeed * fDelta);
        }
        else
        {
            if (_isContracting)
            {
                _currentRadius -= _contractSpeed * fDelta;
                if (_currentRadius <= _minRadius)
                {
                    _currentRadius = _minRadius;
                    _isContracting = false;

                    TriggerAreaAction();
                }
            }
            else
            {
                _currentRadius += _contractSpeed * fDelta;
                if (_currentRadius >= _maxRadius)
                {
                    _currentRadius = _maxRadius;
                    _isPerformingIdleClick = false;
                }
            }
        }

        QueueRedraw();
    }

    public override void _Draw()
    {        
        DrawCircle(Vector2.Zero, _currentRadius, _fillColor);
        DrawArc(Vector2.Zero, _currentRadius, 0, Mathf.Tau, 64, _borderColor, 2.0f);
    }

    private async void TriggerAreaAction()
    {
        await _hitBox.Strike(GlobalPosition);
    }

    private void OnRoundStarted(int roundNumber)
    {
        SetEnabled(!GetTree().Paused);
    }

    private void OnRoundEnded(int roundNumber)
    {
        SetEnabled(false);
    }

    private void SetEnabled(bool enabled)
    {
        if (_isEnabled == enabled)
        {
            return;
        }

        _isEnabled = enabled;
        Visible = enabled;
        Input.MouseMode = enabled
            ? Input.MouseModeEnum.Hidden
            : Input.MouseModeEnum.Visible;

        if (!enabled)
        {
            _isPerformingIdleClick = false;
        }
    }

    private void UpdateRadiusFromStat()
    {
        float radius = (float)_idleClickRadius.Value;
        if (_maxRadius == radius)
        {
            return;
        }

        _maxRadius = radius;
        _minRadius = _maxRadius * 0.8f;
        _hitBox.SetRadius(_maxRadius);
    }
}
