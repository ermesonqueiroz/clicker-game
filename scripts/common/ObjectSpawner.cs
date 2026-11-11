using Godot;
using Game.Globals;
using System.Collections.Generic;

namespace Game.Components;

public interface IPoolableObject
{
    void OnPoolSpawned(ObjectSpawner spawner);
    void OnPoolDespawned();
}

[GlobalClass]
[Tool]
public partial class ObjectSpawner : Node2D
{
    private sealed class ObjectPool
    {
        public readonly Queue<Node2D> Available = [];
        public readonly HashSet<Node2D> Active = [];
    }

    private readonly Dictionary<PackedScene, ObjectPool> _pools = [];

    [Export]
    public Vector2I Dimensions { get; set; }

    [Export]
    public int PoolSize { get; set; } = 10;

    [Export]
    public float MinimumSpawnDistance { get; set; } = 192.0f;

    [ExportGroup("Grid Settings")]
    [Export]
    private bool _showDebugGrid = true;

    [Export]
    private Color _debugGridColor = new Color(1, 1, 1, 0.3f);

    private const int CellSize = 64;
    private const int PositionAttempts = 100;

    public override void _Draw()
    {
        if (!_showDebugGrid) return;

        float totalWidth = Dimensions.X * CellSize;
        float totalHeight = Dimensions.Y * CellSize;

        DrawRect(new Rect2(0, 0, totalWidth, totalHeight), _debugGridColor, false, 2.0f);

        for (int x = 0; x <= Dimensions.X; x++)
        {
            float posX = x * CellSize;
            DrawLine(new Vector2(posX, 0), new Vector2(posX, totalHeight), _debugGridColor, 1.0f);
        }

        for (int y = 0; y <= Dimensions.Y; y++)
        {
            float posY = y * CellSize;
            DrawLine(new Vector2(0, posY), new Vector2(totalWidth, posY), _debugGridColor, 1.0f);
        }
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            QueueRedraw();
        }
    }

    public Vector2 GetRandomPosition()
    {
        int randomCellX = (int)(GD.Randi() % Dimensions.X);
        int randomCellY = (int)(GD.Randi() % Dimensions.Y);

        float cellCenterOffset = CellSize / 2.0f;
        return new Vector2(
            (randomCellX * CellSize) + cellCenterOffset,
            (randomCellY * CellSize) + cellCenterOffset
        );
    }

    public bool SpawnObject(PackedScene scene)
    {
        if (scene == null)
        {
            GD.PushError("Cannot spawn an object from a null PackedScene.");
            return false;
        }

        if (PoolSize <= 0 || MinimumSpawnDistance < 0.0f || Dimensions.X <= 0 || Dimensions.Y <= 0)
        {
            GD.PushError("ObjectSpawner has invalid pool size, spawn distance, or dimensions.");
            return false;
        }

        if (!_pools.TryGetValue(scene, out ObjectPool pool))
        {
            pool = CreatePool(scene);
            if (pool == null)
            {
                return false;
            }

            _pools.Add(scene, pool);
        }

        if (!TryGetAvailablePosition(out Vector2 position))
        {
            GD.PushWarning("No valid spawn position found; skipping spawn.");
            return false;
        }

        if (pool.Available.Count == 0 && !ExpandPool(scene, pool))
        {
            return false;
        }

        Node2D node = pool.Available.Dequeue();
        node.GlobalPosition = GlobalPosition + position;
        node.Visible = true;
        node.SetProcess(true);
        node.SetPhysicsProcess(true);

        if (node is IPoolableObject poolableObject)
            poolableObject.OnPoolSpawned(this);

        pool.Active.Add(node);
        return true;
    }

    public bool DespawnObject(Node2D node)
    {
        foreach (ObjectPool pool in _pools.Values)
        {
            if (!pool.Active.Remove(node)) continue;

            if (node is IPoolableObject poolableObject)
                poolableObject.OnPoolDespawned();

            node.Visible = false;
            node.SetProcess(false);
            node.SetPhysicsProcess(false);
            pool.Available.Enqueue(node);
            return true;
        }

        return false;
    }

    private ObjectPool CreatePool(PackedScene scene)
    {
        if (GameManager.Instance?.GameWorld == null)
        {
            GD.PushError("Cannot create object pool before the game world is ready.");
            return null;
        }

        ObjectPool pool = new();
        AddObjectsToPool(scene, pool, PoolSize);
        return pool;
    }

    private bool ExpandPool(PackedScene scene, ObjectPool pool)
    {
        int currentCapacity = pool.Available.Count + pool.Active.Count;
        if (currentCapacity <= 0 || currentCapacity > int.MaxValue / 2)
        {
            GD.PushError("Cannot expand object pool beyond its maximum capacity.");
            return false;
        }

        AddObjectsToPool(scene, pool, currentCapacity);
        return true;
    }

    private void AddObjectsToPool(PackedScene scene, ObjectPool pool, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Node2D node = scene.Instantiate<Node2D>();
            GameManager.Instance.GameWorld.AddChild(node);
            node.Visible = false;
            node.SetProcess(false);
            node.SetPhysicsProcess(false);

            if (node is IPoolableObject poolableObject)
                poolableObject.OnPoolDespawned();

            pool.Available.Enqueue(node);
        }
    }

    private bool TryGetAvailablePosition(out Vector2 position)
    {
        float minimumDistanceSquared = MinimumSpawnDistance * MinimumSpawnDistance;

        for (int attempt = 0; attempt < PositionAttempts; attempt++)
        {
            Vector2 candidate = GetRandomPosition();
            Vector2 globalCandidate = GlobalPosition + candidate;
            bool positionIsAvailable = true;

            foreach (ObjectPool pool in _pools.Values)
            {
                foreach (Node2D activeNode in pool.Active)
                {
                    if (activeNode.GlobalPosition.DistanceSquaredTo(globalCandidate) <= minimumDistanceSquared)
                    {
                        positionIsAvailable = false;
                        break;
                    }
                }

                if (!positionIsAvailable) break;
            }

            if (positionIsAvailable)
            {
                position = candidate;
                return true;
            }
        }

        position = Vector2.Zero;
        return false;
    }
}
