using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Monsters;
using weizinai.StardewValleyMod.MoreInfo.Framework;

namespace weizinai.StardewValleyMod.MoreInfo.Handler;

internal class MonsterInfoHandler : LocationInfoHandler
{
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
        this.LocationInfo.Clear();

        var monsters = Game1.currentLocation.characters.OfType<Monster>().ToArray();
        
        if (!monsters.Any()) return;

        foreach (var monster in monsters)
        {
            if (!this.LocationInfo.TryAdd(monster.displayName, 1))
            {
                this.LocationInfo[monster.displayName]++;
            }
        }

        var randomMonster = Game1.random.ChooseFrom(monsters);
        randomMonster.Sprite.UpdateSourceRect();
        this.Texture = randomMonster.Sprite.spriteTexture;
        this.SourceRectangle = randomMonster.Sprite.SourceRect;
    }
}