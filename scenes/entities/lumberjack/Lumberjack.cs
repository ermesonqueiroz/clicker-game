using Godot;
using Game.Common;

namespace Game.Entities;

public partial class Lumberjack : CharacterBody2D
{
	private const string LumberjacksGroup = "lumberjacks";

	[Export]
	public NavigationAgent2D NavigationAgent { get; private set; }

	[Export]
	public float MovementSpeed { get; set; } = 100.0f;

	[Export]
	public AnimationTree AnimationTree { get; private set; }

	public AnimationNodeStateMachinePlayback AnimationPlayback { get; private set; }
	public Tree CurrentTargetTree { get; set; }

	private StateMachine _stateMachine;

	public override void _Ready()
	{
		AddToGroup(LumberjacksGroup);
		AnimationPlayback = AnimationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();

		_stateMachine = new StateMachine();
		_stateMachine.AddState("Search", new LumberjackSearchState(this));
		_stateMachine.AddState("Move", new LumberjackMoveState(this));
		_stateMachine.AddState("Cut", new LumberjackCutState(this));

		_stateMachine.ChangeState("Search");
	}

	public override void _Process(double delta)
	{
		_stateMachine.Update(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		_stateMachine.PhysicsUpdate(delta);
	}

	public void ChangeState(string stateName)
	{
		_stateMachine.ChangeState(stateName);
	}

	public void UpdateBlendPositions(Vector2 direction)
	{
		AnimationTree.Set("parameters/Idle/blend_position", direction.X);
		AnimationTree.Set("parameters/Run/blend_position", direction.X);
		AnimationTree.Set("parameters/Interact/blend_position", direction.X);
	}

	public Tree FindClosestTree()
	{
		var trees = GetTree().GetNodesInGroup("trees");
		Tree closestTree = null;
		float minDistance = float.MaxValue;

		foreach (var node in trees)
		{
			if (node is Tree tree
				&& tree.healthComponent.CurrentHealth > 0
				&& !IsTreeTargetedByAnotherLumberjack(tree))
			{
				float distance = GlobalPosition.DistanceTo(tree.GlobalPosition);
				if (distance < minDistance)
				{
					minDistance = distance;
					closestTree = tree;
				}
			}
		}
		return closestTree;
	}

	private bool IsTreeTargetedByAnotherLumberjack(Tree tree)
	{
		foreach (var node in GetTree().GetNodesInGroup(LumberjacksGroup))
		{
			if (node is Lumberjack lumberjack
				&& lumberjack != this
				&& lumberjack.CurrentTargetTree == tree)
			{
				return true;
			}
		}

		return false;
	}

	private sealed class LumberjackSearchState : IState
	{
		private readonly Lumberjack _lumberjack;

		public LumberjackSearchState(Lumberjack lumberjack)
		{
			_lumberjack = lumberjack;
		}

		public void Enter()
		{
			_lumberjack.CurrentTargetTree = null;
			_lumberjack.Velocity = Vector2.Zero;
			_lumberjack.AnimationPlayback.Travel("Idle");
		}

		public void Update(double delta)
		{
			Tree targetTree = _lumberjack.FindClosestTree();

			if (targetTree != null)
			{
				_lumberjack.CurrentTargetTree = targetTree;
				_lumberjack.ChangeState("Move");
			}
		}

		public void PhysicsUpdate(double delta) { }

		public void Exit() { }
	}

	private sealed class LumberjackMoveState : IState
	{
		private readonly Lumberjack _lumberjack;

		public LumberjackMoveState(Lumberjack lumberjack)
		{
			_lumberjack = lumberjack;
		}

		public void Enter()
		{
			if (_lumberjack.CurrentTargetTree != null)
			{
				Marker2D marker = _lumberjack.GlobalPosition.X < _lumberjack.CurrentTargetTree.GlobalPosition.X
					? _lumberjack.CurrentTargetTree.MarkerLeft
					: _lumberjack.CurrentTargetTree.MarkerRight;

				_lumberjack.NavigationAgent.TargetPosition = marker.GlobalPosition;
			}
		}

		public void Update(double delta) { }

		public void PhysicsUpdate(double delta)
		{
			if (_lumberjack.NavigationAgent.IsTargetReached())
			{
				_lumberjack.Velocity = Vector2.Zero;
				_lumberjack.ChangeState("Cut");
				return;
			}

			var navPointDirection = _lumberjack.ToLocal(_lumberjack.NavigationAgent.GetNextPathPosition()).Normalized();
			_lumberjack.Velocity = navPointDirection * _lumberjack.MovementSpeed;

			if (navPointDirection != Vector2.Zero)
			{
				_lumberjack.UpdateBlendPositions(navPointDirection);
				_lumberjack.AnimationPlayback.Travel("Run");
			}

			_lumberjack.MoveAndSlide();
		}

		public void Exit() { }
	}

	private sealed class LumberjackCutState : IState
	{
		private readonly Lumberjack _lumberjack;

		public LumberjackCutState(Lumberjack lumberjack)
		{
			_lumberjack = lumberjack;
		}

		public void Enter()
		{
			_lumberjack.Velocity = Vector2.Zero;
			var directionToTree = (
				_lumberjack.CurrentTargetTree.GlobalPosition - _lumberjack.GlobalPosition
			).Normalized();
			_lumberjack.UpdateBlendPositions(directionToTree);
			_lumberjack.AnimationPlayback.Travel("Interact");
		}

		public void Update(double delta)
		{
			if (_lumberjack.CurrentTargetTree.healthComponent.CurrentHealth <= 0)
			{
				_lumberjack.ChangeState("Search");
			}
		}

		public void PhysicsUpdate(double delta) { }

		public void Exit() {}
	}
}