using Game.Globals;
using Godot;

namespace Game.Components;

[GlobalClass]
public partial class ClickFeedbackComponent : Node
{
	[Export]
	public Node2D target;

	private readonly PackedScene _clickDust = GD.Load<PackedScene>("res://scenes/fx/click_dust/click_dust.tscn");

	public void Execute()
	{
		Tween tween = GetTree().CreateTween().SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(target, "scale:x", 0.9f, 0.1f).SetEase(Tween.EaseType.Out);
		tween.Parallel().TweenProperty(target, "scale:y", 1.1f, 0.1f).SetEase(Tween.EaseType.Out);
		tween.Chain().TweenProperty(target, "scale:x", 1f, 0.1f).SetEase(Tween.EaseType.In);
		tween.Parallel().TweenProperty(target, "scale:y", 1f, 0.1f).SetEase(Tween.EaseType.In);

		var clickDustFX = _clickDust.Instantiate<Node2D>();
		Vector2 mouseWorldPosition = GetViewport().GetCanvasTransform().AffineInverse() * GetViewport().GetMousePosition();
		clickDustFX.Position = ((Node2D)GameManager.Instance.GameWorld).ToLocal(mouseWorldPosition);
		GameManager.Instance.AddChild(clickDustFX);
	}
}
