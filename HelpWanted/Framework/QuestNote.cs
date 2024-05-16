using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
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
        
    }
}