namespace weizinai.StardewValleyMod.FastControlInput.Framework;

internal interface IInputHandler
{
    public bool IsEnable();

    public void Update();
}