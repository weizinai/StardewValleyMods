using System.Runtime.InteropServices;

namespace BetterChineseInput.Framework;

internal static class InputCacheManager
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern void KeyboardEvent(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

    private const byte VK_BACK = 0x08; // Backspace 键的虚拟按键码
    private const int KEYEVENTF_KEYDOWN = 0x0001; // 表示按下按键
    private const int KEYEVENTF_KEYUP = 0x0002; // 表示松开按键

    public static void CacheClear()
    {
        for (var i = 0; i < 30; i++)
        {
            // 模拟按下Backspace键
            KeyboardEvent(VK_BACK, 0, KEYEVENTF_KEYDOWN, 0);
            // 模拟松开Backspace键
            KeyboardEvent(VK_BACK, 0, KEYEVENTF_KEYUP, 0);
        }
    }
}