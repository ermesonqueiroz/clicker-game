using Godot;

namespace Game.FX;

public partial class Chop : Node2D
{
	[Export]
	public AnimationPlayer animation;

	[Signal]
	public delegate void ChoppedEventHandler();

	public override void _Ready()
	{
		animation.AnimationFinished += (anim) => QueueFree();
	}

	private void EmitChop()
	{
		EmitSignal(SignalName.Chopped);
	}
}
