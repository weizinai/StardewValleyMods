using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using weizinai.StardewValleyMod.Common.Log;

namespace TestMod;

public class ModEntry : Mod
{
    private readonly KeybindList testKet = new(SButton.O);
    private SaveGame saveGame = null!;
    private const string SavePath = @"C:\Projects\StardewValleyMods\TestMod\assets\会员6月25日_378995366.xml";

    public override void Entry(IModHelper helper)
    {
        Log.Init(this.Monitor);

        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.testKet.JustPressed())
        {
            this.DeserializeSave();
            
        }
    }

    private void DeserializeSave()
    {
        using var fileStream = new FileStream(SavePath, FileMode.Open);
        this.saveGame = (SaveGame)SaveGame.serializer.Deserialize(fileStream)!;
        Log.Info("成功反序列化存档");
    }

    private void SerializeSave()
    {
        using var streamWriter = new StreamWriter(SavePath);
        SaveGame.serializer.Serialize(streamWriter, this.saveGame);
        Log.Info("成功序列化存档");
    }

    private void ChangeHostPlayer()
    {
        (this.saveGame.player, this.saveGame.farmhands[0]) = 
            (this.saveGame.farmhands[0], this.saveGame.player);
    }
}