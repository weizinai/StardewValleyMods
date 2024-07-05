using MoreInfo.Framework;
using StardewModdingAPI.Events;
using StardewValley;

namespace MoreInfo.Handler;

internal class ObjectInfoHandler : LocationInfoHandler
{
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
        this.LocationInfo.Clear();

        var objects = Game1.currentLocation.Objects.Values;

        if (!objects.Any()) return;

        foreach (var obj in objects)
        {
            if (!this.LocationInfo.TryAdd(obj.DisplayName, 1))
            {
                this.LocationInfo[obj.DisplayName]++;
            }
        }
        
        var data = ItemRegistry.GetDataOrErrorItem(objects.First().QualifiedItemId);
        this.Texture = data.GetTexture();
        this.SourceRectangle = data.GetSourceRect();
    }

}