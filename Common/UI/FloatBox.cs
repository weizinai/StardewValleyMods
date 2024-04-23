namespace Common.UI;

public class Floatbox : Textbox
{
    /*********
     ** Accessors
     *********/
    public float Value
    {
        get => float.TryParse(String, out float value) ? value : 0f;
        set => String = value.ToString();
    }

    public bool IsValid => float.TryParse(String, out _);


    /*********
     ** Protected methods
     *********/
    /// <inheritdoc />
    protected override void ReceiveInput(string str)
    {
        bool hasDot = String.Contains('.');
        bool valid = true;
        for (int i = 0; i < str.Length; ++i)
        {
            char c = str[i];
            if (!char.IsDigit(c) && !(c == '.' && !hasDot) && !(c == '-' && String == "" && i == 0))
            {
                valid = false;
                break;
            }
            if (c == '.')
                hasDot = true;
        }
        if (!valid)
            return;

        String += str;
        Callback?.Invoke(this);
    }
}