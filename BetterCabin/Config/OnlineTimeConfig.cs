using Microsoft.Xna.Framework;

namespace weizinai.StardewValleyMod.BetterCabin.Config;

internal class OnlineTimeConfig
{
    public bool Enable { get; set; }
    public int XOffset { get; set; }
    public int YOffset { get; set; }
    public Color TextColor { get; set; }

    public OnlineTimeConfig(bool enable, int xOffset, int yOffset, Color textColor)
    {
        this.Enable = enable;
        this.XOffset = xOffset;
        this.YOffset = yOffset;
        this.TextColor = textColor;
    }
}
