﻿using System.Reflection;
using Common.Integration;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class RSVQuestBoardOption : BaseOption
{
    private readonly IModHelper helper;

    public RSVQuestBoardOption(Rectangle sourceRect, IModHelper helper) :
        base(I18n.Option_RSVQuestBoard(), sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        var questController = RSVIntegration.GetType("RidgesideVillage.Questing.QuestController");
        object[] parameters = { Game1.currentLocation, new[] { "VillageQuestBoard" }, Game1.player, new Point() };
        questController?.GetMethod("OpenQuestBoard", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, parameters);
    }
}