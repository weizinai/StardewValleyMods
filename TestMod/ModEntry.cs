using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using weizinai.StardewValleyMod.Common.Log;

namespace TestMod;

public class ModEntry : Mod
{
    private readonly KeybindList testKet = new(SButton.O);
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
            SaveGame saveGame;
            using (var fileStream = new FileStream(SavePath, FileMode.Open))
            {
                saveGame = (SaveGame)SaveGame.serializer.Deserialize(fileStream)!;
                Log.Info(saveGame.player.UniqueMultiplayerID.ToString());
            }

            (saveGame.player, saveGame.farmhands[0]) = (saveGame.farmhands[0], saveGame.player);

            using (var streamWriter = new StreamWriter(SavePath))
            {
                SaveGame.serializer.Serialize(streamWriter, saveGame);
            }
        }
    }
}