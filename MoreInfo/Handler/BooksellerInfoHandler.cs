using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using weizinai.StardewValleyMod.MoreInfo.Framework;

namespace weizinai.StardewValleyMod.MoreInfo.Handler;

internal class BooksellerInfoHandler : BaseInfoHandler
{
    protected override Rectangle Bound => new((int)this.Position.X, (int)this.Position.Y, 40, 40);
    protected override string HoverText => Game1.content.LoadString("Strings\\1_6_Strings:BooksellerInTown");

    public BooksellerInfoHandler()
    {
        var questButton = Game1.dayTimeMoneyBox.questButton.bounds;
        this.Position = new Vector2(questButton.Left, questButton.Bottom + 16);
        this.Texture = Game1.mouseCursors_1_6;
        this.SourceRectangle = new Rectangle(52, 475, 20, 20);
    }

    public override bool IsEnable()
    {
        return Utility.getDaysOfBooksellerThisSeason().Contains(Game1.dayOfMonth);
    }

    public override void Draw(SpriteBatch b)
    {
        b.Draw(this.Texture, this.Bound, this.SourceRectangle, Color.White);
    }
}