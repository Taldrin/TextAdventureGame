using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.GameLanguage;
using InterfurCreations.AdventureGames.Graph.Store;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Core.MessageHandlers
{
    public class AIMessageHandler : IMessageHandler
    {
        private readonly IAITextService _aITextService;
        private readonly IGameRetrieverService _gameRetrieverService;
        private readonly ITextParsing _textParsing;
        private readonly MainMenuMessageHandler _mainMenuMessageHandler;

        public int Priorioty => 10;

        public AIMessageHandler(IAITextService AITextService, IGameRetrieverService gameRetrieverService, IGameProcessor gameProcessor, ITextParsing textParsing)
        {
            _mainMenuMessageHandler = new MainMenuMessageHandler(gameRetrieverService, gameProcessor, textParsing);
            _textParsing = textParsing;
            _aITextService = AITextService;
            _gameRetrieverService = gameRetrieverService;
        }

        public List<string> GetOptions(Player player)
        {
            throw new NotImplementedException();
        }

        public ExecutionResult HandleMessage(string message, Player player)
        {
            if(message == Messages.Return)
            {
                player.PlayerFlag = PlayerFlag.MAIN_MENU.ToString();
                return _mainMenuMessageHandler.HandleMessage(message, player);
            }
            var config = _gameRetrieverService.ListGames(true).FirstOrDefault(a => a.GameName == "LiveConfiguration");
            var firstMsg = _textParsing.CleanText(config.GameFunctions.FirstOrDefault(a => a.FunctionName == "AIFirstMessage").StartState.StateText).Trim();
            var postChoiceMsg = _textParsing.CleanText(config.GameFunctions.FirstOrDefault(a => a.FunctionName == "AICharacterChoiceMessage").StartState.StateText).Trim();
            var noChoiceMsg = _textParsing.CleanText(config.GameFunctions.FirstOrDefault(a => a.FunctionName == "AINoOptionMessage").StartState.StateText).Trim();
            if(message == "ChatGPT 1")
            {
                _aITextService.ClearMessagesForUser(player.PlayerId);
                var response = _aITextService.SendMessage(player.PlayerId, firstMsg);
                var options = ParseOptions(response);
                options.Add(Messages.Return);

                return ExecutionResultHelper.SingleMessage(response, options);
            } else
            {
                var count = _aITextService.GetUserMessageCount(player.PlayerId);
                if(count == 2)
                {
                    var animalChoice = message + ". " + postChoiceMsg;
                    var response = _aITextService.SendMessage(player.PlayerId, animalChoice);
                    var options = ParseOptions(response);
                    if(options.Count == 0)
                    {
                        response = _aITextService.SendMessage(player.PlayerId, noChoiceMsg);
                        options = ParseOptions(response);
                    }
                    options.Add(Messages.Return);
                    return ExecutionResultHelper.SingleMessage(response, options);
                }
                else
                {
                    var response = _aITextService.SendMessage(player.PlayerId, message);
                    var options = ParseOptions(response);
                    if (options.Count == 0)
                    {
                        response = _aITextService.SendMessage(player.PlayerId, noChoiceMsg);
                        options = ParseOptions(response);
                    }
                    options.Add(Messages.Return);
                    return ExecutionResultHelper.SingleMessage(response, options);
                }
                return ExecutionResultHelper.SingleMessage("Empty", new List<string> { "Return" });
            }
        }

        public bool ShouldHandleMessage(string message, string gameState, string playerFlag)
        {
            if (playerFlag == PlayerFlag.AI_ADVENTURES.ToString())
            {
                return true;
            }
            return false;
        }

        public List<string> ParseOptions(string text)
        {
            var lines = text.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            var numberedLines = lines.Where(a =>
            {
                if (int.TryParse(a.Trim().First().ToString(), out int num))
                    return true;
                else
                    return false;
            });

            var formattedOptions = numberedLines.Select(a => a.Substring(0, 1)).ToList();

            return formattedOptions;
        }
    }
}
