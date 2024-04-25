using Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestMod.Framework;

public class TestUI
{
    private readonly RootElement ui = new();

    public TestUI()
    {
        var text = new Text("Hello, world!", new Vector2(100, 100));
        ui.AddChild(text);
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        ui.Draw(spriteBatch);
    }
    
    public void Update()
    {
        ui.Update();
    }
}