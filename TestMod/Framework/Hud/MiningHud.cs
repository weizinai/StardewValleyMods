using Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace TestMod.Framework.Hud;

public class MiningHud
{
    private readonly ImageWithBackground ladderHud;
    private readonly ImageWithBackground shaftHud;
    private readonly ImageWithBackground monsterHud;
    private readonly ImageWithBackground mineralHud;

    public MiningHud(ModConfig config)
    {
        ladderHud = new ImageWithBackground(GenerateBackground(0), Game1.temporaryContent.Load<Texture2D>("Maps/Mines/mine_desert"),
            new Rectangle(208, 160, 16, 16))
        {
            CheckHidden = () => !config.ShowLadderInfo
        };
        shaftHud = new ImageWithBackground(GenerateBackground(1), Game1.temporaryContent.Load<Texture2D>("Maps/Mines/mine_desert"),
            new Rectangle(224, 160, 16, 16))
        {
            CheckHidden = () => !config.ShowShaftInfo
        };
        monsterHud = new ImageWithBackground(GenerateBackground(2), Game1.temporaryContent.Load<Texture2D>("Characters/Monsters/Green Slime"),
            new Rectangle(2, 268, 12, 10))
        {
            CheckHidden = () => !config.ShowMonsterInfo
        };
        mineralHud = new ImageWithBackground(GenerateBackground(3), Game1.temporaryContent.Load<Texture2D>("TileSheets/tools"),
            new Rectangle(193, 128, 15, 15))
        {
            CheckHidden = () => !config.ShowMineralInfo
        };
    }

    public void Update()
    {
        if (Game1.player.currentLocation is not MineShaft) return;

        var i = 0;
        if (!ladderHud.IsHidden()) ladderHud.Position = GetDestinationRectangle(i++).Location.ToVector2();
        if (!shaftHud.IsHidden()) shaftHud.Position = GetDestinationRectangle(i++).Location.ToVector2();
        if (!monsterHud.IsHidden()) monsterHud.Position = GetDestinationRectangle(i++).Location.ToVector2();
        if (!mineralHud.IsHidden()) mineralHud.Position = GetDestinationRectangle(i).Location.ToVector2();
        
        ladderHud.Update();
        shaftHud.Update();
        monsterHud.Update();
        mineralHud.Update();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (Game1.player.currentLocation is not MineShaft) return;

        ladderHud.Draw(spriteBatch);
        shaftHud.Draw(spriteBatch);
        monsterHud.Draw(spriteBatch);
        mineralHud.Draw(spriteBatch);
        
        if (ladderHud.Hover) IClickableMenu.drawHoverText(spriteBatch, "Ladder", Game1.smallFont);
        if (shaftHud.Hover) IClickableMenu.drawHoverText(spriteBatch, "Shaft", Game1.smallFont);
        if (monsterHud.Hover) IClickableMenu.drawHoverText(spriteBatch, "Monster", Game1.smallFont);
        if (mineralHud.Hover) IClickableMenu.drawHoverText(spriteBatch, "Mineral", Game1.smallFont);
    }

    private Image GenerateBackground(int index)
    {
        var texture = Game1.temporaryContent.Load<Texture2D>("Maps/MenuTiles");
        var destinationRectangle = GetDestinationRectangle(index);
        var sourceRectangle = new Rectangle(0, 256, 64, 64);
        return new Image(texture, destinationRectangle, sourceRectangle);
    }

    private Rectangle GetDestinationRectangle(int index)
    {
        return new Rectangle(0, 88 + 72 * index, 64, 64);
    }
}