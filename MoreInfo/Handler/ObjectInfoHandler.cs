using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoreInfo.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace MoreInfo.Handler;

internal class ObjectInfoHandler : BaseInfoHandler
{
    private readonly Dictionary<string, int> objectInfo = new();

    protected override string HoverText => this.GetStringFromDictionary(this.objectInfo);
    
    public ObjectInfoHandler()
    {
        this.UpdateObjectInfo();
    }

    public override void Init(IModEvents modEvents)
    {
        modEvents.Player.Warped += this.OnWarp;
        modEvents.World.ObjectListChanged += this.OnObjectListChanged;
    }

    public override void Clear(IModEvents modEvents)
    {
        modEvents.Player.Warped -= this.OnWarp;
        modEvents.World.ObjectListChanged -= this.OnObjectListChanged;
    }

    public override bool IsEnable()
    {
        return this.objectInfo.Any();
    }

    public override void Draw(SpriteBatch b)
    {
        IClickableMenu.drawTextureBox(b, this.Bound.X, this.Bound.Y, this.Bound.Width, this.Bound.Height, Color.White);
        b.Draw(this.Texture, new Rectangle((int)this.Position.X + 16, (int)this.Position.Y + 16, 32, 32), this.SourceRectangle, Color.White);
    }
    
    private void OnWarp(object? sender, WarpedEventArgs e)
    {
        this.UpdateObjectInfo();
    }

    private void OnObjectListChanged(object? sender, ObjectListChangedEventArgs e)
    {
        if (e.Location.Equals(Game1.currentLocation)) this.UpdateObjectInfo();
    }

    private void UpdateObjectInfo()
    {
        this.objectInfo.Clear();

        var objects = Game1.currentLocation.Objects.Values;

        if (!objects.Any()) return;

        foreach (var obj in objects)
        {
            if (!this.objectInfo.TryAdd(obj.DisplayName, 1))
            {
                this.objectInfo[obj.DisplayName]++;
            }
        }
        
        var data = ItemRegistry.GetDataOrErrorItem(objects.First().QualifiedItemId);
        this.Texture = data.GetTexture();
        this.SourceRectangle = data.GetSourceRect();
    }

}