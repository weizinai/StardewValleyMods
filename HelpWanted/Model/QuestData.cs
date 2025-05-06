﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Quests;

namespace weizinai.StardewValleyMod.HelpWanted.Model;

public class QuestData
{
    public Texture2D Pad { get; }
    public Rectangle PadSource { get; }
    public Color PadColor { get; }
    public Texture2D Pin { get; }
    public Rectangle PinSource { get; }
    public Color PinColor { get; }
    public Texture2D Icon { get; }
    public Rectangle IconSource { get; }
    public Color IconColor { get; }
    public float IconScale { get; }
    public Point IconOffset { get; }
    public Quest Quest { get; }

    public QuestData(Texture2D pad, Rectangle padSource, Color padColor, Texture2D pin, Rectangle pinSource, Color pinColor,
        Texture2D icon, Rectangle iconSource, Color iconColor, float iconScale, Point iconOffset, Quest quest)
    {
        this.Pad = pad;
        this.PadSource = padSource;
        this.PadColor = padColor;
        this.Pin = pin;
        this.PinSource = pinSource;
        this.PinColor = pinColor;
        this.Icon = icon;
        this.IconSource = iconSource;
        this.IconColor = iconColor;
        this.IconScale = iconScale;
        this.IconOffset = iconOffset;
        this.Quest = quest;
    }
}