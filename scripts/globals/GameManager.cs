using Godot;

namespace Game.Globals;

public partial class GameManager : Node
{
	public Control woodLabel;

	public override void _Ready()
	{
		woodLabel = GetNodeOrNull<Control>("/root/Main/CanvasLayer/SubViewportContainer/WoodAmount");
		GD.Print(woodLabel.GlobalPosition);
	}
}
