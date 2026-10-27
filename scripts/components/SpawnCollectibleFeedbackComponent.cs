using Godot;
using Game.Globals;

namespace Game.Components;

[GlobalClass]
public partial class SpawnCollectibleFeedbackComponent : Node
{
	[Export]
	public Texture2D texture;

	public void Execute(Vector2 position)
	{
		var viewport = GetViewport();
		var camera = viewport.GetCamera2D();

		float deg = (float)GD.RandRange(0.0f, Mathf.Tau);
		Vector2 offset = Vector2.FromAngle(deg) * 50.0f;

        TextureRect rect = new()
        {
            Texture = texture,
            GlobalPosition = position + offset - camera.GetScreenCenterPosition() + (viewport.GetVisibleRect().Size / 2),
			Scale = Vector2.Zero
        };

		GameManager.Instance.AddUiChild(rect);
		Tween tween = GetTree().CreateTween().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
		tween.TweenProperty(rect, "scale", Vector2.One, 0.5f);
		tween.Chain().TweenProperty(rect, "global_position", GameManager.Instance.woodLabel.GlobalPosition, 1.0f);
		tween.Chain().TweenCallback(Callable.From(() => rect.QueueFree()));
    }
}
