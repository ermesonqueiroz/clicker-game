using Godot;
using System;
using System.Collections.Generic;

namespace Game.Globals;

public partial class EventBus : Node
{
	public static EventBus Instance { get; private set; }
	private readonly Dictionary<Type, Delegate> _subscribers = [];

	public override void _Ready()
	{
		Instance = this;
	}

	public override void _ExitTree()
	{
		Instance = null;
	}

	public void Subscribe<TEvent>(Action<TEvent> handler)
	{
		var eventType = typeof(TEvent);
		if (_subscribers.TryGetValue(eventType, out var subscribers))
		{
			_subscribers[eventType] = Delegate.Combine(subscribers, handler);
			return;
		}

		_subscribers[eventType] = handler;
	}

	public void Unsubscribe<TEvent>(Action<TEvent> handler)
	{
		var eventType = typeof(TEvent);
		if (!_subscribers.TryGetValue(eventType, out var subscribers))
		{
			return;
		}

		var updatedSubscribers = Delegate.Remove(subscribers, handler);
		if (updatedSubscribers == null)
		{
			_subscribers.Remove(eventType);
			return;
		}

		_subscribers[eventType] = updatedSubscribers;
	}

	public void Publish<TEvent>(TEvent eventData)
	{
		if (_subscribers.TryGetValue(typeof(TEvent), out var subscribers))
		{
			((Action<TEvent>)subscribers).Invoke(eventData);
		}
	}
}

public sealed record AreaClickedEvent(Area2D Area);
