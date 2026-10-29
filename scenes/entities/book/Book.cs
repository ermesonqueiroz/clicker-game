using Game.Common;
using Game.Components;
using Godot;

namespace Game.Component;

enum BookState
{
	Idle,
	Clicked,
	Reseting
}

public partial class Book : Area2D
{
	[Export]
	private AnimatedSprite2D _sprite;
	
	[Export]
	private HurtBox _hurtBox;

	[Export]
	private ClickFeedbackComponent _clickFeedbackComponent;


	[Export]
	private ClickLimitComponent _clickLimitComponent;


	private readonly StateMachine<BookState> _stateMachine = new();	
	
	public override void _Ready()
	{
		_hurtBox.HitReceived += OnHitReceived;

		_stateMachine.AddState(BookState.Idle, null);
		_stateMachine.AddState(BookState.Clicked, null, ClickedEnterState);
		_stateMachine.AddState(BookState.Reseting, null, ResetingEnterState);
		_stateMachine.ChangeState(BookState.Idle);
	}

	private void ClickedEnterState()
	{
		_clickLimitComponent.RegisterClick();
		_clickFeedbackComponent.Execute();
		_sprite.Animation = "writing";
		_sprite.Frame++;

		if (_clickLimitComponent.IsLimitReached())
		{
			_stateMachine.ChangeState(BookState.Reseting);
			return;
		}

		_stateMachine.ChangeState(BookState.Idle);
	}

	private async void ResetingEnterState()
	{
		_hurtBox.Enabled = false;
		_clickLimitComponent.Reset();
		_sprite.Play("turn_page");
		await ToSignal(_sprite, AnimatedSprite2D.SignalName.AnimationFinished);

		_hurtBox.Enabled = true;
		_stateMachine.ChangeState(BookState.Idle);
	}

	private void OnHitReceived(float _)
	{
		_stateMachine.ChangeState(BookState.Clicked);
	}
}
