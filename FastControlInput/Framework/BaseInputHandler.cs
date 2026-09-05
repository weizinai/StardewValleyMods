namespace weizinai.StardewValleyMod.FastControlInput.Framework;

internal abstract class BaseInputHandler : IInputHandler
{
    protected readonly float multiplier;

    protected BaseInputHandler(float multiplier)
    {
        this.multiplier = multiplier;
    }

    public abstract bool IsEnable();

    public abstract void Update();
}
