using Godot;
using Game.Globals;
using Game.Components;

namespace Game;

public partial class Game : Node2D
{
	[Export]
	public ObjectSpawnerComponent ObjectSpawnerComponent;

	public float Cooldown = 3.0f;
	public float Timer = 0.0f;

	public PackedScene TreeScene = GD.Load<PackedScene>("res://scenes/entities/tree/tree.tscn");

	public override void _Ready()
	{
		GameManager.Instance.GameWorld = this;
	}

	public override void _Process(double delta)
	{
		Timer += (float)delta;

		if (Timer >= Cooldown)
		{
			Timer = 0.0f;
			ObjectSpawnerComponent.SpawnObject(TreeScene);
		}
	}
}
