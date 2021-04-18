using DiscordRPC;

namespace DTCCUI
{
    internal class DiscordRPC1
    {
        public static DiscordRpcClient client;

        public static void Initialize()
        {
            client = new DiscordRpcClient("781066895029436456");
            client.Initialize();
            client.SetPresence(new RichPresence
            {
                State = "DTC AIO",
                Timestamps = Timestamps.Now,
                Assets = new Assets
                {
                    LargeImageKey = "DTC_logo",
                }
            });
        }
    }
}