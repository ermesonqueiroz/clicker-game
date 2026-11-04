using Godot;
using System;
using Game.Components;
using Game.Globals;
using Game.Resources;

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

    [Export]
    private StatsComponent _statsComponent;

    private Stat _radiusStat => _statsComponent.GetStat("idle_clicker_radius");

    private float _currentRadius = 100.0f;
    private bool _isContracting = true;
    private float _idleClickTimer = 0.0f;
    private bool _isPerformingIdleClick = false;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        UpdateRadius();
    }

    public override void _Process(double delta)
    {
        UpdateRadius();
        Visible = CursorManager.Instance.IdleCursorEnabled;

        if (!CursorManager.Instance.IdleCursorEnabled)
        {
            return;
        }

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

    private void UpdateRadius()
    {
        float minimumRadiusRatio = _minRadius / _maxRadius;
        _maxRadius = _radiusStat.Value;
        _minRadius = _maxRadius * minimumRadiusRatio;
        _hitBox.SetRadius(_maxRadius);
    }

    public override void _Draw()    
    {
        if (_idleClickTimer < _idleClickCooldown)
        {
            DrawArc(
                Vector2.Zero,
                _currentRadius - 8.0f,
                0.0f,
                Mathf.Lerp(0.0f, Mathf.Tau, _idleClickTimer / _idleClickCooldown),
                64,
                _fillColor,
                3.0f,
                true
            );
        }

        _DrawDashedCircle(Vector2.Zero, _currentRadius, _borderColor);
    }

    private async void TriggerAreaAction()
    {
        SoundManager.Instance.PlayPopSound();
        await _hitBox.Strike(GlobalPosition);
    }

    private void SetEnabled(bool enabled)
    {
        if (CursorManager.Instance.IdleCursorEnabled == enabled)
        {
            return;
        }

        CursorManager.Instance.IdleCursorEnabled = enabled;
        Visible = enabled;
        Input.MouseMode = enabled
            ? Input.MouseModeEnum.Hidden
            : Input.MouseModeEnum.Visible;

        if (!enabled)
        {
            _isPerformingIdleClick = false;
        }
    }

    private void _DrawDashedCircle(Vector2 position, float radius, Color color)
    {
        const int dashCount = 30;
        const float dashRatio = 0.5f;
        const float lineWidth = 3.0f;

        float totalAngle = Mathf.Tau;
        float stepAngle = totalAngle / dashCount;
        float dashAngle = stepAngle * dashRatio;

        for (int i = 0; i < dashCount; i++)
        {
            float startAngle = i * stepAngle;
            float endAngle = startAngle + dashAngle;
            DrawArc(position, radius, startAngle, endAngle, 2, color, lineWidth, true);
        }
    }
}
