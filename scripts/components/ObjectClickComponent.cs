using Godot;
using Game.Globals;

namespace Game.Components;

[GlobalClass]
public partial class ObjectClickComponent : Node
{
	[Export]
	public Area2D area;

	[Signal]
	public delegate void ClickedEventHandler();

	[Export]
	public bool Enabled
	{
		get => area.Monitoring;
		set => area?.SetDeferred("monitoring", value);
	}


	public override void _Ready()
	{
		EventBus.Instance.Subscribe<AreaClickedEvent>(OnAreaClicked);
	}

	public override void _ExitTree()
	{
		EventBus.Instance.Unsubscribe<AreaClickedEvent>(OnAreaClicked);
	}

	private void OnAreaClicked(AreaClickedEvent clickEvent)
	{
		if (Enabled && area == clickEvent.Area)
		{
			EmitSignal(SignalName.Clicked);
		}
	}
}
