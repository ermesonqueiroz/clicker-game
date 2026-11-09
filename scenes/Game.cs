using Godot;
using Game.Globals;
using Game.Components;

namespace Game;

public partial class Game : Node2D
{
	[Export]
	private ObjectSpawnerComponent _objectSpawnerComponent;

	[Export]
	private Timer _spawnObjectTimer;

	private readonly PackedScene _treeScene = GD.Load<PackedScene>("res://scenes/entities/tree/tree.tscn");

	public override void _Ready()
	{
		GameManager.Instance.GameWorld = this;
		_objectSpawnerComponent.SpawnObject(_treeScene);
		_objectSpawnerComponent.SpawnObject(_treeScene);
		_objectSpawnerComponent.SpawnObject(_treeScene);

		_spawnObjectTimer.Timeout += () => _objectSpawnerComponent.SpawnObject(_treeScene);
	}
}
