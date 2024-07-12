using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace weizinai.StardewValleyMod.MoreInfo.Framework;

internal abstract class LocationInfoHandler : BaseInfoHandler
{
    protected override string HoverText
    {
        get
        {
            var stringBuilder = new StringBuilder();
            foreach (var (key, value) in this.LocationInfo)
                stringBuilder.AppendLine($"{key}: {value}");
            stringBuilder.Length--;
            return stringBuilder.ToString();
        }
    }

    protected override Rectangle Bound => new((int)this.Position.X, (int)this.Position.Y, 64, 64);

    protected readonly Dictionary<string, int> LocationInfo = new();

    public override bool IsEnable()
    {
        return this.LocationInfo.Any();
    }

    public override void Draw(SpriteBatch b)
    {
        IClickableMenu.drawTextureBox(b, this.Bound.X, this.Bound.Y, this.Bound.Width, this.Bound.Height, Color.White);
        b.Draw(this.Texture, new Rectangle(this.Bound.X + 16, this.Bound.Y + 16, 32, 32), this.SourceRectangle, Color.White);
    }
}