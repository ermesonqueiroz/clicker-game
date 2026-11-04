using Godot;
using System;

public partial class CursorManager : Node
{
	public static CursorManager Instance { get; private set; }

	public bool IdleCursorEnabled = true;

	public override void _Ready()
	{
		Instance = this;

		Input.SetCustomMouseCursor(GD.Load<Texture2D>("res://assets/ui/cursors/Cursor_01.png"));
		Input.SetCustomMouseCursor(GD.Load<Texture2D>("res://assets/ui/cursors/Cursor_02.png"), Input.CursorShape.PointingHand);
	}

	public void EnableIdleCursor()
	{
		IdleCursorEnabled = true;
	}

	public void DisableIdleCursor()
	{
		IdleCursorEnabled = false;
	}
}
