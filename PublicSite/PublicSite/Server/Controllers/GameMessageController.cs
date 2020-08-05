using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Core.Objects;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PublicSite.Client.DataObjects;
using System;

namespace PublicSite.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameMessageController : ControllerBase
    {
        private IReporter _reporter;
        private IGameExecutor _gameExecutor;
        private IAccountController _accountController;
        private IPlayerDatabaseController _playerDatabaseController;
        private ITokenGenerator _tokenGenerator;
        private IHttpContextAccessor _contextAccessor;

        public GameMessageController(IGameExecutor gameExecutor, IReporter reporter, IPlayerDatabaseController playerDatabaseController, 
            IAccountController accountController, ITokenGenerator tokenGenerator, IHttpContextAccessor contextAccessor)
        {
            _reporter = reporter;
            _gameExecutor = gameExecutor;
            _accountController = accountController;
            _playerDatabaseController = playerDatabaseController;
            _tokenGenerator = tokenGenerator;
            _contextAccessor = contextAccessor;
        }

        [HttpPost]
        [Route("message")]
        public ExecutionResult GameMessageRecieved([FromBody] MessageSendDataObject dto)
        {
            try
            {
                var playerState = _playerDatabaseController.GetPlayerByWebKey(dto.player_code);

                if (playerState == null)
                {
                    throw new Exception("Could not find account");
                }
                return _gameExecutor.ProcessNewMessage(dto.message, new PlayerState { player = playerState });
            } catch(Exception e)
            {
                _reporter.ReportError($"Exception in web app controller: {e.Message} \n\n{e.StackTrace} \n\nHandling message: {dto.message}");
                throw e;
            }
        }

        [HttpPost]
        [Route("getWebAccount")]
        public WebPlayerDataObject GetWebAccount([FromBody] string playerKey)
        {
            try
            {
                var player = _playerDatabaseController.GetPlayerByWebKey(playerKey);
                return new WebPlayerDataObject { AccessKey = playerKey, PlayerId = player.PlayerId, Name = player.Name };
            }
            catch (Exception e)
            {
                _reporter.ReportError($"Exception in web app controller: {e.Message} \n\n{e.StackTrace}");
                throw e;
            }
        }

        [HttpGet]
        [Route("createWebAccount")]
        public WebPlayerDataObject CreateWebAccount([FromQuery] string name)
        {
            try
            {
                var token = _tokenGenerator.GenerateToken(10);
                var player = _accountController.CreateNewWebAccount(token, name);
                _reporter.ReportMessage($"New Web account created for user with name: {name}");
                return new WebPlayerDataObject { AccessKey = token, PlayerId = player.PlayerId, Name = player.Name };
            }
            catch (Exception e)
            {
                _reporter.ReportError($"Exception in web app controller: {e.Message} \n\n{e.StackTrace}");
                throw e;
            }
        }
    }
}
