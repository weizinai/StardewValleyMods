using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using weizinai.StardewValleyMod.HelpWanted.Model;

namespace weizinai.StardewValleyMod.HelpWanted.UI;

/// <summary>镇上的原版布告栏（贴图 <c>LooseSprites/Billboard</c>，源区域 338×198）。</summary>
public class VanillaQuestBoard : BaseQuestBoard
{
    public VanillaQuestBoard() : base(
        BoardType.Vanilla,
        Game1.temporaryContent.Load<Texture2D>("LooseSprites/Billboard"),
        new Rectangle(0, 0, 338, 198)
    )
    { }
}
