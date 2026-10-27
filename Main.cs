using Game.Globals;
using Godot;

public partial class Main : Node2D
{
	[Export]
	public Node UiContainer;

	public override void _Ready()
	{
		GameManager.Instance.UiContainer = UiContainer;
	}
}
