using Godot;
using Game.Globals;

namespace Game;

public partial class Game : Node2D
{
	public override void _Ready()
	{
		GameManager.Instance.GameWorld = this;
	}
}
