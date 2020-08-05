using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.Services.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateToken(int length);
    }
}
