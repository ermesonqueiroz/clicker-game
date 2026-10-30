using System;
using Godot;

namespace Game.Resources.Stats;

[GlobalClass]
public partial class Modifier(Callable Formula) : Resource
{
    [Export]
    public Callable Formula = Formula;
}
