using System.Net;
using System.Net.Sockets;
using HarmonyLib;
using Lidgren.Network;
using StardewValley;
using StardewValley.Network;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.MultiHost.Patcher;

public class LidgrenServerPatcher : BasePatcher
{
    private static readonly NetPeerConfiguration Config = new("StardewValley");

    public LidgrenServerPatcher()
    {
        Config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
        Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
        Config.Port = 24642;
        Config.ConnectionTimeout = 30f;
        Config.PingInterval = 5f;
        Config.MaximumConnections = Game1.Multiplayer.playerLimit * 2;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<LidgrenServer>(nameof(LidgrenServer.initialize)),
            prefix: this.GetHarmonyMethod(nameof(InitializePrefix))
        );
    }

    private static bool InitializePrefix(LidgrenServer __instance)
    {
        if (IsPortOccupied(Config.Port))
        {
            Logger.Info($"{Config.Port}已被占用，将使用{24643}作为新的端口");
            Config.Port = 24643;
        }

        __instance.server = new NetServer(Config);
        __instance.server.Start();

        return false;
    }

    private static bool IsPortOccupied(int port)
    {
        using var socker = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        try
        {
            socker.Bind(new IPEndPoint(IPAddress.Any, port));
            return false;
        }
        catch (SocketException ex)
        {
            if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                return true;
            throw;
        }
    }
}