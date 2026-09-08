using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace weizinai.StardewValleyMod.PiCore.UI;

/// <summary>
/// 薄主题对象：把 SDV 默认观感（九宫格矩形、关闭按钮矩形、字体、配色、交互音效）与常用绘制助手收编到一处，
/// 供框架与消费模组使用，避免“常量散落 + 每处重写绘制”。默认观感为 SDV 原生形状 + 扁平外观：
/// 面板与文字均不绘制投影（drawShadow:false / 无文字阴影）。
/// </summary>
public static class Theme
{
    /// <summary>悬停音效名。</summary>
    public const string HoverSound = "Cowboy_gunshot";

    /// <summary>切焦/导航音效名。</summary>
    public const string NavigateSound = "toolSwap";

    /// <summary>确认/接受音效名。</summary>
    public const string AcceptSound = "newArtifact";

    /// <summary>取消/关闭音效名。</summary>
    public const string CancelSound = "cancel";

    /// <summary>标准菜单盒 3×3 九宫格区域（真实世界中散落在多个模组文件里的同一块 rect）。</summary>
    public static readonly Rectangle MenuBoxSourceRect = new(0, 256, 60, 60);

    /// <summary>关闭按钮源区域（mouseCursors 上 (337,494,12,12)，绘制时放大 4 倍）。</summary>
    public static readonly Rectangle CloseSourceRect = new(337, 494, 12, 12);

    /// <summary>菜单打开时的背景压暗色（半透明黑）。</summary>
    public static readonly Color OverlayDim = Color.Black * 0.75f;

    /// <summary>菜单九宫格贴图（经典 SDV 菜单盒）。</summary>
    public static Texture2D MenuTexture => Game1.menuTexture;

    /// <summary>光标/箭头贴图。</summary>
    public static Texture2D MouseCursors => Game1.mouseCursors;

    /// <summary>正文小字体（与游戏语言一致，测量与绘制同源）。</summary>
    public static SpriteFont SmallFont => Game1.smallFont;

    /// <summary>对话框字体。</summary>
    public static SpriteFont DialogueFont => Game1.dialogueFont;

    /// <summary>默认文字颜色。</summary>
    public static Color TextColor => Game1.textColor;

    /// <summary>用标准菜单九宫格绘制扁平面板（无投影，HITL 扁平化决议）。</summary>
    /// <param name="batch">精灵批。</param>
    /// <param name="x">面板左上角 X。</param>
    /// <param name="y">面板左上角 Y。</param>
    /// <param name="width">面板宽度。</param>
    /// <param name="height">面板高度。</param>
    /// <param name="color">面板色调，默认白色。</param>
    public static void DrawPanel(SpriteBatch batch, int x, int y, int width, int height, Color? color = null)
    {
        IClickableMenu.drawTextureBox(batch, MenuTexture, MenuBoxSourceRect, x, y, width, height, color ?? Color.White, drawShadow: false);
    }

    /// <summary>绘制单行扁平文本（无投影，与 <see cref="SpriteFont.MeasureString(string)" /> 同源测量）。</summary>
    /// <param name="batch">精灵批。</param>
    /// <param name="text">文本内容。</param>
    /// <param name="position">文本左上角位置。</param>
    /// <param name="color">文本颜色，默认 <see cref="TextColor" />。</param>
    /// <param name="font">字体，默认 <see cref="SmallFont" />。</param>
    public static void DrawText(SpriteBatch batch, string text, Vector2 position, Color? color = null, SpriteFont? font = null)
    {
        batch.DrawString(font ?? SmallFont, text, position, color ?? TextColor);
    }

    /// <summary>在精灵批最上层重画鼠标光标。原生菜单在自身 draw 末尾才画光标，若有叠层在其后触发会盖住光标，需补画。</summary>
    /// <param name="batch">精灵批。</param>
    public static void DrawMouseCursor(SpriteBatch batch)
    {
        if (Game1.options.hardwareCursor)
        {
            return;
        }

        var cursor = Game1.options.snappyMenus && Game1.options.gamepadControls ? 44 : 0;
        batch.Draw(
            Game1.mouseCursors,
            new Vector2(Game1.getMouseX(), Game1.getMouseY()),
            Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, cursor, 16, 16),
            Color.White * Game1.mouseCursorTransparency,
            0f,
            Vector2.Zero,
            4f + Game1.dialogueButtonScale / 150f,
            SpriteEffects.None,
            1f
        );
    }

    /// <summary>播一个游戏内音效。</summary>
    /// <param name="soundName">音效名。</param>
    public static void PlaySound(string soundName)
    {
        Game1.playSound(soundName);
    }
}
