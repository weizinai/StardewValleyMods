using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;


namespace ActiveMenuAnywhere.Framework.ActiveMenu.Mountain;

public class RobinActiveMenu : BaseActiveMenu
{
    public RobinActiveMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        // if ((int)Game1.player.daysUntilHouseUpgrade < 0 && !Game1.IsThereABuildingUnderConstruction())
        {
            List<Response> options = new List<Response>();
            options.Add(new Response("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")));
            if (Game1.IsMasterGame)
            {
                if ((int)Game1.player.houseUpgradeLevel < 3)
                {
                    options.Add(new Response("Upgrade",
                        Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse")));
                }
                else if ((Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") ||
                          Game1.MasterPlayer.mailReceived.Contains("JojaMember") || Game1.MasterPlayer.hasCompletedCommunityCenter()) &&
                         (int)Game1.RequireLocation<Town>("Town").daysUntilCommunityUpgrade <= 0)
                {
                    if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
                    {
                        options.Add(new Response("CommunityUpgrade",
                            Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
                    }
                    else if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
                    {
                        options.Add(new Response("CommunityUpgrade",
                            Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
                    }
                }
            }
            else if ((int)Game1.player.houseUpgradeLevel < 3)
            {
                options.Add(new Response("Upgrade",
                    Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeCabin")));
            }

            if ((int)Game1.player.houseUpgradeLevel >= 2)
            {
                if (Game1.IsMasterGame)
                {
                    options.Add(new Response("Renovate",
                        Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateHouse")));
                }
                else
                {
                    options.Add(new Response("Renovate",
                        Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateCabin")));
                }
            }

            options.Add(new Response("Construct", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct")));
            options.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave")));
            // this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"), options.ToArray(),
            //   "carpenter");
            Game1.drawObjectQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"), options.ToArray());
        }
    }
}