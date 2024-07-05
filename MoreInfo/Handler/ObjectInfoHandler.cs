using System.Text;
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
    
    private Rectangle sourceRectangle;
    private Texture2D texture = null!;

    public ObjectInfoHandler()
    {
        this.UpdateObjectInfo();
    }

    private Rectangle Bound => new((int)this.Position.X, (int)this.Position.Y, 64, 64);

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
        b.Draw(this.texture, new Rectangle((int)this.Position.X + 16, (int)this.Position.Y + 16, 32, 32), this.sourceRectangle, Color.White,
            0f, Vector2.Zero, SpriteEffects.None, 0f);
    }

    public override void UpdateHover(Vector2 mousePosition)
    {
        this.IsHover = this.Bound.Contains(mousePosition);
    }

    public override void DrawHoverText(SpriteBatch b)
    {
        if (this.IsHover) IClickableMenu.drawHoverText(b, this.GetStringFromDictionary(), Game1.smallFont);
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

        var data = ItemRegistry.GetData(objects.FirstOrDefault()?.QualifiedItemId);
        this.texture = data.GetTexture();
        this.sourceRectangle = data.GetSourceRect();
    }

    private string GetStringFromDictionary()
    {
        var stringBuilder = new StringBuilder();
        foreach (var (key, value) in this.objectInfo)
            stringBuilder.AppendLine($"{key}: {value}");
        stringBuilder.Length--;

        return stringBuilder.ToString();
    }
}