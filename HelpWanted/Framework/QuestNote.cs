using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace HelpWanted.Framework;

internal class QuestNote : ClickableComponent
{
    public readonly QuestData QuestData;

    public QuestNote(QuestData questData, Rectangle bounds): base(bounds, "")
    {
        QuestData = questData;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(QuestData.PadTexture, bounds, QuestData.PadTextureSource, QuestData.PadColor);
        spriteBatch.Draw(QuestData.PinTexture, bounds, QuestData.PinTextureSource, QuestData.PinColor);
        spriteBatch.Draw(QuestData.Icon, new Vector2(bounds.X + QuestData.IconOffset.X, bounds.Y + QuestData.IconOffset.Y), QuestData.IconSource, QuestData.IconColor,
            0,Vector2.Zero,QuestData.IconScale,SpriteEffects.None,0);
    }
}