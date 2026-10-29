using Godot;
using Game.Globals;
using Game.Components;

namespace Game;

public partial class Game : Node2D
{
	[Export]
	private ObjectSpawnerComponent _objectSpawnerComponent;

	[Export]
	private RoundController _roundController;

	private readonly PackedScene _treeScene = GD.Load<PackedScene>("res://scenes/entities/tree/tree.tscn");

	public override void _Ready()
	{
		GameManager.Instance.GameWorld = this;
		GameManager.Instance.RoundController = _roundController;
		_objectSpawnerComponent.SpawnObject(_treeScene);
		_objectSpawnerComponent.SpawnObject(_treeScene);
		_objectSpawnerComponent.SpawnObject(_treeScene);
	}
}
