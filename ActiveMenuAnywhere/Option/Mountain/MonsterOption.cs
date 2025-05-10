using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class MonsterOption : BaseOption
{
    private readonly IModHelper helper;

    public MonsterOption(IModHelper helper)
        : base(I18n.UI_Option_Monster(), TextureManager.Instance.MountainTexture, GetSourceRectangle(2), OptionId.Monster)
    {
        this.helper = helper;
    }

    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("guildMember");
    }

    public override void Apply()
    {
        this.helper.Reflection.GetMethod(new AdventureGuild(), "showMonsterKillList").Invoke();
    }
}