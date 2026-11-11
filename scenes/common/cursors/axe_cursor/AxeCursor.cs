using Godot;
using Game.Components;
using Game.Globals;
using Game.Resources;

namespace Game.Common.Cursors;

public partial class AxeCursor : Node2D
{
    [Export]
    private float _maxRadius = 100.0f;

    [Export]
    private Color _borderColor;

    [Export]
    private HitBoxComponent _hitBox;

    [Export]
    private StatsComponent _statsComponent;

    private Stat _radiusStat => _statsComponent.GetStat("idle_clicker_radius");

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        UpdateRadius();
    }

    public override void _Process(double _)
    {
        UpdateRadius();
        Visible = CursorManager.Instance.IdleCursorEnabled;

        if (!CursorManager.Instance.IdleCursorEnabled)
        {
            return;
        }

        GlobalPosition = GetGlobalMousePosition();
        // QueueRedraw();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!CursorManager.Instance.IdleCursorEnabled || !@event.IsActionPressed("harvest"))
        {
            return;
        }

        GlobalPosition = GetGlobalMousePosition();
        TriggerAreaAction();
        QueueRedraw();
    }

    private void UpdateRadius()
    {
        _maxRadius = _radiusStat.Value;
        _hitBox.SetRadius(_maxRadius);
    }

    public override void _Draw()    
    {
        // _DrawDashedCircle(Vector2.Zero, _maxRadius, _borderColor);
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
