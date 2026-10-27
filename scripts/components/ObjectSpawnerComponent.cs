using Godot;
using Game.Globals;
using System.Collections.Generic;

namespace Game.Components;

public interface IPoolableObject
{
	void OnPoolSpawned(ObjectSpawnerComponent spawner);
	void OnPoolDespawned();
}

[GlobalClass]
public partial class ObjectSpawnerComponent : Node
{
	private sealed class ObjectPool
	{
		public readonly Queue<Node2D> Available = [];
		public readonly HashSet<Node2D> Active = [];
	}

	private readonly Dictionary<PackedScene, ObjectPool> _pools = [];

	[Export]
	public Vector2 TopLeftPoint;

	[Export]
	public Vector2 BottomRightPoint;

	[Export]
	public int PoolSize { get; set; } = 10;

	[Export]
	public float MinimumSpawnDistance { get; set; } = 192.0f;

	private const int PositionAttempts = 100;

	public Vector2 GetRandomPosition()
	{
		float x = (float)GD.RandRange(TopLeftPoint.X, BottomRightPoint.X);
		float y = (float)GD.RandRange(TopLeftPoint.Y, BottomRightPoint.Y);

		return new Vector2(x, y);
	}

	public bool SpawnObject(PackedScene scene)
	{
		if (scene == null)
		{
			GD.PushError("Cannot spawn an object from a null PackedScene.");
			return false;
		}

		if (PoolSize <= 0 || MinimumSpawnDistance < 0.0f ||
			TopLeftPoint.X > BottomRightPoint.X || TopLeftPoint.Y > BottomRightPoint.Y)
		{
			GD.PushError("ObjectSpawnerComponent has invalid pool size, spawn distance, or position bounds.");
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

		if (pool.Available.Count == 0)
		{
			GD.PushWarning($"Object pool is full (capacity: {PoolSize}); skipping spawn.");
			return false;
		}

		if (!TryGetAvailablePosition(out Vector2 position))
		{
			GD.PushWarning("No valid spawn position found; skipping spawn.");
			return false;
		}

		Node2D node = pool.Available.Dequeue();
		node.GlobalPosition = position;
		node.Visible = true;
		node.SetProcess(true);
		node.SetPhysicsProcess(true);

		if (node is IPoolableObject poolableObject)
		{
			poolableObject.OnPoolSpawned(this);
		}

		pool.Active.Add(node);
		return true;
	}

	public bool DespawnObject(Node2D node)
	{
		foreach (ObjectPool pool in _pools.Values)
		{
			if (!pool.Active.Remove(node))
			{
				continue;
			}

			if (node is IPoolableObject poolableObject)
			{
				poolableObject.OnPoolDespawned();
			}

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

		for (int i = 0; i < PoolSize; i++)
		{
			Node2D node = scene.Instantiate<Node2D>();
			GameManager.Instance.GameWorld.AddChild(node);
			node.Visible = false;
			node.SetProcess(false);
			node.SetPhysicsProcess(false);

			if (node is IPoolableObject poolableObject)
			{
				poolableObject.OnPoolDespawned();
			}

			pool.Available.Enqueue(node);
		}

		return pool;
	}

	private bool TryGetAvailablePosition(out Vector2 position)
	{
		float minimumDistanceSquared = MinimumSpawnDistance * MinimumSpawnDistance;

		for (int attempt = 0; attempt < PositionAttempts; attempt++)
		{
			Vector2 candidate = GetRandomPosition();
			bool positionIsAvailable = true;

			foreach (ObjectPool pool in _pools.Values)
			{
				foreach (Node2D activeNode in pool.Active)
				{
					if (activeNode.GlobalPosition.DistanceSquaredTo(candidate) <= minimumDistanceSquared)
					{
						positionIsAvailable = false;
						break;
					}
				}

				if (!positionIsAvailable)
				{
					break;
				}
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
