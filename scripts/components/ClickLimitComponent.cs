using Godot;

namespace Game.Components;

[GlobalClass]
public partial class ClickLimitComponent : Node
{
	[Export]
	public int MaxClicks = 3;

	[Signal]
	public delegate void LimitReachedEventHandler();

	public int ClickCount { get; private set; }

	public void RegisterClick()
	{
		if (MaxClicks > 0 && ClickCount >= MaxClicks)
		{
			return;
		}

		ClickCount++;

		if (ClickCount >= MaxClicks)
		{
			EmitSignal(SignalName.LimitReached);
		}
	}

	public bool IsLimitReached()
	{
		return ClickCount >= MaxClicks;
	}

	public void Reset()
	{
		ClickCount = 0;
	}
}
