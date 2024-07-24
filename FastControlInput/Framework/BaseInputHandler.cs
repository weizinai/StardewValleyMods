namespace weizinai.StardewValleyMod.FastControlInput.Framework;

internal abstract class BaseInputHandler : IInputHandler
{
    private float remainder;
    private readonly float multiplier;

    protected BaseInputHandler(float multiplier)
    {
        this.multiplier = multiplier;
    }

    public abstract bool IsEnable();

    public abstract void Update();

    protected void ApplySkips(Action run)
    {
        var skips = this.GetSkipsThisTick();

        for (var i = 0; i < skips; i++) run.Invoke();
    }

    private int GetSkipsThisTick()
    {
        if (this.multiplier <= 1) return 0;

        var skips = this.multiplier + this.remainder - 1;
        this.remainder = skips % 1;
        return (int)skips;
    }
}