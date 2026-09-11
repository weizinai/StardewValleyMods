using System;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Buildings;
using weizinai.StardewValleyMod.PiCore.UI;
using weizinai.StardewValleyMod.PiCore.UI.Layout;
using weizinai.StardewValleyMod.PiCore.UI.Widget;
using static StardewValley.Menus.BuildingSkinMenu;

namespace weizinai.StardewValleyMod.BetterCabin.UI;

/// <summary>
/// 客机小屋面板的根视图：标题 → 建筑外观预览 → 「移动小屋」/「关闭」两个文本按钮。
/// 外观列表与当前外观由本视图持有：左右箭头循环切换，切换立即写回建筑并随即联机同步（所见即所得、无确认步骤，
/// 与旧面板的行为契约一致）。
/// 两个文本按钮只呈现内容、不决定自己做什么：「关闭」与「移动小屋」的点击动作都由宿主接线，前者要经过
/// 菜单自己的关闭路径，后者要动宿主的视口与收尾流程。
/// </summary>
internal class CabinMenuView : Stack
{
    /// <summary>动作按钮的格子高。</summary>
    private const float ActionCellHeight = 64f;

    /// <summary>动作按钮文本两侧留白（格子宽 = 最长文本宽 + 2× 该值，中英文下都不挤）。</summary>
    private const float ActionCellPadding = 36f;

    private readonly Building building;
    private readonly List<SkinEntry> skins = new();
    private readonly CabinPreview preview;
    private readonly ButtonActionSwitch actionSwitch;
    private SkinEntry currentSkin = null!;

    /// <summary>「关闭」按钮（点击动作由宿主接线，关闭要经过菜单自己的关闭路径）。</summary>
    public Button CloseButton { get; }

    /// <summary>「移动小屋」按钮（点击动作由宿主接线，进入移动态要动宿主的视口与收尾流程）。</summary>
    public Button MoveButton { get; }

    /// <summary>
    /// 动作开关：false 时把面板四个按钮（上一种外观 / 下一种外观 / 移动小屋 / 关闭）的点击动作一并临时置空，
    /// true 时原样还原。移动态靠它挡住手柄 A 的误触（移动态只认鼠标放置）。
    /// </summary>
    public bool AreButtonActionsEnabled
    {
        get => this.actionSwitch.IsEnabled;
        set => this.actionSwitch.IsEnabled = value;
    }

    /// <summary>构造根视图。</summary>
    /// <param name="building">面板要改外观的建筑。</param>
    public CabinMenuView(Building building)
        : base(Direction.Vertical, spacing: 8f)
    {
        this.building = building;
        this.CollectSkins();
        this.preview = new CabinPreview(building, this.ShowPreviousSkin, this.ShowNextSkin);
        this.MoveButton = new Button(I18n.UI_ClientCabinMenu_MoveCabin())
        {
            TooltipText = Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings")
        };
        this.CloseButton = new Button(I18n.UI_ClientCabinMenu_Close());

        // 四个按钮在树里分属两处（两个箭头在预览区内部），故在这里一次性收齐交给同一个开关
        this.actionSwitch = new ButtonActionSwitch(this.preview.PreviousButton, this.preview.NextButton, this.MoveButton, this.CloseButton);

        this.Add(new Label(I18n.UI_ClientCabinMenu_ChooseSkin(), center: true));
        this.Add(this.preview);
        this.Add(BuildActionArea(this.MoveButton, this.CloseButton));

        this.SetSkin(Math.Max(this.skins.FindIndex(skin => skin.Id == this.building.skinId.Value), 0));
    }

    /// <summary>收集可用外观：首项是默认外观（Id 为 null），其余按各自的条件查询过滤（与旧面板一致）。</summary>
    private void CollectSkins()
    {
        var index = 0;
        this.skins.Add(new SkinEntry(index++, null, string.Empty, string.Empty));

        var buildingData = this.building.GetData();

        if (buildingData.Skins is null)
        {
            return;
        }

        foreach (var skin in buildingData.Skins)
        {
            if (GameStateQuery.CheckConditions(skin.Condition, this.building.GetParentLocation()))
            {
                this.skins.Add(new SkinEntry(index++, skin));
            }
        }
    }

    /// <summary>动作区：<see cref="Grid" />（自带整块居中）里两个等宽文本按钮，格子宽按当前语言的文本宽定。</summary>
    /// <param name="moveButton">「移动小屋」按钮。</param>
    /// <param name="closeButton">「关闭」按钮。</param>
    /// <returns>承载两个按钮的网格。</returns>
    private static Element BuildActionArea(Button moveButton, Button closeButton)
    {
        var cellWidth = Math.Max(Theme.SmallFont.MeasureString(moveButton.Text).X, Theme.SmallFont.MeasureString(closeButton.Text).X)
                        + ActionCellPadding * 2;

        var grid = new Grid(2, 1, cellWidth, ActionCellHeight);
        grid.Add(moveButton);
        grid.Add(closeButton);

        return grid;
    }

    /// <summary>切到上一种外观（下标回绕）。</summary>
    private void ShowPreviousSkin()
    {
        this.SetSkin(this.currentSkin.Index - 1);
    }

    /// <summary>切到下一种外观（下标回绕）。</summary>
    private void ShowNextSkin()
    {
        this.SetSkin(this.currentSkin.Index + 1);
    }

    /// <summary>按下标循环切换外观（负数回绕到末尾）。</summary>
    /// <param name="index">目标外观下标（任意整数）。</param>
    private void SetSkin(int index)
    {
        index %= this.skins.Count;

        if (index < 0)
        {
            index = this.skins.Count + index;
        }

        this.SetSkin(this.skins[index]);
    }

    /// <summary>立即把外观写回建筑（所见即所得、随即联机同步）并刷新计数；换外观时颜色回默认值，与旧面板一致。</summary>
    /// <param name="skin">目标外观。</param>
    private void SetSkin(SkinEntry skin)
    {
        this.currentSkin = skin;

        if (this.building.skinId.Value != skin.Id)
        {
            this.building.skinId.Value = skin.Id;
            this.building.netBuildingPaintColor.Value.Color1Default.Value = true;
            this.building.netBuildingPaintColor.Value.Color2Default.Value = true;
            this.building.netBuildingPaintColor.Value.Color3Default.Value = true;
        }

        this.preview.SetCounter(this.currentSkin.Index, this.skins.Count);
    }
}
