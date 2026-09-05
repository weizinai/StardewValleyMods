namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handler;

// internal class SpendLimitHandler : BaseHandlerWithConfig<ModConfig>
// {
//     public const string SpendLimitKey = ModEntry.ModDataPrefix + "SpendLimit";
//     public const string SpendLimitDataKey = ModEntry.ModDataPrefix + "SpendLimitData";
//     public const string SpendAmountKey = ModEntry.ModDataPrefix + "SpendAmount";
//
//     public SpendLimitHandler(IModHelper helper, ModConfig config)
//         : base(helper, config)
//     {
//         if (Context.IsWorldReady) this.InitSpendLimitConfig();
//     }
//
//     public override void Apply()
//     {
//         this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
//         this.Helper.Events.GameLoop.DayStarted += this.OnDayStarted;
//         this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
//         this.Helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
//     }
//
//     public override void Clear()
//     {
//         this.Helper.Events.GameLoop.SaveLoaded -= this.OnSaveLoaded;
//         this.Helper.Events.GameLoop.DayStarted -= this.OnDayStarted;
//         this.Helper.Events.Input.ButtonsChanged -= this.OnButtonChanged;
//         this.Helper.Events.Multiplayer.PeerConnected -= this.OnPeerConnected;
//     }
//
//     private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
//     {
//         this.InitSpendLimitConfig();
//     }
//
//     private void OnDayStarted(object? sender, DayStartedEventArgs e)
//     {
//         if (Game1.IsClient)
//         {
//             Game1.player.modData[SpendAmountKey] = "0";
//             if (Game1.MasterPlayer.modData.ContainsKey(SpendLimitKey))
//             {
//                 Log.NoIconHUDMessage("今日消费金额已重置");
//             }
//         }
//     }
//
//     private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
//     {
//         if (!Context.IsPlayerFree) return;
//
//         if (this.Config.SpendLimitManagerMenuKey.JustPressed())
//         {
//             if (!Game1.MasterPlayer.modData.ContainsKey(SpendLimitKey))
//             {
//                 Log.NoIconHUDMessage("主机未开启金钱限制功能");
//                 return;
//             }
//
//             if (Game1.IsServer)
//             {
//                 Game1.activeClickableMenu = new SpendLimitManagerMenu();
//             }
//             else
//             {
//                 var player = Game1.player;
//                 var amount = int.Parse(player.modData[SpendAmountKey]);
//                 SpendLimitHelper.TryGetFarmerSpendLimit(player.Name, out var limit);
//                 Log.NoIconHUDMessage($"当日消费：{amount}\n可用额度：{limit - amount}\n总额度：{limit}");
//             }
//         }
//     }
//
//     private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
//     {
//         if (Game1.IsClient) return;
//
//         if (!this.Config.SpendLimit) return;
//
//         var farmerName = Game1.getFarmer(e.Peer.PlayerID).Name;
//         if (!SpendLimitHelper.TryGetFarmerSpendLimit(farmerName, out _))
//         {
//             SpendLimitHelper.SetFarmerSpendLimit(farmerName, this.Config.DefaultSpendLimit);
//             Log.Info($"{farmerName}没有额度信息，已将其额度设置为{this.Config.DefaultSpendLimit}金");
//         }
//     }
//
//     private void InitSpendLimitConfig()
//     {
//         if (Game1.IsClient) return;
//
//         var modData = Game1.MasterPlayer.modData;
//         if (this.Config.SpendLimit)
//         {
//             var farmhands = Game1.getAllFarmhands()
//                 .Where(x => !x.isUnclaimedFarmhand)
//                 .Select(x => x.Name);
//
//             var limitData = new Dictionary<string, int>();
//             if (!modData.ContainsKey(SpendLimitDataKey))
//             {
//                 limitData = new Dictionary<string, int>();
//                 foreach (var name in farmhands) limitData[name] = this.Config.DefaultSpendLimit;
//                 Log.NoIconHUDMessage($"当前存档没有额度信息，已自动将所有玩家的额度设置为{this.Config.DefaultSpendLimit}金");
//             }
//             else
//             {
//                 limitData = JsonSerializer.Deserialize<Dictionary<string, int>>(modData[SpendLimitDataKey])!;
//                 foreach (var name in farmhands)
//                 {
//                     if (!limitData.ContainsKey(name))
//                     {
//                         limitData[name] = this.Config.DefaultSpendLimit;
//                         Log.NoIconHUDMessage($"{name}没有额度信息，已将其额度设置为{this.Config.DefaultSpendLimit}金");
//                     }
//                 }
//             }
//
//             modData[SpendLimitKey] = "true";
//             modData[SpendLimitDataKey] = JsonSerializer.Serialize(limitData);
//         }
//         else
//         {
//             modData.Remove(SpendLimitKey);
//             Log.NoIconHUDMessage("花钱限制功能已关闭");
//         }
//     }
// }