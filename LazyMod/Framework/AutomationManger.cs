using LazyMod.Framework.Automation;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace LazyMod.Framework;

public class AutomationManger
{
    private readonly ModConfig config;
    private readonly List<Automate> automations = new();

    private GameLocation? location;
    private Farmer? player;
    private Tool? tool;
    private Item? item;
    private bool modEnable = true;

    public AutomationManger(IModHelper helper, ModConfig config)
    {
        // 初始化
        this.config = config;
        InitAutomates();
        // 注册事件
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.GameLoop.DayEnding += OnDayEnding;
        helper.Events.Input.ButtonsChanged += OnButtonChanged;
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs dayStartedEventArgs)
    {
        if (config.AutoOpenAnimalDoor) AutoAnimal.AutoToggleAnimalDoor(true);
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs updateTickedEventArgs)
    {
        if (!modEnable) return;
        
        location = Game1.currentLocation;
        player = Game1.player;
        tool = player?.CurrentTool;
        item = player?.CurrentItem;

        if (location is null || player is null) return;

        foreach (var automate in automations) automate.AutoDoFunction(location, player, tool, item);
    }

    private void OnDayEnding(object? sender, DayEndingEventArgs dayEndingEventArgs)
    {
        if (config.AutoOpenAnimalDoor) AutoAnimal.AutoToggleAnimalDoor(false);
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (config.ToggleModStateKeybind.JustPressed())
        {
            Game1.addHUDMessage(modEnable ? new HUDMessage(I18n.Message_ModDisable()) : new HUDMessage(I18n.Message_ModEnable()));
            modEnable = !modEnable;
        }
    }

    private void InitAutomates()
    {
        automations.AddRange(new Automate[]
        {
            new AutoFarming(config),
            new AutoAnimal(config),
            new AutoMining(config),
            new AutoForaging(config),
            new AutoFishing(config),
            new AutoFood(config),
            new AutoOther(config),
        });
    }
}