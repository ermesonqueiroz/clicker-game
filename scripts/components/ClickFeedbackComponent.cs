using Game.Globals;
using Godot;

namespace Game.Components;

[GlobalClass]
public partial class ClickFeedbackComponent : Node
{
	[Export]
	public Node2D target;

	[Export]
	private PackedScene _clickEffectScene;

	public void Execute()
	{
		Tween tween = GetTree().CreateTween().SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(target, "scale:x", 0.9f, 0.1f).SetEase(Tween.EaseType.Out);
		tween.Parallel().TweenProperty(target, "scale:y", 1.1f, 0.1f).SetEase(Tween.EaseType.Out);
		tween.Chain().TweenProperty(target, "scale:x", 1f, 0.1f).SetEase(Tween.EaseType.In);
		tween.Parallel().TweenProperty(target, "scale:y", 1f, 0.1f).SetEase(Tween.EaseType.In);

		if (_clickEffectScene != null)
		{
			var clickFx = _clickEffectScene.Instantiate<Node2D>();
			Vector2 mouseWorldPosition = GetViewport().GetCanvasTransform().AffineInverse() * GetViewport().GetMousePosition();
			clickFx.Position = ((Node2D)GameManager.Instance.GameWorld).ToLocal(mouseWorldPosition);
			GameManager.Instance.AddChild(clickFx);
		}
	}
}
