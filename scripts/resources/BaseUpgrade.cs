using Godot;

namespace Game.Resources;

[GlobalClass]
public partial class BaseUpgrade(string Name, Texture2D Icon, Upgrade Head) : Resource
{
	[Export]
	public string Name = Name;

	[Export]
	public Texture2D Icon = Icon;

	[Export]
	public Upgrade Head = Head;

	public BaseUpgrade() : this("", null, null) {}
}
