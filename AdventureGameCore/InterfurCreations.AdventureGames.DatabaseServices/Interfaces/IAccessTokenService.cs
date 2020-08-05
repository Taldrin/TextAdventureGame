using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.DatabaseServices.Interfaces
{
    public interface IAccessTokenService
    {
        Database.AccessToken CreateToken(int minHoursForRefresh, string tokenType);
        void DeleteToken(int id);
        Database.AccessToken GetByToken(string token);
        List<Database.AccessToken> ListTokens();
    }
}
