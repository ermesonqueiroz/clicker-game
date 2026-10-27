using Godot;
using Game.Components;

namespace Game.Entities;

public partial class Tree : Area2D
{
	[Export]
	public HarvestClickComponent harvestClickComponent;

	[Export]
	public HarvestSpriteFeedbackComponent harvestSpriteFeedbackComponent;

	[Export]
	public SpawnEffectFeedbackComponent spawnEffectFeedbackComponent;

	[Export]
	public SpawnCollectibleFeedbackComponent spawnCollectibleFeedbackComponent;

	[Export]
	public AnimatedSprite2D sprite;

	public override void _Ready()
	{
		harvestClickComponent.Clicked += OnTreeClicked;
	}

	public async void OnTreeClicked()
	{
		sprite.Play("click");
		sprite.AnimationFinished += () => sprite.Play("idle");

		harvestSpriteFeedbackComponent.Execute();
		spawnEffectFeedbackComponent.Execute(GlobalPosition);
		spawnCollectibleFeedbackComponent.Execute(GlobalPosition);

		QueueFree();
	}
}
