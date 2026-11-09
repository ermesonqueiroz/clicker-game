using Godot;
using Game.Components;
using Game.Common;
using Game.Globals;
using Game.FX;
using Game.Resources;

namespace Game.Entities;

public partial class Tree : Node2D, IPoolableObject
{
	private const string IdleStateName = "Idle";
	private const string ClickedStateName = "Clicked";
	private const string BreakedStateName = "Breaked";

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
	public DamageLabelSpawnerComponent _damageLabelSpawnerComponent;

	[Export]
	public AnimatedSprite2D sprite;

	[Export]
	public Marker2D MarkerLeft;

	[Export]
	public Marker2D MarkerRight;

	[Export]
    private StatsComponent _statsComponent;

    private Stat _treeCoinDropStat => _statsComponent.GetStat("tree_coin_drop");

	private readonly StateMachine _stateMachine = new();
	private ObjectSpawnerComponent _objectSpawner;
	private float _pendingDamage;

	public override void _Ready()
	{
		AddToGroup("trees");

		hurtBox.HitReceived += OnHitReceived;
		healthComponent.Died += () => _stateMachine.ChangeState(BreakedStateName);

		_stateMachine.AddState(IdleStateName, new IdleState(this));
		_stateMachine.AddState(ClickedStateName, new ClickedState(this));
		_stateMachine.AddState(BreakedStateName, new BreakedState(this));
		_stateMachine.ChangeState(IdleStateName);
	}

    private async void ClickedEnterState()
	{
		hurtBox.Enabled = false;
		// var chopFx = GD.Load<PackedScene>("res://scenes/fx/chop/chop.tscn").Instantiate<Chop>();
		// chopFx.GlobalPosition = GlobalPosition;
		// GameManager.Instance.AddChild(chopFx);

		// await ToSignal(chopFx, Chop.SignalName.Chopped);
		// if (!_stateMachine.IsCurrentState(ClickedStateName))
		// {
		// 	return;
		// }

		clickFeedbackComponent.Execute();
		SoundManager.Instance.PlayChoppingWoodSound();
		healthComponent.TakeDamage(_pendingDamage);
		_damageLabelSpawnerComponent.SpawnLabel(_pendingDamage, GlobalPosition);

		sprite.SelfModulate = new Color(2f, 2f, 2f, 1f);
		var tween = GetTree().CreateTween();
		tween.TweenProperty(
            sprite, 
            "self_modulate", 
            new Color(1f, 1f, 1f, 1f), 
            0.15
        );

		if (_stateMachine.IsCurrentState(ClickedStateName))
		{
			_stateMachine.ChangeState(IdleStateName);
		}
	}

	private void BreakedEnterState()
	{
		sprite.Play("die");
		hurtBox.Enabled = false;
		spawnEffectFeedbackComponent.Execute(GlobalPosition);
		spawnCollectibleFeedbackComponent.SpawnCollectible(GlobalPosition, (int)_treeCoinDropStat.Value);
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
		_stateMachine.ChangeState(IdleStateName);
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
		if (!_stateMachine.IsCurrentState(IdleStateName))
		{
			return;
		}

		_pendingDamage = damage;
		_stateMachine.ChangeState(ClickedStateName);
	}

	private sealed class IdleState : IState
	{
		private readonly Tree _tree;

		public IdleState(Tree tree)
		{
			_tree = tree;
		}

		public void Enter() => _tree.IdleEnterState();
		public void Update(double delta) { }
		public void PhysicsUpdate(double delta) { }
		public void Exit() { }
	}

	private sealed class ClickedState : IState
	{
		private readonly Tree _tree;

		public ClickedState(Tree tree)
		{
			_tree = tree;
		}

		public void Enter() => _tree.ClickedEnterState();
		public void Update(double delta) { }
		public void PhysicsUpdate(double delta) { }
		public void Exit() { }
	}

	private sealed class BreakedState : IState
	{
		private readonly Tree _tree;

		public BreakedState(Tree tree)
		{
			_tree = tree;
		}

		public void Enter() => _tree.BreakedEnterState();
		public void Update(double delta) { }
		public void PhysicsUpdate(double delta) { }
		public void Exit() { }
	}
}
