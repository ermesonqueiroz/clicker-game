using Godot;

public partial class Explosion : Node2D
{
	[Export]
	public AnimatedSprite2D sprite;

	public override void _Ready()
	{
		sprite.AnimationFinished += () => QueueFree();
	}
}
