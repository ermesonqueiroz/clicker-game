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
	private ObjectClickComponent _objectClickComponent;

	[Export]
	private ClickFeedbackComponent _clickFeedbackComponent;


	[Export]
	private ClickLimitComponent _clickLimitComponent;


	private readonly StateMachine<BookState> _stateMachine = new();	
	
	public override void _Ready()
	{
		_objectClickComponent.Clicked += () => _stateMachine.ChangeState(BookState.Clicked);

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
		_objectClickComponent.Enabled = false;
		_clickLimitComponent.Reset();
		_sprite.Play("turn_page");
		await ToSignal(_sprite, AnimatedSprite2D.SignalName.AnimationFinished);

		_objectClickComponent.Enabled = true;
		_stateMachine.ChangeState(BookState.Idle);
	}
}
