using Godot;
using Game.Components;
using Game.Common;
using Game.Globals;
using Game.Resources;

namespace Game.Entities;

public partial class Tree : Node2D, IPoolableObject
{
	[Export] public HurtBox HurtBox;

	[Export] public SpawnCollectibleFeedbackComponent spawnCollectibleFeedbackComponent;

	[Export] public HealthComponent HealthComponent;

	[Export] public Sprite2D Sprite;
	[Export] public AnimationPlayer AnimationPlayer;

	[Export] public Marker2D MarkerLeft;

	[Export] public Marker2D MarkerRight;

	[Export] public StatsComponent StatsComponent;
 
	private Stat _treeCoinDropStat => StatsComponent.GetStat("tree_coin_drop");
	public readonly StateMachine StateMachine = new();
	private ObjectSpawner _objectSpawner;
	public float PendingDamage;

	public override void _Ready()
	{
		AddToGroup("trees");

		HurtBox.HitReceived += (_) =>
		{
			StateMachine.ChangeState(HealthComponent.CurrentHealth > 0 ? "Clicked" : "Breaked");
		};

		StateMachine.AddState("Idle", new IdleState(this));
		StateMachine.AddState("Clicked", new ClickedState(this));
		StateMachine.AddState("Breaked", new BreakedState(this));
		StateMachine.ChangeState("Idle");
	}

	private void BreakedEnterState()
	{
		AnimationPlayer.Play("die");
		HurtBox.Enabled = false;
		GameManager.Instance.GameData.CoinAmount += (int)_treeCoinDropStat.Value;

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

	public void OnPoolSpawned(ObjectSpawner spawner)
	{
		AddToGroup("trees");
		_objectSpawner = spawner;
		Modulate = Colors.White;
		HealthComponent.ResetHealth();
		HurtBox.Enabled = true;
		StateMachine.ChangeState("Idle");
	}

	public void OnPoolDespawned()
	{
		RemoveFromGroup("trees");
		HurtBox.Enabled = false;
	}

	private void IdleEnterState()
	{
		HurtBox.Enabled = true;
		AnimationPlayer.Play("idle");
	}

	private void OnHitReceived(float damage)
	{
		if (!StateMachine.IsCurrentState("Idle"))
		{
			return;
		}

		PendingDamage = damage;
		StateMachine.ChangeState("Clicked");
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

		public void Enter()
		{
			_tree.AnimationPlayer.Play("click");
			_tree.HurtBox.Enabled = false;
			SoundManager.Instance.PlayChoppingWoodSound();

			_tree.AnimationPlayer.AnimationFinished += OnAnimationFinished;
		}

		public void Update(double delta) { }
		public void PhysicsUpdate(double delta) { }

		public void Exit() {
			_tree.AnimationPlayer.AnimationFinished -= OnAnimationFinished;
		}

		private void OnAnimationFinished(StringName animation)
		{
			if (_tree.HealthComponent.CurrentHealth > 0)
			{
				_tree.StateMachine.ChangeState("Idle");
			}
		}
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
