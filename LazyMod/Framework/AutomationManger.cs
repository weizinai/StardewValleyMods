﻿using LazyMod.Framework.Automation;
using StardewValley;

namespace LazyMod.Framework;

public class AutomationManger
{
    private readonly ModConfig config;
    private readonly List<Automate> automations = new();

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
        UpdateAutomation();
    }
    
    public void OnDayEnded()
    {
        if (config.AutoOpenAnimalDoor) AutoAnimal.AutoToggleAnimalDoor(false);
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
            new AutoForaging(config),
            new AutoFishing(config),
            new AutoFood(config),
            new AutoOther(config),
        });
    }
}