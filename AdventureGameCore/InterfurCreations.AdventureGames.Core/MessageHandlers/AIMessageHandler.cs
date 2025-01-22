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
            var promptCommandText = _textParsing.CleanText(config.GameFunctions.FirstOrDefault(a => a.FunctionName == "AICommand").StartState.StateText).Trim();
            var postChoiceMsg = _textParsing.CleanText(config.GameFunctions.FirstOrDefault(a => a.FunctionName == "AICharacterChoiceMessage").StartState.StateText).Trim();
            var noChoiceMsg = _textParsing.CleanText(config.GameFunctions.FirstOrDefault(a => a.FunctionName == "AINoOptionMessage").StartState.StateText).Trim();
            if(message == "Fully Generated")
            {
                _aITextService.ClearMessagesForUser(player.PlayerId);
                var response = _aITextService.SendMessage(player.PlayerId, firstMsg);
                for(int i = 0; i < 5; i++)
                {
                    if (!response.Contains("OpenAI"))
                        break;
                    _aITextService.ClearMessagesForUser(player.PlayerId);
                     response = _aITextService.SendMessage(player.PlayerId, firstMsg);
                }
                var options = ParseOptions(response);
                options.Add(Messages.Return);

                return ExecutionResultHelper.SingleMessage(response, options);
            } else if(message == "Prompt Based") {
                _aITextService.ClearMessagesForUser(player.PlayerId);

                var options = new List<string>();
                var prompts = config.GameFunctions.Where(a => a.FunctionName.StartsWith("AIPrompt")).ToList();
                var promptNum = new Random().Next(prompts.Count);
                var selectedPrompt = prompts[promptNum];
                var promptName = selectedPrompt.FunctionName.Replace("AIPrompt_", "");
                var prompttxt = _textParsing.CleanText(selectedPrompt.StartState.StateText).Trim();
                _aITextService.AddSystemMessage(player.PlayerId, promptCommandText);
                _aITextService.SeedAssistantMessage(player.PlayerId, prompttxt);
                options.Add("1");
                options.Add("2");
                options.Add("3");
                options.Add("4");
                options.Add("Change Prompt");
                options.Add(Messages.Return);
                return ExecutionResultHelper.SingleMessage($"{promptName} ({promptNum}/{prompts.Count})\n\n{prompttxt}", options);
            } else if(message == "Change Prompt")
            {
                _aITextService.ClearMessagesForUser(player.PlayerId);

                var options = new List<string>();
                var prompts = config.GameFunctions.Where(a => a.FunctionName.StartsWith("AIPrompt")).ToList();
                var promptNum = new Random().Next(prompts.Count);
                var selectedPrompt = prompts[promptNum];
                var promptName = selectedPrompt.FunctionName.Replace("AIPrompt_", "");
                var prompttxt = _textParsing.CleanText(selectedPrompt.StartState.StateText).Trim();
                _aITextService.AddSystemMessage(player.PlayerId, promptCommandText);
                _aITextService.SeedAssistantMessage(player.PlayerId, prompttxt);
                options.Add("1");
                options.Add("2");
                options.Add("3");
                options.Add("4");
                options.Add("Change Prompt");
                options.Add(Messages.Return);
                return ExecutionResultHelper.SingleMessage($"{promptName} ({promptNum + 1}/{prompts.Count})\n\n{prompttxt}", options);
            }
            else
            {
                var count = _aITextService.GetUserMessageCount(player.PlayerId);
                var messages= _aITextService.GetUserMessages(player.PlayerId);
                if (count == 2 && messages.FirstOrDefault() == firstMsg)
                {
                    var animalChoice = message + ". " + postChoiceMsg;
                    var response = _aITextService.SendMessage(player.PlayerId, animalChoice);
                    var options = ParseOptions(response);
                    if(options.Count == 0)
                    {
                        response = response + "\n" + _aITextService.SendMessage(player.PlayerId, "");
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
                        response = response + "\n" + _aITextService.SendMessage(player.PlayerId, noChoiceMsg);
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
                if (a.Trim().Length > 0 && int.TryParse(a.Trim().First().ToString(), out int num))
                    return true;
                else
                    return false;
            });

            var formattedOptions = numberedLines.Select(a => a.Substring(0, 1)).ToList();

            if(formattedOptions.Count == 0)
            {
                var optionedLines = lines.Where(a =>
                {
                    if(a.StartsWith("Option"))
                    {
                        return true;
                    }
                    return false;
                });

                int i = 0;
                formattedOptions = optionedLines.Select(a => (++i).ToString()).ToList();
            }

            return formattedOptions;
        }
    }
}
