using System.Collections.Generic;
using Microsoft.Xna.Framework;
using weizinai.StardewValleyMod.HelpWanted.UI;

namespace weizinai.StardewValleyMod.HelpWanted.Manager;

/// <summary>
/// 一块任务板（原版 / RSV）当天的待接任务与便签：待接便签及各自的摆放位置都归它持有，界面只向下读它、
/// 只通过它摘除。于是「有没有未接任务」这个问题不必反向依赖界面层。
/// </summary>
public interface IQuestManager
{
    /// <summary>当天已经上板、还没被接下的便签（含各自的摆放位置），顺序即上板顺序。</summary>
    public IReadOnlyList<QuestNote> PendingNotes { get; }

    /// <summary>当天是否还有没被接下的任务：含还没上板的与已上板但还没接下的。</summary>
    public bool HasPendingQuests { get; }

    /// <summary>把当天还没上板的待接任务摆到板面上；已经在板上的便签保持原有位置不动。</summary>
    /// <param name="boardBounds">板面矩形（屏幕坐标系），便签位置据此计算。</param>
    public void PlacePendingNotes(Rectangle boardBounds);

    /// <summary>任务被接下后，把它的便签从待接便签里摘掉。</summary>
    /// <param name="note">要摘掉的便签。</param>
    public void RemovePendingNote(QuestNote note);
}
