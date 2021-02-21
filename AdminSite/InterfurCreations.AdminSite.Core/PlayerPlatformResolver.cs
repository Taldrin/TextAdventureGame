using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices;
using InterfurCreations.AdventureGames.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdminSite.Core
{
    public static class PlayerPlatformResolver
    {
        public static PlatformType ResolvePlatformFromPlayer(PlayerListModel player)
        {
            if (player == null)
                return PlatformType.NONE;
            if(player.DiscordPlayer != null)
                return PlatformType.Discord;
            if(player.TelegramPlayer != null)
                return PlatformType.Telegram;
            if(player.KikPlayer != null)
                return PlatformType.Kik;
            if(player.WebPlayer != null)
                return PlatformType.Web;
            return PlatformType.NONE;
        }

        public static PlatformType ResolvePlatformFromPlayer(Player player)
        {
            if (player == null)
                return PlatformType.NONE;
            if(player.DiscordPlayer != null)
                return PlatformType.Discord;
            if(player.TelegramPlayer != null)
                return PlatformType.Telegram;
            if(player.KikPlayer != null)
                return PlatformType.Kik;
            if(player.WebPlayer != null)
                return PlatformType.Web;
            return PlatformType.NONE;
        }
    }
}
