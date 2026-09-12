using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using weizinai.StardewValleyMod.HelpWanted.Model;

namespace weizinai.StardewValleyMod.HelpWanted.UI;

/// <summary>里奇赛德村的委托板（贴图 <c>LooseSprites/RSVQuestBoard</c>，源区域 338×424）。</summary>
public class RSVQuestBoard : BaseQuestBoard
{
    public RSVQuestBoard() : base(
        BoardType.RSV,
        Game1.temporaryContent.Load<Texture2D>("LooseSprites/RSVQuestBoard"),
        new Rectangle(0, 0, 338, 424)
    )
    { }
}
