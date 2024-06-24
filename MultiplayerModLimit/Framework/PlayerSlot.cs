namespace weizinai.StardewValleyMod.MultiplayerModLimit.Framework;

internal class PlayerSlot
{
    public readonly long ID;
    public int Cooldown;

    public PlayerSlot(long id, int cooldown)
    {
        this.ID = id;
        this.Cooldown = cooldown;
    }
}