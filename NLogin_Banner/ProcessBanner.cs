using AdvancedBot;
using AdvancedBot.client;
using AdvancedBot.client.NBT;
using AdvancedBot.client.Packets;
using NLogin_Banner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NLogin_Banner
{
    public class ProcessBanner
    {
        public string text;
        public ProcessBanner(MinecraftClient client, String text)
        {
            this.text = text;
            this.client = client;
            Task.Run(async delegate ()
            {
                  try
                  {
                    Thread.Sleep(500);
                    if(client.OpenWindow != null)
                    {
                        Inventory inv = client.OpenWindow;
                        if (isValid(inv) && !running.Contains(client))
                        {
                            running.Add(client);
                            List<int> slots = getSlots();
                            if(slots.Count == 0)
                            {
                                running.Remove(client);
                                return;
                            }
                            String n = "";
                            foreach (int slot in slots)
                            {
                                n += slot+" ";
                            }
                            client.PrintToChat("§a[NLogin_Bypass] Slots: "+n);
                            foreach (int slot in slots)
                            {
                                client.OpenWindow.Click(client, (short)slot, false);
                                Thread.Sleep(1000);
                            }
                            client.OpenWindow.Click(client, (short)42, false);
                            client.PrintToChat("§a[NLogin_Bypass] Banner captcha passado.");
                            running.Remove(client);
                        }
                    }
                  }
                  catch (Exception ex2)
                  {
                     client.PrintToChat("§cOcorreu um erro no banner bypass: " + ex2.ToString());
                  }
             });
        }

        // Token: 0x04000006 RID: 6
        private MinecraftClient client;

        private List<int> getSlots()
        {
            List<int> list = new List<int>();
            char[] chars = text.ToCharArray();

            foreach(char c in chars)
            {
                for (int i = 0; i < client.OpenWindow.NumSlots; i++)
                {
                    if (i <= 20) continue;
                    ItemStack item = client.OpenWindow.Slots[i];
                    if (item != null && item.ID == Items.banner)
                    {
                        String bannerData = getData(item);
                        String numberData = getNumberData(c);
                        if (bannerData.Equals(numberData))
                        {
                            list.Add(i);
                            break;
                        }
                    }
                }
            }

            
            return list;
        }

        private String getNumberData(char c)
        {
            switch (c)
            {
                case '1': return Banners.um;
                case '2': return Banners.dois;
                case '3': return Banners.tres;
                case '4': return Banners.quatro;
                case '5': return Banners.cinco;
                case '6': return Banners.seis;
                case '7': return Banners.sete;
                case '8': return Banners.oito;
                case '9': return Banners.nove;
            }
            return "";
        }
        private static List<MinecraftClient> running = new List<MinecraftClient>();
        private String getData(ItemStack stack)
        {
            StringBuilder sb = new StringBuilder();
            var tag = stack.NBTData?.GetCompound("BlockEntityTag") ?? new CompoundTag();
            var patterns = tag.GetList("Patterns");

            for (int i = 0; i < Math.Min(16, patterns.Count); i++)
            {
                var pat = (CompoundTag)patterns[i];
                sb.Append(pat.GetString("Pattern"));
            }
            return sb.ToString();
        }

        private bool isValid(Inventory inventory)
        {
            foreach(ItemStack item in inventory.Slots)
            {
                if(item == null) { continue; }
                if (item.ID == Items.banner) return true;
            }
            return false;
        }
    }
}
