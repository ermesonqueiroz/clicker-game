using Godot;
using Game.Resources;

namespace Game.Globals;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }

	public Node GameWorld;
	public Node UiContainer;

	public GameData GameData { get; private set; }

	public override void _Ready()
	{
		Instance = this;

		GameData = new GameData();
	}

	public void AddChild(Node node)
	{
		GameWorld.CallDeferred("add_child", node);
	}

	public void AddUiChild(Node node)
	{
		UiContainer.CallDeferred("add_child", node);
	}
}
