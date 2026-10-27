using Godot;

namespace Game.Components;

[GlobalClass]
public partial class HarvestClickComponent : Node
{
	[Export]
	public Area2D area;

	[Signal]
	public delegate void ClickedEventHandler();

	[Export]
	public bool Enabled
	{
		get => area.Monitoring;
		set => area.Monitoring = value;
	}


	public override void _Ready()
	{
		area.InputEvent += OnAreaInputEvent;
	}

	public void OnAreaInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (Enabled == false)
		{
			return;
		}

		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			EmitSignal(SignalName.Clicked);
		}
	}
}
