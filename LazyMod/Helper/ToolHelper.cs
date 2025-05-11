using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace weizinai.StardewValleyMod.LazyMod.Helper;

public static class ToolHelper
{
    private static HashSet<Tool> toolCache = new();

    public static void UpdateToolCache()
    {
        toolCache.Clear();

        toolCache = Game1.player.Items.Where(item => item is Tool).Cast<Tool>().ToHashSet();
    }

    public static T? GetTool<T>(bool findToolFromInventory) where T : Tool
    {
        return findToolFromInventory ? toolCache.FirstOrDefault(tool => tool is T) as T : Game1.player.CurrentTool as T;
    }
}