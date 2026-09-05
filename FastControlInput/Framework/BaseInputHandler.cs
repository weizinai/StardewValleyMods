namespace weizinai.StardewValleyMod.FastControlInput.Framework;

internal abstract class BaseInputHandler : IInputHandler
{
    protected readonly float Multiplier;

    protected BaseInputHandler(float multiplier)
    {
        this.Multiplier = multiplier;
    }

    public abstract bool IsEnable();

    public abstract void Update();
}