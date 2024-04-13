using LazyMod.Framework.Automation;
using StardewValley;

namespace LazyMod.Framework;

public class LazyModManager
{
    private ModConfig config;
    private readonly List<Automate> automations = new();

    public LazyModManager(ModConfig config)
    {
        this.config = config;
        automations.Add(new AutoHoe(config));
        automations.Add(new AutoWateringCan(config));
        automations.Add(new AutoHand(config));
    }

    public void Update()
    {
        UpdateAutomation();
    }

    private void UpdateAutomation()
    {
        var location = Game1.currentLocation;
        var player = Game1.player;
        var tool = player.CurrentTool;
        var item = player.CurrentItem;

        foreach (var automate in automations)
        {
            automate.AutoDoFunction(location, player, tool, item);
        }
    }
}