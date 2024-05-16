using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace HelpWanted.Framework;

internal class QuestNote : ClickableComponent
{
    private readonly QuestData questData;

    public QuestNote(QuestData questData, Rectangle bounds): base(bounds, "")
    {
        this.questData = questData;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(questData.PadTexture, bounds, questData.PadTextureSource, questData.PadColor);
        spriteBatch.Draw(questData.PinTexture, bounds, questData.PinTextureSource, questData.PinColor);
        spriteBatch.Draw(questData.Icon, new Vector2(bounds.X + questData.IconOffset.X, bounds.Y + questData.IconOffset.Y), questData.IconSource, questData.IconColor,
            0,Vector2.Zero,questData.IconScale,SpriteEffects.None,0);
    }
}