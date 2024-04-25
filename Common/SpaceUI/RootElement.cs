using StardewValley;

namespace Common.SpaceUI;

public class RootElement : Container
{
    public bool Obscured { get; set; } = false;

    public override int Width => Game1.viewport.Width;
    public override int Height => Game1.viewport.Height;
    
    public override void Update(bool isOffScreen = false)
    {
        base.Update(isOffScreen || Obscured);
        if (Dropdown.ActiveDropdown?.GetRoot() != this)
        {
            Dropdown.ActiveDropdown = null;
        }

        if (Dropdown.SinceDropdownWasActive > 0)
        {
            Dropdown.SinceDropdownWasActive--;
        }
    }
    
    public override RootElement GetRootImpl()
    {
        return this;
    }
}