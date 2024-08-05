using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;
using xTile.Dimensions;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class MarnieOption : BaseOption
{
    private readonly IModHelper helper;

    private GameLocation originLocation = null!;
    private Location originViewport;

    public MarnieOption(IModHelper helper) : base(I18n.UI_Option_Marnie(), GetSourceRectangle(0))
    {
        this.helper = helper;
    }

    public override void Apply()
    {
        var options = new List<Response>
        {
            new("Supplies", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Supplies")),
            new("Purchase", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Animals")),
            new("Leave", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Leave"))
        };
        if (Game1.player.mailReceived.Contains("MarniePetAdoption") || Game1.player.mailReceived.Contains("MarniePetRejectedAdoption"))
            options.Insert(2, new Response("Adopt", Game1.content.LoadString("Strings\\1_6_Strings:AdoptPets")));
        Game1.currentLocation.createQuestionDialogue("", options.ToArray(), this.AfterDialogueBehavior);
    }

    private void AfterDialogueBehavior(Farmer who, string whichAnswer)
    {
        switch (whichAnswer)
        {
            case "Supplies":
                Utility.TryOpenShopMenu("AnimalShop", "Marnie");
                break;
            case "Purchase":
                Game1.currentLocation.ShowAnimalShopMenu(_ =>
                {
                    this.originLocation = Game1.currentLocation;
                    this.originViewport = Game1.viewport.Location;
                });
                this.helper.Events.Display.MenuChanged += this.OnMenuChanged;
                break;
            case "Adopt":
                Utility.TryOpenShopMenu("PetAdoption", "Marnie");
                break;
            case "Leave":
                Game1.exitActiveMenu();
                Game1.player.forceCanMove();
                break;
        }
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        if (e.OldMenu is PurchaseAnimalsMenu)
        {
            var request = Game1.getLocationRequest(this.originLocation.NameOrUniqueName);
            request.OnWarp += () =>
            {
                Game1.currentLocation = this.originLocation;
                Game1.viewport.Location = this.originViewport;
            };
            Game1.warpFarmer(request, Game1.player.TilePoint.X, Game1.player.TilePoint.Y, Game1.player.FacingDirection);
            this.helper.Events.Display.MenuChanged -= this.OnMenuChanged;
        }
    }
}