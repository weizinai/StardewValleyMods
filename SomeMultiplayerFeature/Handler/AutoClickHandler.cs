using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.PiCore.Logging;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handler;

internal class AutoClickHandler : BaseHandler
{
    private int cooldown;

    public AutoClickHandler(IModHelper helper) : base(helper) { }

    public override void Apply()
    {
        this.helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
        this.helper.Events.Display.MenuChanged += this.OnMenuChanged;
    }

    public override void Clear()
    {
        this.helper.Events.GameLoop.OneSecondUpdateTicked -= this.OnOneSecondUpdateTicked;
        this.helper.Events.Display.MenuChanged -= this.OnMenuChanged;
    }

    private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (Game1.activeClickableMenu is LevelUpMenu levelUpMenu)
        {
            this.cooldown++;

            if (this.cooldown > 20)
            {
                if (levelUpMenu.isProfessionChooser)
                {
                    var professionToChoose = this.helper.Reflection.GetField<List<int>>(levelUpMenu, "professionsToChoose").GetValue()[0];
                    Game1.player.professions.Add(professionToChoose);
                    levelUpMenu.getImmediateProfessionPerk(professionToChoose);
                    levelUpMenu.isActive = false;
                    levelUpMenu.informationUp = false;
                    levelUpMenu.isProfessionChooser = false;
                    levelUpMenu.RemoveLevelFromLevelList();
                    Logger<ModEntry>.Info("你长时间没有选择职业，已自动为你选择左侧职业。");
                }
                else
                {
                    levelUpMenu.okButtonClicked();
                    Logger<ModEntry>.Info("你长时间没有确认，已自动点击确认按钮。");
                }
            }
        }
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        if (e.NewMenu is LevelUpMenu)
        {
            this.cooldown = 0;
        }
    }
}
