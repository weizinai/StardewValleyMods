using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace weizinai.StardewValleyMod.PiCore.UI.Widget;

/// <summary>
/// 文本折行：把字符串按给定 <see cref="SpriteFont" /> 与最大像素宽折成多行，供 <see cref="Label" />
/// （MaxWidth 折行）与 <see cref="Tooltip" />（正文折行）共用同一契约。断行规则为票 02 原型在真实 SDV
/// 中验证过的规则（HITL 通过，spec 布局章直接引用）：
/// 中文/CJK 逐字断行、拉丁按空格断词、zh+en 混合在词界退让；收尾标点（。，、；等）粘到上一行，
/// 行首不出现这些标点；显式换行符 \n 是硬断点。每个候选行都用 <see cref="SpriteFont.MeasureString(string)" />
/// 实测（测量路径与绘制同源），因此测得宽 = 绘制宽。
/// </summary>
public static class TextWrap
{
    /// <summary>是否 CJK 宽字符（中文/假名/谚文及全角符号标点）。判定为 CJK 时按单字断行。</summary>
    private static bool IsCjk(char c)
    {
        return c is >= '\u2E80' and <= '\u9FFF'
            or >= '\uAC00' and <= '\uD7AF'
            or >= '\uF900' and <= '\uFAFF'
            or >= '\uFF00' and <= '\uFFEF';
    }

    /// <summary>不该出现在行首的收尾标点（中文句读 + 成对闭合符 + 全半角标点；粘到上一行，避免行首出现这些标点）。</summary>
    private static bool IsLineStartForbidden(char c)
    {
        return c switch
        {
            '，' or '。' or '、' or '；' or '：' or '？' or '！' or '）' or '】' or '》' or '」' or '』'
                or '”' or '’' or '…' or ',' or '.' or ')' or ';' or ':' or '?' or '!' or ']' or '}'
                or '>' or '"' => true,
            _ => false
        };
    }

    /// <summary>按给定字体折行，返回不超过 <paramref name="maxWidth" /> 的各行；显式换行符 \n 保留为硬断点。</summary>
    /// <param name="font">测量用字体（与绘制同源）。</param>
    /// <param name="text">待折行文本（可含 \n 硬换行）。</param>
    /// <param name="maxWidth">每行最大像素宽。</param>
    /// <returns>折出的各行（不含换行符）。</returns>
    public static IReadOnlyList<string> Wrap(SpriteFont font, string text, float maxWidth)
    {
        var lines = new List<string>();

        if (string.IsNullOrEmpty(text) || maxWidth <= 0f)
        {
            return lines;
        }

        foreach (var hard in text.Split('\n'))
        {
            WrapSoft(font, hard, maxWidth, lines);
        }

        return lines;
    }

    /// <summary>把不含 \n 的一段文本折行，结果追加到 <paramref name="lines" />。</summary>
    /// <param name="font">测量用字体。</param>
    /// <param name="text">不含换行符的一段文本。</param>
    /// <param name="maxWidth">每行最大像素宽。</param>
    /// <param name="lines">结果行列表（追加）。</param>
    private static void WrapSoft(SpriteFont font, string text, float maxWidth, List<string> lines)
    {
        var tokens = Tokenize(text);
        var current = string.Empty;
        var pendingSpace = false;

        foreach (var token in tokens)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                if (current.Length > 0)
                {
                    pendingSpace = true;
                }

                continue;
            }

            // 收尾标点不能起新行：把它并进上一行（若存在；文本最开头无处可并时只能作行首）
            if (current.Length == 0 && token.Length == 1 && IsLineStartForbidden(token[0]))
            {
                if (lines.Count > 0)
                {
                    lines[^1] += token;
                }
                else
                {
                    current = token;
                }

                continue;
            }

            var sep = pendingSpace && current.Length > 0 ? " " : string.Empty;
            var candidate = current + sep + token;

            if (font.MeasureString(candidate).X <= maxWidth)
            {
                current = candidate;
                pendingSpace = false;

                continue;
            }

            if (current.Length > 0)
            {
                lines.Add(current);
                current = string.Empty;
                pendingSpace = false;
            }

            // token 单独成行（能放下就整词放）
            if (font.MeasureString(token).X <= maxWidth)
            {
                current = token;
            }
            else
            {
                // 单个超宽 token（多为超长 Latin/URL）：按测量拆开，每片自成一行
                lines.AddRange(SplitLongToken(font, token, maxWidth));
            }
        }

        if (current.Length > 0)
        {
            lines.Add(current);
        }
    }

    /// <summary>切成 token：空白段 / 单个 CJK 字符 / 非 CJK 连续词（拉丁词或符号串）。</summary>
    /// <param name="text">待切分文本。</param>
    /// <returns>token 列表。</returns>
    private static List<string> Tokenize(string text)
    {
        var tokens = new List<string>();
        var index = 0;

        while (index < text.Length)
        {
            var c = text[index];

            if (char.IsWhiteSpace(c))
            {
                var end = index;

                while (end < text.Length && char.IsWhiteSpace(text[end]))
                {
                    end++;
                }

                tokens.Add(text.Substring(index, end - index));
                index = end;
            }
            else if (IsCjk(c))
            {
                tokens.Add(c.ToString());
                index++;
            }
            else
            {
                var end = index;

                while (end < text.Length && !char.IsWhiteSpace(text[end]) && !IsCjk(text[end]))
                {
                    end++;
                }

                tokens.Add(text.Substring(index, end - index));
                index = end;
            }
        }

        return tokens;
    }

    /// <summary>把放不下的一整个 token 按测量宽度拆成若干不超过 <paramref name="maxWidth" /> 的片段。</summary>
    /// <param name="font">测量用字体。</param>
    /// <param name="token">超宽 token。</param>
    /// <param name="maxWidth">每行最大像素宽。</param>
    /// <returns>拆出的片段列表。</returns>
    private static List<string> SplitLongToken(SpriteFont font, string token, float maxWidth)
    {
        var pieces = new List<string>();
        var index = 0;

        while (index < token.Length)
        {
            var end = index + 1;

            while (end <= token.Length && font.MeasureString(token.Substring(index, end - index)).X <= maxWidth)
            {
                end++;
            }

            // end 指向首个超宽位置（或串尾+1）：取它前面一个位置；至少推进一个字
            var cut = Math.Max(index + 1, end - 1);
            pieces.Add(token.Substring(index, cut - index));
            index = cut;
        }

        return pieces;
    }
}
