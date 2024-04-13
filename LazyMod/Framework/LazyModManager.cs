using LazyMod.Framework.Automation;
using StardewValley;

namespace LazyMod.Framework;

public class LazyModManager
{
    private readonly ModConfig config;
    private readonly List<Automate> automations = new();

    public LazyModManager(ModConfig config)
    {
        this.config = config;
        InitAutomates();
    }

    public void OnDayStarted()
    {
        if (config.AutoOpenAnimalDoor)
            AutoAnimal.AutoOpenAnimalDoor();
    }

    public void Update()
    {
        UpdateAutomation();
    }
    
    public void OnDayEnded()
    {
        if (config.AutoOpenAnimalDoor)
            AutoAnimal.AutoCloseAnimalDoor();
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
    
    private void InitAutomates()
    {
        automations.AddRange(new Automate[]
        {
            new AutoFarming(config),
            new AutoAnimal(config),
            new AutoMining(config),
            new AutoOther(config),
        });
    }
}