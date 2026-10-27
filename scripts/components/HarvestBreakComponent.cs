using Godot;

namespace Game.Components;

[GlobalClass]
public partial class HarvestBreakComponent : Node
{
	[Export]
	private int _clicksToBreak = 3;

	[Signal]
	public delegate void BreakedEventHandler();

	public int ClicksAmount { get; private set; } = 0;

	public void RegisterClick()
	{
		ClicksAmount++;

		if (ClicksAmount >= _clicksToBreak)
		{
			EmitSignal(SignalName.Breaked);
		}
	}
}
