using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InterfurCreations.AdventureGames.DatabaseServices
{
    public class AccessTokenService : IAccessTokenService
    {
        private readonly DatabaseContext _context;
        private readonly ITokenGenerator _tokenGenerator;

        public AccessTokenService(IDatabaseContextProvider contextProvider, ITokenGenerator tokenGenerator)
        {
            _context = contextProvider.GetContext();
            _tokenGenerator = tokenGenerator;
        }

        public AccessToken CreateToken(int minHoursForRefresh, string tokenType)
        {
            string token = _tokenGenerator.GenerateToken(8);
            var addedToken = _context.AccessToken.Add(new AccessToken
            {
                HoursForRefresh = minHoursForRefresh,
                LastActivated = DateTime.MinValue,
                TokenType = tokenType,
                Token = token
            });
            _context.SaveChanges();

            return addedToken.Entity;
        }

        public List<AccessToken> ListTokens()
        {
            return _context.AccessToken.Include(a => a.Player).ThenInclude(a => a.TelegramPlayer).
                Include(a => a.Player).ThenInclude(a => a.KikPlayer).Include(a => a.Player).ThenInclude(a => a.DiscordPlayer).ToList();
        }

        public void DeleteToken(int id)
        {
            var token = _context.AccessToken.SingleOrDefault(a => a.Id == id);
            if (token == null)
                throw new Exception("Could not find token with ID: " + id);
            _context.AccessToken.Remove(token);
            _context.SaveChanges();
        }

        public AccessToken GetByToken(string token)
        {
            var accessToken = _context.AccessToken.SingleOrDefault(a => a.Token == token);
            return accessToken;
        }
    }
}
