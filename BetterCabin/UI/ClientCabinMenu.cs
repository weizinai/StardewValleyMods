using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using weizinai.StardewValleyMod.PiCore;
using weizinai.StardewValleyMod.PiCore.UI;
using weizinai.StardewValleyMod.PiCore.UI.Host;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace weizinai.StardewValleyMod.BetterCabin.UI;

/// <summary>
/// 客机小屋面板：PiCore.UI 菜单宿主（扁平观感，无投影），单页视图见 <see cref="CabinMenuView" />
/// （标题 → 建筑外观预览 → 「移动小屋」/「关闭」两个文本按钮）。
/// 窗口宽 760、高按建筑 4× 预览高自适应并以视口高度封顶；封顶后仍放不下时窗口居中、内容由视口裁切。
/// 面板分两态，共用同一个菜单（不关菜单、不引入叠层）：
/// <list type="bullet">
///     <item>正常态：框架 chrome + 根视图；关闭路径是右上角 X、Esc、手柄 B 与「关闭」文本按钮。</item>
///     <item>
///     移动态（点「移动小屋」进入）：视口冻结在小屋上、只画世界放置格与光标，鼠标左键拾起 / 放下小屋，
///     Esc 与手柄 B 结束移动并把玩家送回原位。移动态不画右上角 X（<see cref="shouldDrawCloseButton" />），
///     那块点击区靠「左键被吞掉、不链到 base」一并消掉。
///     </item>
/// </list>
/// 所有退出路径统一收尾在 <see cref="cleanupBeforeExit" />（<c>exitThisMenu</c> 是非虚方法，收尾只能挂它必经的这个钩子）。
/// </summary>
internal class ClientCabinMenu : MenuHost
{
    /// <summary>窗口宽度（容纳 4× 建筑预览与两侧箭头后仍有留白）。</summary>
    private const int WindowWidth = 760;

    /// <summary>窗口最小高度（预览较矮时也保证标题与动作区舒展）。</summary>
    private const int MinWindowHeight = 560;

    /// <summary>预览高之外留给标题、计数行、动作区与边框留白的高度。</summary>
    private const int WindowHeightReserve = 200;

    /// <summary>移动态平移视口的每帧像素步进。</summary>
    private const int PanSpeed = 32;

    /// <summary>移动态光标进入视口边缘多少像素内就开始平移。</summary>
    private const int PanThreshold = 64;

    /// <summary>放置格提示图块在 mouseCursors 上的起点 X（可放 / 不可放是相邻的两格）。</summary>
    private const int PlacementSheetX = 194;

    /// <summary>放置格提示图块在 mouseCursors 上的起点 Y。</summary>
    private const int PlacementSheetY = 388;

    /// <summary>放置格提示的单格边长（源图块尺寸，绘制时 4×）。</summary>
    private const int PlacementTileSize = 16;

    /// <summary>放置格提示的绘制倍率。</summary>
    private const int PlacementTileScale = 4;

    /// <summary>放置格提示的绘制层深（压在世界内容与建筑美术之上）。</summary>
    private const float PlacementTileDepth = 0.999f;

    /// <summary>可拾起高亮的透明度（原版木工菜单移动建筑时用同一档，<c>CarpenterMenu.cs:515,527</c>）。</summary>
    private const float HighlightAlpha = 0.8f;

    /// <summary>落位后第二声尘土的延迟（毫秒，照原版木屋移动：<c>CarpenterMenu.cs:1041</c>）。</summary>
    private const int SecondDirtSoundDelay = 50;

    /// <summary>落位后第三声尘土的延迟（毫秒，同前：<c>CarpenterMenu.cs:1042</c>）。</summary>
    private const int ThirdDirtSoundDelay = 150;

    private readonly Building building;
    private readonly CabinMenuView view;
    private readonly GameLocation originLocation;
    private readonly Location originViewport;

    /// <summary>已拾起、正跟着光标走的小屋；null = 还没拾起。</summary>
    private Building? buildingToMove;

    /// <summary>是否处于移动态。</summary>
    private bool isMoving;

    /// <summary>小屋所在的地点（移动态的放置目标，也是黑屏后接管显示的地点）。</summary>
    private GameLocation TargetLocation => this.building.GetParentLocation();

    /// <summary>构造面板。</summary>
    /// <param name="targetBuilding">要改外观的客机小屋。</param>
    public ClientCabinMenu(Building targetBuilding)
        : base(new CabinMenuView(targetBuilding), WindowWidth, ComputeWindowHeight(targetBuilding))
    {
        this.building = targetBuilding;
        this.view = (CabinMenuView)this.Root;

        // 视口与地点的「原位」在开面板时定下：移动态结束时要把玩家送回这里、相机也复原到这一帧
        this.originLocation = Game1.player.currentLocation;
        this.originViewport = Game1.viewport.Location;

        // 关闭走菜单自己的关闭路径（而不是直接 Game1.exitActiveMenu）：确认音只由宿主播一次，
        // 关闭音被 playSound: false 挡掉（否则与框架确认音叠成两声）
        this.view.CloseButton.OnClick = () => this.exitThisMenu(playSound: false);

        // 「移动小屋」进移动态：动的是宿主的视口与收尾流程，故点击动作由宿主接线
        this.view.MoveButton.OnClick = this.EnterMoveMode;
    }

    /// <summary>窗口高度：建筑 4× 预览高 + 其余留白，不低于最小高度，并以视口高度封顶。</summary>
    /// <param name="building">要改外观的建筑。</param>
    /// <returns>窗口高度（像素）。</returns>
    private static int ComputeWindowHeight(Building building)
    {
        var previewHeight = CabinPreview.MeasureArtworkHeight(building);
        var desired = Math.Max(MinWindowHeight, previewHeight + WindowHeightReserve);

        return Math.Min(desired, Game1.uiViewport.Height);
    }

    /// <summary>
    /// 移动态只画世界放置格与光标（跳过压暗背景、chrome 外框、根视图与提示框）；
    /// 正常态整块交给基类。
    /// </summary>
    /// <param name="batch">精灵批。</param>
    public override void draw(SpriteBatch batch)
    {
        if (!this.isMoving)
        {
            base.draw(batch);

            return;
        }

        // 放置格是世界坐标下的图块：菜单绘制时精灵批处于 UI 模式，须切进世界批再画，否则位置会偏
        Game1.StartWorldDrawInUI(batch);
        this.DrawPlacementTiles(batch);
        Game1.EndWorldDrawInUI(batch);

        // 菜单活动时游戏不代画光标
        Theme.DrawMouseCursor(batch);
    }

    /// <summary>
    /// 移动态照常走基类更新（手柄 B 关闭与脏布局冲刷都由它管），另加视口平移；
    /// 正常态只走基类。
    /// </summary>
    /// <param name="time">游戏时间。</param>
    public override void update(GameTime time)
    {
        base.update(time);

        if (this.isMoving)
        {
            PanScreenHelper.PanScreen(PanSpeed, PanThreshold);
        }
    }

    /// <summary>
    /// 移动态不路由按钮悬停（没有悬停音、没有提示框），只把光标下的小屋染绿提示「可以拾起」；
    /// 正常态交给基类。
    /// </summary>
    /// <param name="x">鼠标 X（UI 坐标）。</param>
    /// <param name="y">鼠标 Y（UI 坐标）。</param>
    public override void performHoverAction(int x, int y)
    {
        if (!this.isMoving)
        {
            base.performHoverAction(x, y);

            return;
        }

        var hovered = this.GetHoveredBuilding();

        // 只有本面板的这座小屋可拾起，其他建筑 / 空处一律恢复原色
        this.building.color = this.building.Equals(hovered) ? Color.Lime * HighlightAlpha : Color.White;
    }

    /// <summary>
    /// 移动态吞掉鼠标左键做拾起 / 放下（不链到 base，故右上角 X 那块点击区在移动态也不响应）；
    /// 正常态交给基类。
    /// </summary>
    /// <param name="x">鼠标 X（UI 坐标）。</param>
    /// <param name="y">鼠标 Y（UI 坐标）。</param>
    /// <param name="playSound">是否播放音效。</param>
    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (!this.isMoving)
        {
            base.receiveLeftClick(x, y, playSound);

            return;
        }

        if (this.buildingToMove is null)
        {
            this.PickUpBuilding();

            return;
        }

        this.PlaceBuilding();
    }

    /// <summary>
    /// 移动态不画右上角 X（退出靠 Esc 与手柄 B）。移动态的 <see cref="draw" /> 本就不走基类绘制，
    /// 这里再挡一层：只要还有任何路径走到基类绘制（如带色调的 draw 重载），X 都不会冒出来。
    /// </summary>
    /// <returns>正常态为 true；移动态为 false。</returns>
    public override bool shouldDrawCloseButton()
    {
        return !this.isMoving;
    }

    /// <summary>
    /// 统一收尾（唯一一处）：移动态退出时把玩家送回开面板前的位置、复原视口与 HUD，并还原面板按钮的点击动作。
    /// <c>IClickableMenu.exitThisMenu</c> 是非虚方法不能重写，而它同步调用本钩子且右上角 X、Esc、手柄 B、
    /// 显式「关闭」按钮四条路径都经过 <c>exitThisMenu</c>，故收尾挂在这里即可覆盖全部退出路径。
    /// 幂等兜底：只有本菜单仍是当前活动菜单、且没有别的 warp 在路上时才发起送回原位的 warp。
    /// </summary>
    protected override void cleanupBeforeExit()
    {
        if (!this.isMoving)
        {
            return;
        }

        // 先落地退出状态：即使下面的 warp 因兜底条件没发起，移动态也已经结束，
        // 不会留下「菜单关了但还停在移动态」的半截状态（顺带让迟到的黑屏回调变成空操作）
        this.isMoving = false;
        this.buildingToMove = null;

        // 高亮是移动态给小屋染的色，退出时还原，避免小屋留在绿色
        this.building.color = Color.White;
        this.view.AreButtonActionsEnabled = true;

        if (Game1.activeClickableMenu != this || Game1.locationRequest is not null)
        {
            // 兜底：别的 warp 在路上时不能再插一次 warp，但那趟 warp 的 OnWarp 不会替我们还原显示状态，
            // 而原地不动会留下隐藏的 HUD、被冻住的视口与指向小屋地点的 viewingLocation（玩家看着小屋、
            // 不能动也没有 HUD）——所以显示状态在这里同步还回去，不等 warp
            this.RestoreViewportAndHud();

            return;
        }

        var locationRequest = Game1.getLocationRequest(this.originLocation.NameOrUniqueName);
        locationRequest.OnWarp += this.RestoreViewportAndHud;
        Game1.warpFarmer(locationRequest, Game1.player.TilePoint.X, Game1.player.TilePoint.Y, Game1.player.FacingDirection);
    }

    /// <summary>还原移动态改过的显示状态：放回 HUD 与玩家小人、取消「正在看某个地点」、解冻视口并把相机放回开面板前那一帧。</summary>
    private void RestoreViewportAndHud()
    {
        Game1.displayHUD = true;
        Game1.displayFarmer = true;
        Game1.player.viewingLocation.Value = null;
        Game1.viewportFreeze = false;
        Game1.viewport.Location = this.originViewport;
    }

    /// <summary>
    /// 进入移动态：把面板按钮的点击动作临时置空（手柄 A 在移动态既不重入、也不播确认音），
    /// 先黑屏再在小屋所在的场景接管视口（黑屏盖住镜头跳转，与旧面板的手感一致）。
    /// </summary>
    private void EnterMoveMode()
    {
        this.isMoving = true;
        this.view.AreButtonActionsEnabled = false;
        Game1.globalFadeToBlack(this.SetUpForBuildingPlacement);
    }

    /// <summary>
    /// 黑屏完成后的接管：把显示用的当前地点切到小屋所在地点、视口冻结在小屋上、收起 HUD 与玩家小人。
    /// 玩家可能在这次黑屏跑完之前就按 Esc / 手柄 B 退出（那时菜单已关、送回原位的 warp 已发起），
    /// 于是这个延迟回调会晚于退出才触发——必须在那种情况下变成空操作，否则会把视口重新冻在地图上，
    /// 把玩家卡在移动态里回不去。
    /// </summary>
    private void SetUpForBuildingPlacement()
    {
        if (!this.isMoving)
        {
            return;
        }

        Game1.currentLocation.cleanupBeforePlayerExit();
        Game1.currentLocation = this.TargetLocation;
        Game1.currentLocation.resetForPlayerEntry();
        Game1.globalFadeToClear();

        Game1.displayHUD = false;
        Game1.displayFarmer = false;

        Game1.player.viewingLocation.Value = this.TargetLocation.NameOrUniqueName;
        Game1.viewportFreeze = true;

        var position = PositionHelper.GetAbsolutePositionFromTilePosition(new Vector2(this.building.tileX.Value, this.building.tileY.Value));
        Game1.viewport.Location = new Location((int)position.X - Game1.viewport.Width / 2, (int)position.Y - Game1.viewport.Height / 2);
        Game1.panScreen(0, 0);
    }

    /// <summary>拾起光标下的小屋：只有本面板的这座小屋能被拾起，别处点了不响应。</summary>
    private void PickUpBuilding()
    {
        if (!this.building.Equals(this.GetHoveredBuilding()))
        {
            return;
        }

        this.buildingToMove = this.building;
        this.building.isMoving = true;
        Game1.playSound("axchop");
    }

    /// <summary>光标所在格子上的建筑（那个格子上没有建筑时为 null）。</summary>
    /// <returns>光标下的建筑。</returns>
    private Building? GetHoveredBuilding()
    {
        return this.TargetLocation.getBuildingAt(PositionHelper.GetTilePositionFromMousePosition());
    }

    /// <summary>把拾起的小屋放到光标所在的格子：放得下就落位（敲击 + 两下尘土），放不下只播取消音、小屋继续跟着光标。</summary>
    private void PlaceBuilding()
    {
        if (this.buildingToMove is not { } building)
        {
            return;
        }

        var tilePosition = PositionHelper.GetTilePositionFromMousePosition();

        if (!this.TargetLocation.buildStructure(building, tilePosition, Game1.player))
        {
            Game1.playSound("cancel");

            return;
        }

        building.isMoving = false;
        this.buildingToMove = null;
        Game1.playSound("axchop");
        DelayedAction.playSoundAfterDelay("dirtyHit", SecondDirtSoundDelay);
        DelayedAction.playSoundAfterDelay("dirtyHit", ThirdDirtSoundDelay);
    }

    /// <summary>
    /// 画「这次放置合不合法」的格子提示：可放 / 不可放用 mouseCursors 上相邻的两个图块区分
    /// （不可放时图块下标 +1，与旧面板逐式一致）。只在已拾起小屋时画。
    /// </summary>
    /// <param name="batch">精灵批（已在世界批坐标空间）。</param>
    private void DrawPlacementTiles(SpriteBatch batch)
    {
        if (this.buildingToMove is not { } building)
        {
            return;
        }

        var mouseTile = PositionHelper.GetTilePositionFromMousePosition();

        for (var y = 0; y < building.tilesHigh.Value; y++)
        {
            for (var x = 0; x < building.tilesWide.Value; x++)
            {
                var sheetIndex = building.getTileSheetIndexForStructurePlacementTile(x, y);
                var tilePosition = new Vector2(mouseTile.X + x, mouseTile.Y + y);

                if (!this.TargetLocation.isBuildable(tilePosition))
                {
                    sheetIndex++;
                }

                var sourceRect = new Rectangle(PlacementSheetX + sheetIndex * PlacementTileSize, PlacementSheetY, PlacementTileSize, PlacementTileSize);

                batch.Draw(
                    Game1.mouseCursors,
                    Game1.GlobalToLocal(Game1.viewport, tilePosition * 64f),
                    sourceRect,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    PlacementTileScale,
                    SpriteEffects.None,
                    PlacementTileDepth
                );
            }
        }
    }
}
