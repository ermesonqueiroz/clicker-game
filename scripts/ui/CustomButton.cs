using Godot;

namespace Game.UI;

[GlobalClass]
public partial class CustomButton : Control
{
	public override void _Ready()
	{
		MouseDefaultCursorShape = (CursorShape)Input.CursorShape.PointingHand;
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}

	public void	OnMouseEntered()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "modulate", new Color(0.6f, 0.6f, 0.6f), 0.1f);
	}

	public void OnMouseExited()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "modulate", new Color(1f, 1f, 1f), 0.1f);
	}
}
