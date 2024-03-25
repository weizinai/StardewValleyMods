using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;


namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class RobinMenu : BaseActiveMenu
{
    public RobinMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.IsThereABuildingUnderConstruction())
        {
            var options = new List<Response>
                { new("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")) };
            if (Game1.IsMasterGame)
            {
                // 房屋升级_主玩家
                if (Game1.player.HouseUpgradeLevel < 3)
                {
                    options.Add(new Response("Upgrade",
                        Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse")));
                }
                // 社区升级
                else if (CommunityUpgrade())
                {
                    // 拖车
                    if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
                    {
                        options.Add(new Response("CommunityUpgrade",
                            Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
                    }
                    // 捷径
                    else if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
                    {
                        options.Add(new Response("CommunityUpgrade",
                            Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
                    }
                }
            }
            else if (Game1.player.HouseUpgradeLevel < 3)
            {
                // 房屋升级
                options.Add(new Response("Upgrade",
                    Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeCabin")));
            }

            // 装修房屋
            if (Game1.player.HouseUpgradeLevel >= 2)
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

            // 农场建筑
            options.Add(new Response("Construct", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct")));
            // 离开
            options.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave")));

            Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"),
                options.ToArray(), "carpenter");
        }
        else
        {
            Utility.TryOpenShopMenu("Carpenter", "Robin");
        }
    }

    // 判断是否可以进行社区升级
    private bool CommunityUpgrade()
    {
        var isCommunityCenterCompleted = Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") ||
                                         Game1.MasterPlayer.hasCompletedCommunityCenter();
        var isJojaMember = Game1.MasterPlayer.mailReceived.Contains("JojaMember");
        var isCommunityUpgradeCompleted = Game1.RequireLocation<StardewValley.Locations.Town>("Town").daysUntilCommunityUpgrade.Value <= 0;
        return (isCommunityCenterCompleted || isJojaMember) && isCommunityUpgradeCompleted;
    }
}