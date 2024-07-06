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
    private Farmer saveGameInfo = null!;
    private const string SavePath = @"C:\Projects\StardewValleyMods\TestMod\assets\会员6月25日_378995366.xml";
    private const string SaveInfoPath = @"C:\Projects\StardewValleyMods\TestMod\assets\SaveGameInfo.xml";

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
            
            this.ChangeHostPlayer();
            
            this.SerializeSave();
        }
    }

    private void DeserializeSave()
    {
        using (var fileStream = new FileStream(SavePath, FileMode.Open))
        {
            this.saveGame = (SaveGame)SaveGame.serializer.Deserialize(fileStream)!;
        }
        using (var fileStream = new FileStream(SaveInfoPath, FileMode.Open))
        {
            this.saveGameInfo = (Farmer)SaveGame.farmerSerializer.Deserialize(fileStream)!;
        }
        Log.Info("成功反序列化存档");
    }

    private void SerializeSave()
    {
        using (var streamWriter = new StreamWriter(SavePath))
        {
            SaveGame.serializer.Serialize(streamWriter, this.saveGame);
        }
        using (var streamWriter = new StreamWriter(SaveInfoPath))
        {
            SaveGame.farmerSerializer.Serialize(streamWriter, this.saveGameInfo);
        }
        Log.Info("成功序列化存档");
    }

    private void ChangeHostPlayer()
    {
        Log.Alert("开始更换主机：");

        var originPlayer = this.saveGame.player;
        var targetPlayer = this.saveGame.farmhands[0];
        
        (originPlayer.HouseUpgradeLevel, targetPlayer.HouseUpgradeLevel) =
            (targetPlayer.HouseUpgradeLevel, originPlayer.HouseUpgradeLevel);
        Log.Info("成功交换玩家房屋等级信息");

        this.saveGameInfo = targetPlayer;
        
        (this.saveGame.player, this.saveGame.farmhands[0]) = (targetPlayer, originPlayer);
        Log.Info("成功交换玩家信息");

        foreach (var location in this.saveGame.locations)
        {
            foreach (var building in location.buildings)
            {
                if (building.owner.Value == originPlayer.UniqueMultiplayerID)
                {
                    building.owner.Value = targetPlayer.UniqueMultiplayerID;
                }
                else if (building.owner.Value == targetPlayer.UniqueMultiplayerID)
                {
                    building.owner.Value = originPlayer.UniqueMultiplayerID;
                }
            }
        }
        Log.Info("成功交换建筑主人信息");
    }
}