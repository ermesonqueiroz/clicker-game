using Godot;
using Game.Globals;
using Game.Entities;
using Godot.Collections;

namespace Game;

public partial class Game : Node2D
{
	[Export]
	private Island _island;

	[Export]
	private Timer _spawnObjectTimer;

	private readonly Array<PackedScene> _treeScenes = [
		GD.Load<PackedScene>("res://scenes/entities/tree1/tree1.tscn"),
		GD.Load<PackedScene>("res://scenes/entities/tree2/tree2.tscn"),
		GD.Load<PackedScene>("res://scenes/entities/tree3/tree3.tscn"),
		GD.Load<PackedScene>("res://scenes/entities/tree4/tree4.tscn")

	];
	private readonly PackedScene _lumberjackScene = GD.Load<PackedScene>("res://scenes/entities/lumberjack/lumberjack.tscn");

	private RandomNumberGenerator _rng = new();

	public void SpawnRandomTree()
	{
		int selectedIndex = (int)_rng.RandWeighted([20.0f, 20.0f, 20.0f, 20.0f]);
		_island.SpawnObject(_treeScenes[selectedIndex]);
	}

	public override void _Ready()
	{
		GameManager.Instance.GameWorld = this;
		SpawnRandomTree();
		SpawnRandomTree();
		SpawnRandomTree();

		_spawnObjectTimer.Timeout += SpawnRandomTree;

		GameManager.Instance.GameData.LumberjackAmountChanged += (_) => _island.SpawnObject(_lumberjackScene);
	}
}
