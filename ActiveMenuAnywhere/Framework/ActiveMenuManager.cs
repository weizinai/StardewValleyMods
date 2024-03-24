using ActiveMenuAnywhere.Framework.ActiveMenu;
using StardewModdingAPI;

namespace ActiveMenuAnywhere.Framework;

public class ActiveMenuManager
{
    private readonly IModHelper helper;

    public ActiveMenuManager(IModHelper helper)
    {
        this.helper = helper;
    }
}