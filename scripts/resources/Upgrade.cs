using Godot;

namespace Game.Resources;

[GlobalClass]
public partial class Upgrade(string Name, Texture2D Icon) : Resource
{
	[Export]
	public string Name = Name;

	[Export]
	public Texture2D Icon = Icon;

	public Upgrade() : this("", null) {}
}
