using Godot;
using Godot.Collections;
using Game.Resources.Skills;

namespace Game.Resources;

[GlobalClass]
public partial class SaveGame : Resource
{
    public const string SaveGamePath = "user://savegame.tres";
    
    [Export]
    public Array<Skill> UnlockedSkills = [];

    public void Save()
    {
        ResourceSaver.Save(this, SaveGamePath);
    }

    public static SaveGame Load()
    {
        if (!ResourceLoader.Exists(SaveGamePath))
        {
            return new SaveGame();
        }

        return ResourceLoader.Load<SaveGame>(SaveGamePath);
    }
}
