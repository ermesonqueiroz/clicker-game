using Godot;
using Game.Globals;

namespace Game.Components;

[GlobalClass]
public partial class SpawnEffectFeedbackComponent : Node
{
	[Export]
	public PackedScene effectScene;

	public void Execute(Vector2 position)
	{
		Node2D explosionInstance = effectScene.Instantiate<Node2D>();
		explosionInstance.Position = position;
		GameManager.Instance.AddChild(explosionInstance);
	}
}
