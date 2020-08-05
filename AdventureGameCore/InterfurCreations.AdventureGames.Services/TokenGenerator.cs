using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        public string GenerateToken(int length)
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            if (length < token.Length)
                return token.Substring(0, length);
            else
                return token;
        }
    }
}
