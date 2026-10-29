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


public partial class Tree : Node2D, IPoolableObject
{
	[Export]
	public HurtBox hurtBox;

	[Export]
	public ClickFeedbackComponent clickFeedbackComponent;

	[Export]
	public SpawnEffectFeedbackComponent spawnEffectFeedbackComponent;

	[Export]
	public SpawnCollectibleFeedbackComponent spawnCollectibleFeedbackComponent;

	[Export]
	public HealthComponent healthComponent;

	[Export]
	public AnimatedSprite2D sprite;

	private readonly StateMachine<TreeState> _stateMachine = new();
	private ObjectSpawnerComponent _objectSpawner;
	private float _pendingDamage;

	public override void _Ready()
	{
		hurtBox.HitReceived += OnHitReceived;
		healthComponent.Died += () => _stateMachine.ChangeState(TreeState.Breaked);

		_stateMachine.AddState(TreeState.Idle, null, IdleEnterState);
		_stateMachine.AddState(TreeState.Clicked, null, ClickedEnterState);
		_stateMachine.AddState(TreeState.Breaked, null, BreakedEnterState);
		_stateMachine.ChangeState(TreeState.Idle);
	}

    public async void ClickedEnterState()
	{
		hurtBox.Enabled = false;
		var chopFx = GD.Load<PackedScene>("res://scenes/fx/chop/chop.tscn").Instantiate<Chop>();
		chopFx.GlobalPosition = GlobalPosition;
		GameManager.Instance.AddChild(chopFx);

		await ToSignal(chopFx, Chop.SignalName.Chopped);

		sprite.Play("click");
		GD.Print(sprite.Animation);
		clickFeedbackComponent.Execute();
		SoundManager.Instance.PlayChoppingWoodSound();
		healthComponent.TakeDamage(_pendingDamage);

		sprite.SelfModulate = new Color(2f, 2f, 2f, 1f);
		var tween = GetTree().CreateTween();
		tween.TweenProperty(
            sprite, 
            "self_modulate", 
            new Color(1f, 1f, 1f, 1f), 
            0.15
        );

		await ToSignal(sprite, AnimatedSprite2D.SignalName.AnimationFinished);
		if (_stateMachine.CurrentStateKey != TreeState.Breaked)
		{
			_stateMachine.ChangeState(TreeState.Idle);
		}
	}

	public void BreakedEnterState()
	{
		sprite.Play("die");
		hurtBox.Enabled = false;
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
		healthComponent.ResetHealth();
		hurtBox.Enabled = true;
		_stateMachine.ChangeState(TreeState.Idle);
	}

	public void OnPoolDespawned()
	{
		hurtBox.Enabled = false;
	}

	private void IdleEnterState()
	{
		hurtBox.Enabled = true;
		sprite.Play("idle");
	}

	private void OnHitReceived(float damage)
	{
		if (_stateMachine.CurrentStateKey != TreeState.Idle)
		{
			return;
		}

		_pendingDamage = damage;
		_stateMachine.ChangeState(TreeState.Clicked);
	}
}
