using Godot;
using Game.Components;
using Game.Common;
using Game.Globals;
using Game.FX;

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
	public ObjectClickComponent objectClickComponent;

	[Export]
	public ClickFeedbackComponent clickFeedbackComponent;

	[Export]
	public SpawnEffectFeedbackComponent spawnEffectFeedbackComponent;

	[Export]
	public SpawnCollectibleFeedbackComponent spawnCollectibleFeedbackComponent;

	[Export]
	public ClickLimitComponent clickLimitComponent;

	[Export]
	public AnimatedSprite2D sprite;

	[Export]
	private Area2D _clickArea;

	private readonly StateMachine<TreeState> _stateMachine = new();
	private ObjectSpawnerComponent _objectSpawner;

	public override void _Ready()
	{
		objectClickComponent.Clicked += () => _stateMachine.ChangeState(TreeState.Clicked);
		clickLimitComponent.LimitReached += () => _stateMachine.ChangeState(TreeState.Breaked);
		sprite.AnimationFinished += OnTreeAnimationFinished;

		_stateMachine.AddState(TreeState.Idle, null, IdleEnterState);
		_stateMachine.AddState(TreeState.Clicked, null, ClickedEnterState);
		_stateMachine.AddState(TreeState.Breaked, null, BreakedEnterState);
		_stateMachine.ChangeState(TreeState.Idle);
	}

    public async void ClickedEnterState()
	{
		objectClickComponent.Enabled = false;
		var chopFx = GD.Load<PackedScene>("res://scenes/fx/chop/chop.tscn").Instantiate<Chop>();
		chopFx.GlobalPosition = GlobalPosition;
		GameManager.Instance.AddChild(chopFx);

		await ToSignal(chopFx, Chop.SignalName.Chopped);

		sprite.Play("click");
		clickFeedbackComponent.Execute();
		SoundManager.Instance.PlayChoppingWoodSound();
		clickLimitComponent.RegisterClick();

		sprite.SelfModulate = new Color(2f, 2f, 2f, 1f);
		var tween = GetTree().CreateTween();
		tween.TweenProperty(
            sprite, 
            "self_modulate", 
            new Color(1f, 1f, 1f, 1f), 
            0.15
        );

		if (_stateMachine.CurrentStateKey != TreeState.Breaked)
		{
			_stateMachine.ChangeState(TreeState.Idle);
		}
	}

	public void BreakedEnterState()
	{
		sprite.Play("die");
		objectClickComponent.Enabled = false;
		spawnEffectFeedbackComponent.Execute(GlobalPosition);
		spawnCollectibleFeedbackComponent.SpawnCollectible(GlobalPosition, 3);
		SoundManager.Instance.PlaWoodStackSound();

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
		clickLimitComponent.Reset();
		objectClickComponent.Enabled = true;
		_stateMachine.ChangeState(TreeState.Idle);
	}

	public void OnPoolDespawned()
	{
		objectClickComponent.Enabled = false;
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
		objectClickComponent.Enabled = true;
		sprite.Play("idle");
	}
}
