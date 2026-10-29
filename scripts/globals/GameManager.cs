using Godot;
using Game.Components;

namespace Game.Globals;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }

	public Node GameWorld;
	public Node UiContainer;
	public Control woodLabel;
	public RoundController RoundController;

	public override void _Ready()
	{
		woodLabel = GetNodeOrNull<Control>("/root/Main/CanvasLayer/SubViewportContainer/WoodAmount");
		Instance = this;
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
