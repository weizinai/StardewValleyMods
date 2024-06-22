using Microsoft.Xna.Framework;

namespace BetterCabin.Framework.Config;

internal class OnlineTimeConfig
{
    public bool Enable;
    public int XOffset;
    public int YOffset;
    public Color TextColor;

    public OnlineTimeConfig(bool enable, int xOffset, int yOffset, Color textColor)
    {
        Enable = enable;
        XOffset = xOffset;
        YOffset = yOffset;
        TextColor = textColor;
    }
}