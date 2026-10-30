using Godot;
using Game.Components;
using Game.Resources;
using System.ComponentModel.DataAnnotations.Schema;

namespace Game.Globals;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }

	public Node GameWorld;
	public Node UiContainer;
	public Control woodLabel;
	public RoundController RoundController;
	public SaveGame SaveGame;

	public override void _Ready()
	{
		woodLabel = GetNodeOrNull<Control>("/root/Main/CanvasLayer/SubViewportContainer/WoodAmount");
		Instance = this;
		SaveGame = SaveGame.Load();
	}

	public void AddChild(Node node)
	{
		GameWorld.CallDeferred("add_child", node);
	}

	public void AddUiChild(Node node)
	{
		UiContainer.CallDeferred("add_child", node);
	}

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
		{
			SaveGame.Save();
			GetTree().Quit();
		}
    }
}
