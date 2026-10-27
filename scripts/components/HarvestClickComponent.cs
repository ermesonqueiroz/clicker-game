using Godot;

namespace Game.Components;

[GlobalClass]
public partial class HarvestClickComponent : Node
{
	[Export]
	public Area2D area;

	[Signal]
	public delegate void ClickedEventHandler();

	public override void _Ready()
	{
		area.InputEvent += OnAreaInputEvent;
	}

	public void OnAreaInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			EmitSignal(SignalName.Clicked);
		}
	}
}
