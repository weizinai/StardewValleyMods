using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using weizinai.StardewValleyMod.HelpWanted.Model;

namespace weizinai.StardewValleyMod.HelpWanted.Menu;

public class VanillaQuestBoard : BaseQuestBoard
{
    public VanillaQuestBoard() : base(
        BoardType.Vanilla,
        Game1.temporaryContent.Load<Texture2D>("LooseSprites/Billboard"),
        new Rectangle(0, 0, 338, 198)
    ) { }
}