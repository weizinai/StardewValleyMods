using StardewValley;

namespace weizinai.StardewValleyMod.LazyMod.Framework;

internal static class ToolHelper
{
    private static HashSet<Tool> toolCache = new();

    public static void UpdateToolCache()
    {
        toolCache.Clear();

        toolCache = Game1.player.Items.Where(item => item is Tool).Cast<Tool>().ToHashSet();
    }

    public static T? FindToolFromInventory<T>() where T : Tool
    {
        return toolCache.FirstOrDefault(tool => tool is T) as T;
    }
}