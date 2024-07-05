using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoreInfo.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Menus;
using StardewValley.Monsters;

namespace MoreInfo.Handler;

internal class MonsterInfoHandler : BaseInfoHandler
{
    private readonly Dictionary<string, int> monsterInfo = new();

    protected override string HoverText => this.GetStringFromDictionary(this.monsterInfo);

    public MonsterInfoHandler()
    {
        this.UpdateMonsterInfo();
    }

    public override void Init(IModEvents modEvents)
    {
        modEvents.Player.Warped += this.OnWarped;
        modEvents.World.NpcListChanged += this.OnNpcListChanged;
    }

    public override void Clear(IModEvents modEvents)
    {
        modEvents.Player.Warped -= this.OnWarped;
        modEvents.World.NpcListChanged -= this.OnNpcListChanged;
    }

    public override bool IsEnable()
    {
        return this.monsterInfo.Any();
    }

    public override void Draw(SpriteBatch b)
    {
        IClickableMenu.drawTextureBox(b, this.Bound.X, this.Bound.Y, this.Bound.Width, this.Bound.Height, Color.White);
        b.Draw(this.Texture, new Rectangle(this.Bound.X + 16, this.Bound.Y + 16, 32, 32), this.SourceRectangle, Color.White);
    }

    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        this.UpdateMonsterInfo();
    }

    private void OnNpcListChanged(object? sender, NpcListChangedEventArgs e)
    {
        if (e.Location.Equals(Game1.currentLocation)) this.UpdateMonsterInfo();
    }

    private void UpdateMonsterInfo()
    {
        this.monsterInfo.Clear();

        var monsters = Game1.currentLocation.characters.OfType<Monster>().ToArray();
        
        if (!monsters.Any()) return;

        foreach (var monster in monsters)
        {
            if (!this.monsterInfo.TryAdd(monster.displayName, 1))
            {
                this.monsterInfo[monster.displayName]++;
            }
        }

        var randomMonster = Game1.random.ChooseFrom(monsters);
        randomMonster.Sprite.UpdateSourceRect();
        this.Texture = randomMonster.Sprite.spriteTexture;
        this.SourceRectangle = randomMonster.Sprite.SourceRect;
    }
}