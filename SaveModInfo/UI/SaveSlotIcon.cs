using Microsoft.Xna.Framework;
using weizinai.StardewValleyMod.SaveModInfo.Record;

namespace weizinai.StardewValleyMod.SaveModInfo.UI;

/// <summary>叠层要画在某个存档槽上的一枚图标：唯一键（存档文件夹名）、位置与差异结果。</summary>
/// <param name="Key">存档文件夹名（原版存档文件夹名与记录文件名同源，是稳定的唯一键）。</param>
/// <param name="Position">图块左上角在 UI 坐标的位置（已含按存档名文本宽度计算的偏移）。</param>
/// <param name="Diff">该存档的差异结果。</param>
internal record SaveSlotIcon(string Key, Vector2 Position, ModDiff Diff);
