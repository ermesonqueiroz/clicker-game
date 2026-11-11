using Godot;
using Game.Components;

namespace Game.Entities;

[Tool]
public partial class Island : Node2D
{
    [Export]
    private TileMapLayer _ground;

	[Export]
    private TileMapLayer _foam;

    private Vector2I _size;

    [Export]
    public Vector2I Size
    {
        get => _size;
        set
        {
            _size = value;
            if (_objectSpawner != null)
            {
                _objectSpawner.Dimensions = value;
            }
        }
    }

	[Export]
	private ObjectSpawner _objectSpawner;

	[ExportToolButton("Generate island")]
    private Callable GenerateIslandButton => Callable.From(GenerateIsland);

    private readonly Vector2I _atlasCoordGrama = new Vector2I(0, 0);
    private readonly Vector2I _atlasCoordTerra = new Vector2I(1, 0);

    public override void _Ready()
    {
		if (Engine.IsEditorHint())
        {
            GenerateIsland();
        }
    }

    public void GenerateIsland()
    {
        _ground.Clear();
        _foam.Clear();

		var coordinatesGrid = new Godot.Collections.Array<Vector2I>();

        for (int x = 0; x < Size.X; x++)
        {
            for (int y = 0; y < Size.Y; y++)
            {
				coordinatesGrid.Add(new Vector2I(x, y));
				_foam.SetCell(new Vector2I(x, y), 2, new Vector2I(0, 0));
            }
        }

		_ground.SetCellsTerrainConnect(coordinatesGrid, 0, 1);

		var rocksCells = new Godot.Collections.Array<Vector2I>();
		for (int x = 0; x < Size.X; x++)
        {
			rocksCells.Add(new Vector2I(x, Mathf.FloorToInt(Size.Y)));
			_foam.SetCell(new Vector2I(x, Mathf.FloorToInt(Size.Y)), 2, new Vector2I(0, 0));
        }

		_ground.SetCellsTerrainConnect(rocksCells, 0, 2);
    }

	public bool SpawnObject(PackedScene scene)
	{
		return _objectSpawner.SpawnObject(scene);
	}
}