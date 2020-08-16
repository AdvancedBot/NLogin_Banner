using AdvancedBot;
using AdvancedBot.client;
using AdvancedBot.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NLogin_Banner
{
    public class Main : IPlugin
    {

        public static Dictionary<MinecraftClient, long> connected = new Dictionary<MinecraftClient, long>();
        public static Dictionary<MinecraftClient, String> bannerProcess = new Dictionary<MinecraftClient, String>();

        public void onClientConnect(MinecraftClient client)
        {
            connected[client] = Utils.GetTimestamp();
            if (bannerProcess.ContainsKey(client))
            {
                bannerProcess.Remove(client);
            }
        }

        public void onReceiveChat(string chat, byte pos, MinecraftClient client)
        { }

        public void OnReceivePacket(ReadBuffer pkt, MinecraftClient client)
        {
            int id = pkt.ID;

            if (pkt.ID == 0x2D)
            {
                try
                {
                    if (bannerProcess.ContainsKey(client) && Utils.GetTimestamp() - connected[client] <= 15000)
                    {
                        string text = bannerProcess[client];
                        String value = Regex.Match(text, @"\d+").Value;
                        new ProcessBanner(client, value);
                        bannerProcess.Remove(client);
                    }

                }
                catch (Exception ex3)
                {
                }
            }

            if (pkt.ID == 0x45)
            {
                try
                {
                    int action = pkt.ReadVarInt();
                    if (action == 0 || action == 1)
                    {//title or subtitle
                        string json = pkt.ReadString();
                        string text = Utils.StripColorCodes(ChatParser.ParseJson(json));
                        bool isDigitPresent = text.Any(c => char.IsDigit(c));
                        if (isDigitPresent && Utils.GetTimestamp() - connected[client] <= 15000)
                        {
                            bannerProcess[client] = text;
                        }
                    }

                }
                catch (Exception ex3)
                {
                    Program.FrmMain.DebugConsole(ex3.ToString());
                }
            }

        }

        public void onSendChat(string chat, MinecraftClient client)
        { }

        public void OnSendPacket(IPacket packet, MinecraftClient client)
        { }

        public void Tick()
        { }

        public void Unload()
        { }

    }
}
