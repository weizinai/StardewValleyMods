namespace weizinai.StardewValleyMod.MultiplayerModLimit.Framework;

internal class PlayerSlot
{
    public readonly long Id;
    public int Cooldown;

    public PlayerSlot(long id, int cooldown)
    {
        this.Id = id;
        this.Cooldown = cooldown;
    }
}