using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.PiCore.Handler;

namespace weizinai.StardewValleyMod.SaveModInfo.Handler;

internal class RecordModInfoHandler : BaseHandler
{
    public RecordModInfoHandler(IModHelper helper) : base(helper) { }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        if (Game1.dayOfMonth == 1)
            this.RecordModInfo();
        else
            this.Helper.Events.GameLoop.Saved += this.OnSaved;
    }

    private void OnSaved(object? sender, SavedEventArgs e)
    {
        this.RecordModInfo();

        this.Helper.Events.GameLoop.Saved -= this.OnSaved;
    }

    private void RecordModInfo()
    {
        var modInfo = this.Helper.ModRegistry.GetAll().ToDictionary(mod => mod.Manifest.UniqueID, mod => mod.Manifest.Name);

        this.Helper.Data.WriteJsonFile($"data/{Constants.SaveFolderName}.json", modInfo);

        Logger.Info(I18n.UI_RecordModInfo_Tooltip());
    }
}