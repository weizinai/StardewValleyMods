using SaveModInfo.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Log;

namespace SaveModInfo.Handler;

internal class RecordModInfoHandler : BaseHandler
{
    public RecordModInfoHandler(IModHelper helper) : base(helper) { }

    public override void Init()
    {
        this.Helper.Events.GameLoop.SaveCreated += this.OnSaveCreated;
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
    }
    
    private void OnSaveCreated(object? sender, SaveCreatedEventArgs e)
    {
        this.RecordModInfo();
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        this.Helper.Events.GameLoop.Saved += this.OnSaved;
    }

    private void OnSaved(object? sender, SavedEventArgs e)
    {
        this.Helper.Events.GameLoop.Saved -= this.OnSaved;

        this.RecordModInfo();
    }
    
    private void RecordModInfo()
    {
        var modInfo = this.Helper.ModRegistry.GetAll().ToDictionary(mod => mod.Manifest.UniqueID, mod => mod.Manifest.Name);

        var player = Game1.MasterPlayer;
        this.Helper.Data.WriteJsonFile($"data/{player.displayName}_{player.UniqueMultiplayerID}.json", modInfo);
        
        Log.Info(I18n.UI_RecordModInfo_Tooltip());
    }
}