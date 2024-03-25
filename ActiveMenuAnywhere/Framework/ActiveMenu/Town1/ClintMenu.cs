﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.TokenizableStrings;
using StardewValley.Tools;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class ClintMenu : BaseActiveMenu
{
    public ClintMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        var options = new List<Response>()
        {
            // 商店
            new("Shop", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Shop")),
        };

        if (Game1.player.toolBeingUpgraded.Value == null)
        {
            // 工具升级
            options.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Upgrade")));
        }
        else
        {
            options.Add((new Response("Receive", "取回工具")));
        }

        // 砸开晶球
        var hasGeode = Game1.player.Items.Any(item1 => Utility.IsGeode(item1));
        if (hasGeode)
            options.Add(new Response("Process", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Geodes")));

        // 离开
        options.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave")));

        Game1.currentLocation.createQuestionDialogue("", options.ToArray(), AfterDialogueBehavior);

        /*

        */
    }
    
    private void AfterDialogueBehavior(Farmer who, string whichAnswer)
    {
        switch (whichAnswer)
        {
            case "Shop":
                Utility.TryOpenShopMenu("Blacksmith", "Clint");
                break;
            case "Upgrade":
                Utility.TryOpenShopMenu("ClintUpgrade", "Clint");
                break;
            case "Receive":
                if (Game1.player.toolBeingUpgraded.Value != null && 
                    Game1.player.daysLeftForToolUpgrade.Value <= 0)
                {
                    if (Game1.player.freeSpotsInInventory() > 0 || Game1.player.toolBeingUpgraded.Value is GenericTool)
                    {
                        var tool = Game1.player.toolBeingUpgraded.Value;
                        Game1.player.toolBeingUpgraded.Value = null;
                        Game1.player.hasReceivedToolUpgradeMessageYet = false;
                        Game1.player.holdUpItemThenMessage(tool);
                        if (tool is GenericTool)
                        {
                            tool.actionWhenClaimed();
                        }
                        else
                        {
                            Game1.player.addItemToInventoryBool(tool);
                        }
                    }
                }
                else
                    Game1.drawObjectDialogue("工具还未升级完成");
                break;
            case "Process":
                Game1.activeClickableMenu = new GeodeMenu();
                break;
            case "Leave":
                Game1.exitActiveMenu();
                break;
        }
    }
}