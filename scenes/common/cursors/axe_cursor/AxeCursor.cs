using Godot;
using Game.Components;

namespace Game.Common.Cursors;

public partial class AxeCursor : Node2D
{
    [Export]
    private float _minRadius = 80.0f;
    
    [Export]
    private float _maxRadius = 100.0f;
    
    [Export]
    private float _contractSpeed = 300.0f;

    [Export]
    private Color _fillColor;

    [Export]
    private Color _borderColor;

    [Export]
    private HitBoxComponent _hitBox;

    [Export]
    private float _idleClickCooldown = 3.0f;

    private float _currentRadius = 100.0f;
    private bool _isContracting = true;
    private float _idleClickTimer = 0.0f;
    private bool _isPerformingIdleClick = false;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Hidden;
        _currentRadius = _maxRadius;
        _hitBox.SetRadius(_maxRadius);
    }

    public override void _Process(double delta)
    {
        float fDelta = (float)delta;

        GlobalPosition = GetGlobalMousePosition();

        if (!_isPerformingIdleClick)
        {
            _idleClickTimer += fDelta;
            if (_idleClickTimer >= _idleClickCooldown)
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
}
