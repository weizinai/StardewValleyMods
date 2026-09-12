using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace weizinai.StardewValleyMod.HelpWanted.Helper;

/// <summary>
/// 便签摆放的纯几何计算：只在板面矩形内随机取点、按重叠边界判定冲突，不引用任何游戏静态，
/// 配置值与随机源全部由调用方传入。
/// </summary>
public static class NotePlacement
{
    /// <summary>
    /// 摆放用的重叠边界：便签中心在某一方向上相距小于「便签尺寸 × 该方向边界」时视为冲突。
    /// </summary>
    /// <param name="X">X 方向的重叠边界。</param>
    /// <param name="Y">Y 方向的重叠边界。</param>
    public readonly record struct Boundary(float X, float Y)
    {
        /// <summary>
        /// 放宽一轮：两个方向都按固定的缩放比例缩小，缩到下限以下时归零，即允许完全重叠。
        /// 边界归零后任意取点都不冲突，所以逐轮放宽必然结束。
        /// </summary>
        public Boundary Relax()
        {
            return new Boundary(RelaxComponent(this.X), RelaxComponent(this.Y));
        }

        private static float RelaxComponent(float boundary)
        {
            var relaxed = boundary * RelaxationFactor;

            return relaxed < MinimumBoundary ? 0f : relaxed;
        }
    }

    /// <summary>
    /// 摆放结果。
    /// </summary>
    /// <param name="Bounds">每张便签的最终矩形，顺序与传入的便签尺寸一致。</param>
    /// <param name="Relaxed">是否为了放下全部便签而放宽过重叠边界。</param>
    public readonly record struct Result(IReadOnlyList<Rectangle> Bounds, bool Relaxed);

    // 每轮随机取点的尝试次数，一轮下来还有便签没放下就放宽一轮重叠边界
    private const int TriesPerRound = 1000;

    // 每轮放宽时重叠边界的缩放比例
    private const float RelaxationFactor = 0.5f;

    // 重叠边界缩到该值以下时直接归零（允许完全重叠）
    private const float MinimumBoundary = 0.01f;

    /// <summary>
    /// 为每张便签算出一个最终矩形，并保证每张便签都拿得到位置：一轮里每张还没上板的便签各随机取点至多
    /// <see cref="TriesPerRound"/> 次，一轮下来仍有没上板的就放宽一轮重叠边界再走一轮，直到允许完全重叠
    /// （此时任意取点都不冲突）。放宽按轮推进——一轮里所有还没上板的便签都用同一个边界试过才放宽一次，
    /// 因此不会因为某一张便签抽样失败就让后面的便签一起变挤。
    /// </summary>
    /// <param name="boardBounds">板面矩形，返回的矩形也在同一坐标系内。</param>
    /// <param name="noteSizes">每张便签的尺寸，顺序即结果顺序。</param>
    /// <param name="boundary">重叠边界初值。</param>
    /// <param name="occupied">已占位的位置集合，摆放结果不会与它冲突。</param>
    /// <param name="random">摆放专用的随机源，由调用方传入。</param>
    /// <returns>每张便签的最终矩形，以及是否放宽过重叠边界。</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="noteSizes"/>、<paramref name="occupied"/> 或 <paramref name="random"/> 为 <see langword="null"/>。
    /// </exception>
    public static Result PlaceAll(
        Rectangle boardBounds,
        IReadOnlyList<Point> noteSizes,
        Boundary boundary,
        IReadOnlyList<Rectangle> occupied,
        Random random)
    {
        ArgumentNullException.ThrowIfNull(noteSizes);
        ArgumentNullException.ThrowIfNull(occupied);
        ArgumentNullException.ThrowIfNull(random);

        var bounds = new Rectangle[noteSizes.Count];
        var placed = new List<Rectangle>(occupied);
        var relaxed = false;
        var unplaced = new List<int>(noteSizes.Count);

        for (var i = 0; i < noteSizes.Count; i++) unplaced.Add(i);

        while (unplaced.Count > 0)
        {
            var stillUnplaced = new List<int>();

            foreach (var index in unplaced)
            {
                if (TryFindFreeBounds(boardBounds, noteSizes[index], boundary, placed, random, out var freeBounds))
                {
                    placed.Add(freeBounds);
                    bounds[index] = freeBounds;
                }
                else
                {
                    stillUnplaced.Add(index);
                }
            }

            if (stillUnplaced.Count == 0) break;

            // 本轮没放下的便签留到下一轮，用放宽后的重叠边界继续试
            relaxed = true;
            boundary = boundary.Relax();
            unplaced = stillUnplaced;
        }

        return new Result(bounds, relaxed);
    }

    // 在板面矩形内随机取点，取满 TriesPerRound 次仍躲不开冲突时返回 false
    private static bool TryFindFreeBounds(
        Rectangle boardBounds,
        Point noteSize,
        Boundary boundary,
        IReadOnlyList<Rectangle> occupied,
        Random random,
        out Rectangle bounds)
    {
        // 便签装得下时，取点范围让整张便签落在板面内；比板面还大时，范围变成「整张便签始终盖住板面」的那一段，
        // 保证它仍能拿到位置、照旧随机散开，而不是静默消失
        var minX = Math.Min(boardBounds.X, boardBounds.Right - noteSize.X);
        var maxX = Math.Max(boardBounds.X, boardBounds.Right - noteSize.X);
        var minY = Math.Min(boardBounds.Y, boardBounds.Bottom - noteSize.Y);
        var maxY = Math.Max(boardBounds.Y, boardBounds.Bottom - noteSize.Y);

        for (var tries = 0; tries < TriesPerRound; tries++)
        {
            var candidate = new Rectangle(
                random.Next(minX, maxX),
                random.Next(minY, maxY),
                noteSize.X,
                noteSize.Y
            );

            if (IsOverlapping(candidate, occupied, boundary)) continue;

            bounds = candidate;

            return true;
        }

        bounds = default;

        return false;
    }

    private static bool IsOverlapping(Rectangle candidate, IReadOnlyList<Rectangle> occupied, Boundary boundary)
    {
        foreach (var other in occupied)
        {
            var offsetX = Math.Abs(other.Center.X - candidate.Center.X);
            var offsetY = Math.Abs(other.Center.Y - candidate.Center.Y);

            if (offsetX < candidate.Width * boundary.X || offsetY < candidate.Height * boundary.Y) return true;
        }

        return false;
    }
}
