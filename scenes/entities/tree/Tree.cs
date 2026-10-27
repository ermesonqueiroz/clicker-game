using Godot;
using Game.Components;
using Game.Common;
using Game.Globals;
using System;

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
	public HarvestBreakComponent harvestBreakComponent;

	[Export]
	public AnimatedSprite2D sprite;

	[Export]
	private Area2D _clickArea;

	public override void _Ready()
	{
		harvestClickComponent.Clicked += OnTreeClicked;
		harvestBreakComponent.Breaked += OnTreeBreaked;
		sprite.AnimationFinished += OnTreeAnimationFinished;
	}

    public void OnTreeClicked()
	{
		sprite.Play("click");
		harvestSpriteFeedbackComponent.Execute();
		SoundManager.Instance.PlayChoppingWoodSound();
		harvestBreakComponent.RegisterClick();
	}

	public void OnTreeBreaked()
	{
		sprite.Play("die");
		harvestClickComponent.Enabled = false;
		spawnEffectFeedbackComponent.Execute(GlobalPosition);
		spawnCollectibleFeedbackComponent.SpawnCollectible(GlobalPosition, 3);
	}

	private void OnTreeAnimationFinished()
	{
		if (sprite.Animation == "click")
		{
			sprite.Play("idle");
		}
	}
}
