using Godot;

namespace Game.FX.ClickDust;

public partial class ClickDust : Node2D
{
	[Export]
	public AnimatedSprite2D sprite;

	public override void _Ready()
	{
		sprite.AnimationFinished += () => QueueFree();
	}
}
