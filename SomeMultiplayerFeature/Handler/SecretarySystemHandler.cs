using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Objects;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handler;

internal class SecretarySystemHandler : BaseHandlerWithConfig<ModConfig>
{
    public const string SecretaryKey = ModEntry.ModDataPrefix + "Secretary";

    public SecretarySystemHandler(IModHelper helper, ModConfig config) : base(helper, config) { }

    public override void Apply()
    {
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
        this.Helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
    }

    public override void Clear()
    {
        this.Helper.Events.Input.ButtonsChanged -= this.OnButtonChanged;
        this.Helper.Events.Multiplayer.ModMessageReceived -= this.OnModMessageReceived;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (Game1.IsServer && this.Config.SecretarySystemKey.JustPressed())
        {
            var farmers = Game1.otherFarmers.Values
                .OrderBy(IsSecretary)
                .Select(x => new KeyValuePair<string, string>(x.UniqueMultiplayerID.ToString(), x.Name + (x.mailReceived.Contains(SecretaryKey) ? "(*)" : "")));

            Game1.currentLocation.ShowPagedResponses("请选择你要聘用或者解雇的玩家: (*代表目前是秘书的玩家)", farmers.ToList(), value =>
            {
                this.Helper.Multiplayer.SendMessage("Secretary", "SecretarySystem", new[] { ModEntry.ModUniqueId }, new[] { long.Parse(value) });
            });
        }
    }

    private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e is { Type: "SecretarySystem", FromModID: ModEntry.ModUniqueId })
        {
            var message = e.ReadAs<string>();
            switch (message)
            {
                case "Secretary":
                {
                    var player = Game1.player;
                    if (player.mailReceived.Contains(SecretaryKey))
                    {
                        player.mailReceived.Toggle(SecretaryKey, false);
                        Log.NoIconHUDMessage("很遗憾的通知你，你被炒鱿鱼了！");
                        MultiplayerLog.NoIconHUDMessage($"{player.Name}被炒鱿鱼了！");
                    }
                    else
                    {
                        player.mailReceived.Toggle(SecretaryKey, true);
                        Log.NoIconHUDMessage("恭喜你，你获得了秘书的工作！");
                        MultiplayerLog.NoIconHUDMessage($"{player.Name}获得了秘书的工作！");
                    }
                    break;
                }
                case "BackpackPurchase":
                {
                    PurchaseBackpack(Context.IsPlayerFree);
                    break;
                }
            }
        }
    }

    public static void PurchaseBackpack(bool showAnimation = true)
    {
        var farmer = Game1.player;
        var flag = farmer.MaxItems == 12;
        farmer.Money -= flag ? 2000 : 10000;
        farmer.increaseBackpackSize(12);

        var message = Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.870" + (flag ? 8 : 9));
        if (showAnimation) farmer.holdUpItemThenMessage(new SpecialItem(99, message));
    }

    public static bool IsSecretary(Farmer farmer)
    {
        return Game1.IsServer || farmer.mailReceived.Contains(SecretaryKey);
    }
}