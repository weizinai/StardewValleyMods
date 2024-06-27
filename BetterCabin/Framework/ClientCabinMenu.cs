using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Menus;
using static StardewValley.Menus.BuildingSkinMenu;

namespace weizinai.StardewValleyMod.BetterCabin.Framework;

internal class ClientCabinMenu : IClickableMenu
{
    private const int WindowWidth = 576;
    private const int WindowHeight = 576;

    private readonly Building building;
    private SkinEntry currentSkin = null!;
    private readonly List<SkinEntry> skins = new();

    private ClickableTextureComponent okButton = null!;
    private ClickableTextureComponent nextSkinButton = null!;
    private ClickableTextureComponent previousSkinButton = null!;

    private Rectangle Bound => new(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);

    public ClientCabinMenu(Building targetBuilding)
        : base(Game1.uiViewport.Width / 2 - WindowWidth / 2, Game1.uiViewport.Height / 2 - WindowHeight / 2, WindowWidth, WindowHeight)
    {
        this.building = targetBuilding;

        var buildingData = targetBuilding.GetData();
        var index = 0;
        this.skins.Add(new SkinEntry(index++, null, "", ""));
        if (buildingData.Skins != null)
        {
            foreach (var skin in buildingData.Skins)
            {
                if (GameStateQuery.CheckConditions(skin.Condition, this.building.GetParentLocation()))
                {
                    this.skins.Add(new SkinEntry(index++, skin));
                }
            }
        }

        this.InitButton();
        this.SetSkin(Math.Max(this.skins.FindIndex(skin => skin.Id == this.building.skinId.Value), 0));
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (this.okButton.containsPoint(x, y))
        {
            this.exitThisMenu(playSound);
        }
        else if (this.previousSkinButton.containsPoint(x, y))
        {
            Game1.playSound("shwip");
            this.SetSkin(this.currentSkin.Index - 1);
        }
        else if (this.nextSkinButton.containsPoint(x, y))
        {
            Game1.playSound("shwip");
            this.SetSkin(this.currentSkin.Index + 1);
        }
        else
        {
            base.receiveLeftClick(x, y, playSound);
        }
    }

    private void SetSkin(int index)
    {
        index %= this.skins.Count;
        if (index < 0) index = this.skins.Count + index;
        this.SetSkin(this.skins[index]);
    }

    private void SetSkin(SkinEntry skin)
    {
        this.currentSkin = skin;
        if (this.building.skinId.Value != skin.Id)
        {
            this.building.skinId.Value = skin.Id;
            this.building.netBuildingPaintColor.Value.Color1Default.Value = true;
            this.building.netBuildingPaintColor.Value.Color2Default.Value = true;
            this.building.netBuildingPaintColor.Value.Color3Default.Value = true;
            var buildingData = this.building.GetData();
            if (buildingData != null && this.building.daysOfConstructionLeft.Value == buildingData.BuildDays)
            {
                this.building.daysOfConstructionLeft.Value = skin.Data?.BuildDays ?? buildingData.BuildDays;
            }
        }
    }

    public override void performHoverAction(int x, int y)
    {
        this.okButton.tryHover(x, y);
        this.previousSkinButton.tryHover(x, y);
        this.nextSkinButton.tryHover(x, y);
    }

    private void InitButton()
    {
        this.previousSkinButton = new ClickableTextureComponent(new Rectangle(this.Bound.Left, this.Bound.Center.Y - 32, 64, 64),
            Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
        this.nextSkinButton = new ClickableTextureComponent(new Rectangle(this.Bound.Right - 64, this.Bound.Center.Y - 32, 64, 64),
            Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
        this.okButton = new ClickableTextureComponent(new Rectangle(this.Bound.Right - 64, this.Bound.Bottom + 16, 64, 64),
            Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);

        if (this.skins.Count == 0)
        {
            this.nextSkinButton.visible = false;
            this.previousSkinButton.visible = false;
        }
    }

    public override void draw(SpriteBatch b)
    {
        if (!Game1.options.showClearBackgrounds) b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

        Game1.DrawBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);

        var sourceRect = this.building.getSourceRect();
        this.building.drawInMenu(b, this.Bound.Center.X - sourceRect.Width * 4 / 2, this.Bound.Center.Y - sourceRect.Height * 4 / 2 - 16);

        SpriteText.drawStringWithScrollCenteredAt(b, I18n.UI_ClientCabinMenu_ChooseSkin(), this.Bound.Center.X, this.yPositionOnScreen - 96);

        this.okButton.draw(b);
        this.nextSkinButton.draw(b);
        this.previousSkinButton.draw(b);

        this.drawMouse(b);
    }
}