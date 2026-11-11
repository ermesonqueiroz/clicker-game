using Godot;
using Game.Globals;
using System.Globalization;

namespace Game.Components;

[GlobalClass]
public partial class SpawnCollectibleFeedbackComponent : Node
{
	[Export]
	public Texture2D texture;

	[Export]
	private LabelSettings _labelSettings;

	public void SpawnCollectible(Vector2 position, int amount = 1)
	{
		if (amount <= 0)
		{
			throw new System.ArgumentOutOfRangeException(nameof(amount), "Collection amount must be greater than zero.");
		}

		var gameWorld = GameManager.Instance.GameWorld as Node2D;
		if (gameWorld == null)
		{
			throw new System.InvalidOperationException("GameWorld must be a Node2D to spawn collectible feedback.");
		}

		Vector2 spawnPosition = gameWorld.ToLocal(position);
		Node2D feedback = new()
		{
			Position = spawnPosition + new Vector2(
				(float)GD.RandRange(-12.0f, 12.0f),
				(float)GD.RandRange(-8.0f, 8.0f)
			),
			Scale = new Vector2(0.65f, 0.65f),
			ZIndex = 1
		};

		Sprite2D icon = new()
		{
			Texture = texture,
			Position = new Vector2(-30.0f, 0.0f)
		};
		Label amountLabel = new()
		{
			Text = $"+{amount.ToString(CultureInfo.InvariantCulture)}",
			LabelSettings = _labelSettings,
			Position = new Vector2(-4.0f, -18.0f),
			Size = new Vector2(70.0f, 36.0f),
			VerticalAlignment = VerticalAlignment.Center
		};

		feedback.AddChild(icon);
		feedback.AddChild(amountLabel);
		feedback.TreeEntered += () => AnimateCollectible(feedback);
		gameWorld.AddChild(feedback);
    }

	private void AnimateCollectible(Node2D feedback)
	{
		Vector2 targetPosition = feedback.Position + new Vector2(
			(float)GD.RandRange(-10.0f, 10.0f),
			-42.0f
		);

		Tween scaleTween = GetTree().CreateTween();
		scaleTween.TweenProperty(feedback, "scale", new Vector2(1.2f, 1.2f), 0.12f)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.Out);
		scaleTween.TweenProperty(feedback, "scale", Vector2.One, 0.1f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);

		Tween motionTween = GetTree().CreateTween();
		motionTween.TweenProperty(feedback, "position", targetPosition, 0.7f)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);

		Tween fadeTween = GetTree().CreateTween();
		fadeTween.TweenProperty(feedback, "modulate:a", 0.0f, 0.25f)
			.SetDelay(0.45f);
		fadeTween.TweenCallback(Callable.From(() =>
		{
			SoundManager.Instance.PlayPopSound();
			feedback.QueueFree();
		}));
	}
}
