using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.DatabaseServices

{
    public class AccessService : IAccessService
    {
        private readonly IAccessTokenService _tokenService;
        private readonly IConfigurationService _configService;

        public AccessService(IAccessTokenService tokenService, IConfigurationService configService)
        {
            _tokenService = tokenService;
            _configService = configService;
        }

        public bool DoesPlayerHaveAccess(Player player)
        {
            var accessRequired = _configService.GetConfigOrDefault(ConfigEntries.AccessRequired.ToString(), "None");
            if (accessRequired == "None") return true;
            else
            {
                if (player.AccessTokens.Count == 0)
                    return false;
                foreach (var a in player.AccessTokens)
                {
                    if (a.TokenType == "AlphaAccess")
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        
        public (bool result, string reason) TryGrantAccess(Player player, string accessToken)
        {
            var token = _tokenService.GetByToken(accessToken);
            if (token == null)
                return (false, "Invalid token");
            if(token.LastActivated > DateTime.UtcNow.Subtract(TimeSpan.FromHours(token.HoursForRefresh)))
                return (false, "Cannot reactivate this code that frequently. Try again at: " + token.LastActivated.Add(TimeSpan.FromHours(token.HoursForRefresh)).ToString("ddd, dd MMM HH:mm:ss UTC"));
            if (token != null)
            {
                token.LastActivated = DateTime.UtcNow;
                player.AccessTokens.Add(token);
                return (true, "");
            }
            return (false, "Invalid token");
        }
    }
}
