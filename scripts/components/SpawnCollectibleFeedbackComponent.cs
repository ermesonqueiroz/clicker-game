using Godot;
using Game.Globals;

namespace Game.Components;

[GlobalClass]
public partial class SpawnCollectibleFeedbackComponent : Node
{
	[Export]
	public Texture2D texture;

	public void SpawnCollectible(Vector2 position, int amount = 1)
	{
		var viewport = GetViewport();
		var camera = viewport.GetCamera2D();

		for (int i = 0; i < amount; i++)
		{
			float deg = (float)GD.RandRange(0.0f, Mathf.Tau);
			Vector2 offset = Vector2.FromAngle(deg) * (float)GD.RandRange(30.0f, 80.0f);

			TextureRect collectibleRect = new()
			{
				Texture = texture,
				GlobalPosition = position - camera.GetScreenCenterPosition() + (viewport.GetVisibleRect().Size / 2),
				Scale = Vector2.Zero
			};

			GameManager.Instance.AddUiChild(collectibleRect);
			Tween tween = GetTree().CreateTween().SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);

			tween.TweenProperty(collectibleRect, "scale", Vector2.One, 0.5f);
			tween.Parallel().TweenProperty(collectibleRect, "global_position", collectibleRect.GlobalPosition + offset, 0.5f);

			var randomDelay = (float)GD.RandRange(0.0f, 0.5f);

			tween.Chain().TweenProperty(collectibleRect, "global_position", GameManager.Instance.woodLabel.GlobalPosition, 1.0f).SetDelay(randomDelay);
			tween.Parallel().TweenProperty(collectibleRect, "modulate:a", 0.0f, 0.1f).SetDelay(0.9f + randomDelay).SetTrans(Tween.TransitionType.Linear);
			tween.Chain().TweenCallback(Callable.From(() => {
				SoundManager.Instance.PlayPopSound();
				collectibleRect.QueueFree();
			}));
		}
    }
}
