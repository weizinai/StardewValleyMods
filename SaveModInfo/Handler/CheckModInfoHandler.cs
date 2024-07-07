using System.Text;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Log;

namespace SaveModInfo.Handler;

internal class CheckModInfoHandler : BaseHandler
{
    public static readonly Dictionary<string, string> CheckResult = new();

    public CheckModInfoHandler(IModHelper helper) : base(helper) { }

    public override void Init()
    {
        this.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var savesPath = Constants.SavesPath;
        
        if (Directory.Exists(savesPath))
        {
            var currentModInfo = this.Helper.ModRegistry.GetAll()
                .Select(mod => mod.Manifest.UniqueID)
                .ToHashSet();

            foreach (var directory in Directory.EnumerateDirectories(savesPath))
            {
                var saveName = directory.Split(Path.DirectorySeparatorChar).Last();
                var lastModInfo = this.Helper.Data.ReadJsonFile<Dictionary<string, string>>($"data/{saveName}.json");

                if (lastModInfo == null)
                {
                    CheckResult.Add(saveName, I18n.UI_CheckModInfo_NoInfo());
                    continue;
                }

                var message = new StringBuilder();
                foreach (var (id, name) in lastModInfo)
                {
                    if (!currentModInfo.Contains(id))
                    {
                        message.AppendLine(I18n.UI_CheckModInfo_RemovedMod(name));
                    }
                }
                if (message.Length > 0) message.Length--;
                CheckResult.Add(saveName, message.ToString());
            }
        }
        else
        {
            Log.Error("The saves directory does not exist.");
        }
    }
}