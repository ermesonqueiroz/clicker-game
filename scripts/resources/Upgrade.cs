using Godot;
using Godot.Collections;

namespace Game.Resources;

[GlobalClass]
public partial class Upgrade(int Cost, Upgrade Next, int Level) : Resource
{
	[Export]
	public int Cost = Cost;

	[Export]
	public Upgrade Next = Next;

	[Export]
	public int Level = Level;

	[Export]
	public Array<Modifier> Modifiers = [];

	public Upgrade() : this(0, null, 0) {}
}
