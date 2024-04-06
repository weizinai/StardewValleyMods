using StardewModdingAPI;
using StardewValley;

namespace Common;

public static class CommonHelper
{
    public static string GetDllPath(IModHelper helper, string dllName)
    {
        var directoryInfo = new DirectoryInfo(helper.DirectoryPath);
        while (directoryInfo.Parent != null)
        {
            if (directoryInfo.Name == "Mods") break;
            directoryInfo = directoryInfo.Parent;
        }

        var modsFolderPath = directoryInfo.FullName;
        var allFiles = Directory.GetFiles(modsFolderPath, dllName, SearchOption.AllDirectories);
        return allFiles[0];
    }
}