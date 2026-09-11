using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Config;
using weizinai.StardewValleyMod.PiCore.UI.World;

namespace weizinai.StardewValleyMod.BetterCabin.UI;

/// <summary>
/// 小屋三个世界标签（主人名字 / 总在线时间 / 上次在线时间）的世界锚定内容：内部持有最多三个 <see cref="LabelBox" />，
/// 各自按配置偏移相对内容盒中心定位。宿主用 <c>Centered</c> 摆放，内容盒中心即建筑左上角 tile 的屏幕位置，
/// 因此每个标签盒的中心都是「建筑左上角 tile + 该标签的配置偏移」——与旧标签盒的摆放语义一致，偏移配置项含义零变化。
/// 文本只在签名变化时重建：总在线时间按分钟、在线态切换、名字 / 日期变化时才新建面板，稳态下不再每帧为每个小屋分配对象。
/// </summary>
internal sealed class CabinTagsContent : IAnchoredContent
{
    /// <summary>总在线时间的量化粒度：文本以分钟为最小变化单位（60000 毫秒）。</summary>
    private const ulong MillisecondsPerMinute = 60000;

    private readonly Cabin cabin;

    private LabelBox? nameBox;
    private string? nameText;
    private Color nameColor;

    private LabelBox? totalOnlineTimeBox;
    private int totalOnlineTimeMinutes;
    private Color totalOnlineTimeColor;

    private LabelBox? lastOnlineTimeBox;
    private int lastOnlineTimeDaysPlayed;
    private int lastOnlineTimeDisconnectDay;
    private Color lastOnlineTimeColor;

    /// <summary>构造小屋标签内容。</summary>
    /// <param name="cabin">标签所属小屋（提供主人名字、总在线时间与上次在线时间）。</param>
    public CabinTagsContent(Cabin cabin)
    {
        this.cabin = cabin;
    }

    /// <summary>主人名字标签的配置偏移（像素）。</summary>
    private Point NameOffset => new(ModConfig.Instance.NameTagXOffset, ModConfig.Instance.NameTagYOffset);

    /// <summary>总在线时间标签的配置偏移（像素）。</summary>
    private Point TotalOnlineTimeOffset => new(ModConfig.Instance.TotalOnlineTime.XOffset, ModConfig.Instance.TotalOnlineTime.YOffset);

    /// <summary>上次在线时间标签的配置偏移（像素）。</summary>
    private Point LastOnlineTimeOffset => new(ModConfig.Instance.LastOnlineTime.XOffset, ModConfig.Instance.LastOnlineTime.YOffset);

    /// <summary>
    /// 按当前配置与主人状态刷新三个标签盒：只在签名（可见性 / 变化键 / 颜色）变化时重建对应盒。
    /// 由对账器每 tick 调用；稳态下不做任何重建、不分配标签对象。
    /// </summary>
    public void Update()
    {
        this.UpdateNameBox();
        this.UpdateTotalOnlineTimeBox();
        this.UpdateLastOnlineTimeBox();
    }

    /// <inheritdoc />
    public Vector2 Measure()
    {
        var outer = new AnchorCover();

        if (this.nameBox is not null) outer.Include(this.nameBox, this.NameOffset);
        if (this.totalOnlineTimeBox is not null) outer.Include(this.totalOnlineTimeBox, this.TotalOnlineTimeOffset);
        if (this.lastOnlineTimeBox is not null) outer.Include(this.lastOnlineTimeBox, this.LastOnlineTimeOffset);

        return outer.Size;
    }

    /// <inheritdoc />
    public void Draw(SpriteBatch batch, Rectangle bounds)
    {
        // 宿主按 Centered 摆放内容盒，bounds.Center 即建筑左上角 tile 的屏幕位置（世界锚点）
        var anchor = bounds.Center;

        this.nameBox?.Draw(batch, anchor, this.NameOffset);
        this.totalOnlineTimeBox?.Draw(batch, anchor, this.TotalOnlineTimeOffset);
        this.lastOnlineTimeBox?.Draw(batch, anchor, this.LastOnlineTimeOffset);
    }

    /// <summary>
    /// 主接缝（纯几何，不引用任何游戏静态）：盒中心对准「锚点 + 偏移」时盒的屏幕矩形。
    /// 与旧标签盒的摆放数学逐式等价（<c>锚点 - 半盒 + 偏移</c>）。
    /// </summary>
    /// <param name="anchor">锚点屏幕坐标。</param>
    /// <param name="size">盒尺寸。</param>
    /// <param name="offset">相对锚点的像素偏移。</param>
    /// <returns>盒的屏幕矩形。</returns>
    private static Rectangle GetPanelBounds(Point anchor, Vector2 size, Point offset)
    {
        return new Rectangle(
            anchor.X + offset.X - (int)(size.X / 2f),
            anchor.Y + offset.Y - (int)(size.Y / 2f),
            (int)size.X,
            (int)size.Y
        );
    }

    /// <summary>刷新主人名字标签：自己 = 红，其他在线玩家 = 在线色，其他离线玩家 = 离线色。</summary>
    private void UpdateNameBox()
    {
        var config = ModConfig.Instance;

        if (!config.CabinOwnerNameTag)
        {
            this.nameBox = null;

            return;
        }

        var text = this.cabin.owner.Name;
        var color = this.GetOwnerNameColor(config);

        if (this.nameBox is not null && this.nameText == text && this.nameColor == color) return;

        this.nameText = text;
        this.nameColor = color;
        this.nameBox = new LabelBox(text, color);
    }

    /// <summary>刷新总在线时间标签：文本按分钟变化，分钟数或颜色变化时才重建。</summary>
    private void UpdateTotalOnlineTimeBox()
    {
        var onlineTime = ModConfig.Instance.TotalOnlineTime;

        if (!onlineTime.Enable)
        {
            this.totalOnlineTimeBox = null;

            return;
        }

        var millisecondsPlayed = this.cabin.owner.millisecondsPlayed;
        var minutes = (int)(millisecondsPlayed / MillisecondsPerMinute);
        var color = onlineTime.TextColor;

        if (this.totalOnlineTimeBox is not null && this.totalOnlineTimeMinutes == minutes && this.totalOnlineTimeColor == color) return;

        this.totalOnlineTimeMinutes = minutes;
        this.totalOnlineTimeColor = color;
        this.totalOnlineTimeBox = new LabelBox(Utility.getHoursMinutesStringFromMilliseconds(millisecondsPlayed), color);
    }

    /// <summary>刷新上次在线时间标签：仅主人离线时显示，离线天数或颜色变化时才重建。</summary>
    private void UpdateLastOnlineTimeBox()
    {
        var onlineTime = ModConfig.Instance.LastOnlineTime;
        var visible = onlineTime.Enable && !Game1.player.team.playerIsOnline(this.cabin.owner.UniqueMultiplayerID);

        if (!visible)
        {
            this.lastOnlineTimeBox = null;

            return;
        }

        var daysPlayed = (int)Game1.stats.DaysPlayed;
        var disconnectDay = this.cabin.owner.disconnectDay.Value;
        var color = onlineTime.TextColor;

        if (this.lastOnlineTimeBox is not null
            && this.lastOnlineTimeDaysPlayed == daysPlayed
            && this.lastOnlineTimeDisconnectDay == disconnectDay
            && this.lastOnlineTimeColor == color)
        {
            return;
        }

        this.lastOnlineTimeDaysPlayed = daysPlayed;
        this.lastOnlineTimeDisconnectDay = disconnectDay;
        this.lastOnlineTimeColor = color;
        this.lastOnlineTimeBox = new LabelBox(Utility.getDateString(-(daysPlayed - disconnectDay)), color);
    }

    /// <summary>名字标签的颜色：自己 / 在线 / 离线三色，语义与旧标签盒一致。</summary>
    /// <param name="config">当前配置。</param>
    /// <returns>名字文字颜色。</returns>
    private Color GetOwnerNameColor(ModConfig config)
    {
        if (Game1.player.Equals(this.cabin.owner)) return config.OwnerColor;

        return Game1.player.team.playerIsOnline(this.cabin.owner.UniqueMultiplayerID) ? config.OnlineFarmerColor : config.OfflineFarmerColor;
    }

    /// <summary>一个已建标签盒：面板 + 重建时量得的尺寸（Measure / Draw 共用，避免每帧重新测量文本）。</summary>
    private sealed class LabelBox
    {
        /// <summary>按文本与颜色建盒并量尺寸（<see cref="TilePanel" /> 没有清行 API，文本变化只能整盒重建）。</summary>
        /// <param name="text">标签文本。</param>
        /// <param name="color">文字颜色。</param>
        public LabelBox(string text, Color color)
        {
            this.Panel = new TilePanel().AddLine(text, null, color);
            this.Size = this.Panel.Measure();
        }

        /// <summary>扁平九宫格面板 + 扁平文字（画在世界批里）。</summary>
        public TilePanel Panel { get; }

        /// <summary>盒尺寸（重建时量得）。</summary>
        public Vector2 Size { get; }

        /// <summary>按「盒中心 = 锚点 + 偏移」把盒画进世界批。</summary>
        /// <param name="batch">精灵批（世界批坐标空间）。</param>
        /// <param name="anchor">世界锚点的屏幕坐标（内容盒中心）。</param>
        /// <param name="offset">盒中心相对锚点的配置偏移（像素）。</param>
        public void Draw(SpriteBatch batch, Point anchor, Point offset)
        {
            this.Panel.Draw(batch, GetPanelBounds(anchor, this.Size, offset));
        }
    }

    /// <summary>
    /// 各标签盒按偏移相对锚点排布后的覆盖范围累加器（纯几何，以锚点为原点）。
    /// 尺寸对锚点对称——宿主把测得尺寸居中摆到锚点上，只有对称取尺寸才能保证它的视口裁剪框覆盖全部标签盒；
    /// 若改成并集尺寸，偏移不对称时（如名字 0,0 与上次在线 0,+64）裁剪框会整体偏移，边上的标签会提前消失。
    /// </summary>
    private struct AnchorCover
    {
        private bool hasBox;
        private int minX;
        private int minY;
        private int maxX;
        private int maxY;

        /// <summary>覆盖尺寸（对锚点对称）；尚未并入任何盒时为零。</summary>
        public Vector2 Size => this.hasBox
            ? new Vector2(Math.Max(-this.minX, this.maxX) * 2, Math.Max(-this.minY, this.maxY) * 2)
            : Vector2.Zero;

        /// <summary>并入一个标签盒（盒中心 = 锚点 + 偏移）。</summary>
        /// <param name="box">标签盒（面板 + 尺寸）。</param>
        /// <param name="offset">盒中心相对锚点的偏移。</param>
        public void Include(LabelBox box, Point offset)
        {
            var bounds = GetPanelBounds(Point.Zero, box.Size, offset);

            if (!this.hasBox)
            {
                this.minX = bounds.Left;
                this.minY = bounds.Top;
                this.maxX = bounds.Right;
                this.maxY = bounds.Bottom;
                this.hasBox = true;

                return;
            }

            this.minX = Math.Min(this.minX, bounds.Left);
            this.minY = Math.Min(this.minY, bounds.Top);
            this.maxX = Math.Max(this.maxX, bounds.Right);
            this.maxY = Math.Max(this.maxY, bounds.Bottom);
        }
    }
}
