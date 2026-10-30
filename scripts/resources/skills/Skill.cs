using Game.Resources.Stats;
using Godot;
using Godot.Collections;

namespace Game.Resources.Skills;

[GlobalClass]
public partial class Skill : Resource
{
    [Export]
    public string ID = "";

    [Export]
    public string Name = "";

    [Export]
    public string Description = "";

    [Export]
    public Array<Modifier> Modifiers = [];
}
