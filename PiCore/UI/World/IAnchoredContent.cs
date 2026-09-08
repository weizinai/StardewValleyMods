using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace weizinai.StardewValleyMod.PiCore.UI.World;

/// <summary>
/// 世界锚定只读 immediate 内容契约（form C）：世界锚定宿主每帧读取锚点，经 <see cref="PositionHelper" />
/// 做世界→屏幕变换、再对内容盒显式视口裁剪后，把最终屏幕矩形交给本内容测量与绘制。只读 immediate：
/// 无 retained 布局、无焦点、无输入（交互世界 UI 不在 v1）。消费方也可在任意 RenderedWorld 处理器里
/// 直接调用 <see cref="Measure" /> / <see cref="Draw" /> 自绘。
/// </summary>
public interface IAnchoredContent
{
    /// <summary>测量内容尺寸（屏幕像素）。宿主在裁剪与摆放前调用，应反映当前内容。</summary>
    public Vector2 Measure();

    /// <summary>在 <paramref name="bounds" />（内容盒最终屏幕矩形，左上角即盒左上角）内绘制内容。</summary>
    /// <param name="batch">精灵批（RenderedWorld 触发时已 open，世界批坐标空间）。</param>
    /// <param name="bounds">内容盒的屏幕矩形。</param>
    public void Draw(SpriteBatch batch, Rectangle bounds);
}
