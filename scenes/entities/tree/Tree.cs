using Godot;
using Game.Components;
using Game.Common;
using Game.Globals;
using System;

namespace Game.Entities;

enum TreeState
{
	Idle,
	Clicked,
	Breaked
}


public partial class Tree : Area2D, IPoolableObject
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

	private readonly StateMachine _stateMachine = new();
	private ObjectSpawnerComponent _objectSpawner;

	public override void _Ready()
	{
		harvestClickComponent.Clicked += () => _stateMachine.ChangeState((int)TreeState.Clicked);
		harvestBreakComponent.Breaked += () => _stateMachine.ChangeState((int)TreeState.Breaked);
		sprite.AnimationFinished += OnTreeAnimationFinished;

		_stateMachine.AddState((int)TreeState.Idle, null, IdleEnterState);
		_stateMachine.AddState((int)TreeState.Clicked, null, ClickedEnterState);
		_stateMachine.AddState((int)TreeState.Breaked, null, BreakedEnterState);
		_stateMachine.ChangeState((int)TreeState.Idle);
	}

    public void ClickedEnterState()
	{
		sprite.Play("click");
		harvestSpriteFeedbackComponent.Execute();
		SoundManager.Instance.PlayChoppingWoodSound();
		harvestBreakComponent.RegisterClick();

		if (_stateMachine.CurrentStateKey != (int)TreeState.Breaked)
		{
			_stateMachine.ChangeState((int)TreeState.Idle);
		}
	}

	public void BreakedEnterState()
	{
		sprite.Play("die");
		harvestClickComponent.Enabled = false;
		spawnEffectFeedbackComponent.Execute(GlobalPosition);
		spawnCollectibleFeedbackComponent.SpawnCollectible(GlobalPosition, 3);

		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "modulate:a", 0.0f, 1.0f).SetDelay(3.0f);
		tween.Chain().TweenCallback(Callable.From(() =>
		{
			if (_objectSpawner != null)
			{
				_objectSpawner.DespawnObject(this);
			}
			else
			{
				QueueFree();
			}
		}));
	}

	public void OnPoolSpawned(ObjectSpawnerComponent spawner)
	{
		_objectSpawner = spawner;
		Modulate = Colors.White;
		harvestBreakComponent.Reset();
		harvestClickComponent.Enabled = true;
		_stateMachine.ChangeState((int)TreeState.Idle);
	}

	public void OnPoolDespawned()
	{
		harvestClickComponent.Enabled = false;
	}

	private void OnTreeAnimationFinished()
	{
		if (sprite.Animation == "click")
		{
			sprite.Play("idle");
		}
	}

	private void IdleEnterState()
	{
		sprite.Play("idle");
	}
}
