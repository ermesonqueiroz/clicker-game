using System;
using Godot;

namespace Game.Resources.Stats;

[GlobalClass]
public partial class Modifier : Resource
{
    [Export]
    public Callable Formula;
}
