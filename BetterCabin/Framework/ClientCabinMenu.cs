using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.Menus;
using static StardewValley.Menus.BuildingSkinMenu;

namespace weizinai.StardewValleyMod.BetterCabin.Framework;

internal class ClientCabinMenu : IClickableMenu
{
	private const int WindowWidth = 576;

	private const int WindowHeight = 576;

	private Rectangle previewPane;

	private ClickableTextureComponent okButton = null!;

	/// <summary>The building whose skin to change.</summary>
	private readonly Building building;

	private ClickableTextureComponent nextSkinButton = null!;

	private ClickableTextureComponent previousSkinButton = null!;

	/// <summary>The building skins available in the menu.</summary>
	private readonly List<SkinEntry> skins = new();

	/// <summary>The current building skin shown in the menu.</summary>
	private SkinEntry currentSkin = null!;
	
	public ClientCabinMenu(Building targetBuilding)
		: base(Game1.uiViewport.Width / 2 - WindowWidth / 2, Game1.uiViewport.Height / 2 - WindowHeight / 2, WindowWidth, WindowHeight)
	{
		Game1.player.Halt();
		this.building = targetBuilding;
		var buildingData = targetBuilding.GetData();
		var index = 0;
		if (buildingData.Skins != null)
		{
			foreach (BuildingSkin skin2 in buildingData.Skins)
			{
				if (skin2.Id == this.building.skinId.Value || GameStateQuery.CheckConditions(skin2.Condition, this.building.GetParentLocation()))
				{
					this.skins.Add(new SkinEntry(index++, skin2));
				}
			}
		}
		this.RepositionElements();
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
			this.SetSkin(this.currentSkin.Index + 1);
			Game1.playSound("shwip");
		}
		else
		{
			base.receiveLeftClick(x, y, playSound);
		}
	}

	private void SetSkin(int index)
	{
		index %= this.skins.Count;
		if (index < 0)
		{
			index = this.skins.Count + index;
		}
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
			BuildingData buildingData = this.building.GetData();
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

	private void RepositionElements()
	{
		this.previewPane.Y = this.yPositionOnScreen + 48;
		this.previewPane.Width = 576;
		this.previewPane.Height = 576;
		this.previewPane.X = this.xPositionOnScreen;
		var panelRectangle = this.previewPane;
		panelRectangle.Inflate(-16, -16);
		this.previousSkinButton = new ClickableTextureComponent(new Rectangle(panelRectangle.Left, panelRectangle.Center.Y - 32, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
		this.nextSkinButton = new ClickableTextureComponent(new Rectangle(panelRectangle.Right - 64, panelRectangle.Center.Y - 32, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
		panelRectangle.Y += 64;
		panelRectangle.Height = 0;
		panelRectangle.Y += 80;
		panelRectangle.Y += 64;
		this.okButton = new ClickableTextureComponent(new Rectangle(this.previewPane.Right - 64 - 16, this.previewPane.Bottom - 64 - 16, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
		if (this.skins.Count == 0)
		{
			this.nextSkinButton.visible = false;
			this.previousSkinButton.visible = false;
		}
	}

	public override void draw(SpriteBatch b)
	{
		if (!Game1.options.showClearBackgrounds)
		{
			b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
		}
		Game1.DrawBox(this.previewPane.X, this.previewPane.Y, this.previewPane.Width, this.previewPane.Height);
		Rectangle rectangle = this.previewPane;
		rectangle.Inflate(0, 0);
		b.End();
		b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, Utility.ScissorEnabled);
		b.GraphicsDevice.ScissorRectangle = rectangle;
		Vector2 buildingDrawCenter = new Vector2(this.previewPane.X + this.previewPane.Width / 2.0f, this.previewPane.Y + this.previewPane.Height / 2.0f - 16);
		Rectangle sourceRect = this.building.getSourceRectForMenu() ?? this.building.getSourceRect();
		this.building.drawInMenu(b, (int)buildingDrawCenter.X - (int)(this.building.tilesWide.Value / 2f * 64f), (int)buildingDrawCenter.Y - sourceRect.Height * 4 / 2);
		b.End();
		b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
		SpriteText.drawStringWithScrollCenteredAt(b, Game1.content.LoadString("Strings\\Buildings:BuildingSkinMenu_ChooseAppearance", ""), this.xPositionOnScreen + this.width / 2, this.previewPane.Top - 96);
		this.okButton.draw(b);
		this.nextSkinButton.draw(b);
		this.previousSkinButton.draw(b);
		this.drawMouse(b);
	}
}