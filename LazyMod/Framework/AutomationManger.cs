using LazyMod.Framework.Automation;
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

    public AutomationManger(ModConfig config)
    {
        this.config = config;
        InitAutomates();
    }
    
    public void OnDayStarted()
    {
        if (config.AutoOpenAnimalDoor) AutoAnimal.AutoToggleAnimalDoor(true);
    }
    
    public void Update()
    {
        location = Game1.currentLocation;
        player = Game1.player;
        tool = player?.CurrentTool;
        item = player?.CurrentItem;
        
        if (location is null || player is null) return;

        foreach (var automate in automations) automate.AutoDoFunction(location, player, tool, item);
    }
    
    public void OnDayEnded()
    {
        if (config.AutoOpenAnimalDoor) AutoAnimal.AutoToggleAnimalDoor(false);
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