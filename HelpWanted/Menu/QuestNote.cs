using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using weizinai.StardewValleyMod.HelpWanted.Model;

namespace weizinai.StardewValleyMod.HelpWanted.Menu;

public class QuestNote : ClickableComponent
{
    public readonly QuestModel QuestModel;

    public QuestNote(QuestModel questModel, Rectangle bounds) : base(bounds, "")
    {
        this.QuestModel = questModel;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(this.QuestModel.Pad, this.bounds, this.QuestModel.PadSource, this.QuestModel.PadColor);
        spriteBatch.Draw(this.QuestModel.Pin, this.bounds, this.QuestModel.PinSource, this.QuestModel.PinColor);
        spriteBatch.Draw(
            this.QuestModel.Icon,
            new Vector2(this.bounds.X + this.QuestModel.IconOffset.X, this.bounds.Y + this.QuestModel.IconOffset.Y),
            this.QuestModel.IconSource,
            this.QuestModel.IconColor,
            0,
            Vector2.Zero,
            this.QuestModel.IconScale,
            SpriteEffects.None,
            0
        );
    }
}