using Godot;

namespace Game.UI;

[GlobalClass]
public partial class CustomButton : Control
{
	[Signal]
	public delegate void PressedEventHandler();

	[Export]
	public bool Disabled = false;

	public override void _Ready()
	{
		MouseDefaultCursorShape = (CursorShape)Input.CursorShape.PointingHand;
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (Disabled)
		{
			return;
		}

		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			EmitSignal(SignalName.Pressed);
		}
	}

	public void	OnMouseEntered()
	{
		if (Disabled)
		{
			return;
		}

		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "modulate", new Color(0.6f, 0.6f, 0.6f), 0.1f);
	}

	public void OnMouseExited()
	{
		if (Disabled)
		{
			return;
		}

		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "modulate", new Color(1f, 1f, 1f), 0.1f);
	}

	public void Disable()
	{
		Disabled = true;
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "modulate:a", 0.5f, 0.1f);
		MouseDefaultCursorShape = (CursorShape)Input.CursorShape.Forbidden;
	}

	public void Enable()
	{
		Disabled = false;
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "modulate:a", 1.0f, 0.1f);
		MouseDefaultCursorShape = (CursorShape)Input.CursorShape.PointingHand;
	}
}
