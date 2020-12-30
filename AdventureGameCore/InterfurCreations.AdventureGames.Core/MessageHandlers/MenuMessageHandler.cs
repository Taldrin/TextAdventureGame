using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.GameLanguage;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Graph.Store;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Core.MessageHandlers
{
    public class MenuMessageHandler : IMessageHandler
    {
        public int Priorioty => 5;

        private readonly MainMenuMessageHandler _mainMenuMessageHandler;
        private readonly IGameProcessor _gameProcessor;
        private readonly IGameStore _gameStore;
        private readonly ITextParsing _textParsing;
        private readonly IGameSaveService _gameSaveService;
        private readonly IConfigurationService _configService;
        private readonly IReporter _reporter;

        public MenuMessageHandler(IGameStore gameStore, IGameProcessor gameProcessor, ITextParsing textParsing, IGameSaveService gameSaveService,
            IConfigurationService configService, IReporter reporter)
        {
            _mainMenuMessageHandler = new MainMenuMessageHandler(gameStore, gameProcessor, textParsing);
            _gameStore = gameStore;
            _textParsing = textParsing;
            _gameProcessor = gameProcessor;
            _gameSaveService = gameSaveService;
            _configService = configService;
            _reporter = reporter;
        }

        public ExecutionResult HandleMessage(string message, Player player)
        {
            ExecutionResult executionResult = new ExecutionResult();
            List<MessageResult> messages = new List<MessageResult>();

            var activeGame = _gameStore.ListGames().FirstOrDefault(a => a.GameName == player.ActiveGameSave.GameName);
            var achievementList = new List<(bool hasAchieved, DrawAchievement achievement)>();
            if(activeGame != null)
            {
                try
                {
                    achievementList = AchievementService.HasPlayerDoneAchievements(activeGame, player);
                } catch (Exception e)
                {
                    _reporter.ReportError($"Error when finding achievements for player: {player?.Name}. Game was: {activeGame?.GameName}, player achievs: {player?.PermanentData?.Count}");
                }
            }

            if (message == Messages.ShowData)
            {
                var execResult = new ExecutionResult();
                execResult.MessagesToShow = new List<MessageResult>
                {
                    new MessageResult
                    {
                        Message = "- Showing current game data -"
                    },
                    new MessageResult
                    {
                        Message = string.Join("\n", player.ActiveGameSave.GameSaveData.Select(a => $"{a.Name}: [{a.Value}]"))
                    }
                };
                execResult.OptionsToShow = GetOptions(player);
                return execResult;
            } else if(message == Messages.RefreshGames)
            {
                messages.Add(new MessageResult { Message = "Starting to look for new games at: " + DateTime.UtcNow.ToLongTimeString() });
                _gameStore.ListGames(TimeSpan.FromSeconds(0));
                messages.Add(new MessageResult { Message = "Finished refreshing at: " + DateTime.UtcNow.ToLongTimeString() });
            }

            if (message == "Main Menu")
            {
                player.PlayerFlag = PlayerFlag.MAIN_MENU.ToString();
                return _mainMenuMessageHandler.HandleMessage(message, player);
            }
            if(message == "Return to Game")
            {
                return MessageHandlerHelpers.ReturnToGame(player, _gameStore, _textParsing, _gameProcessor);
            }
            if(message.StartsWith(Messages.Achievements))
            {
                var achievmentListString = achievementList.OrderBy(a => a.hasAchieved).Select(a => $"{(a.hasAchieved ? "UNLOCKED! " : "")}{a.achievement.Name} - {a.achievement.Description}").ToList();
                var responseMessage = string.Join("\n\n", achievmentListString);
                messages.Add(new MessageResult { Message = "Achievements for  game: " + activeGame.GameName });
                messages.Add(new MessageResult { Message = responseMessage });
            }

            messages.Add(new MessageResult
            {
                Message = "Returning to the main menu will reset your progress!"
            });

            messages.Add(new MessageResult
            {
                Message = "*** MENU ***"
            });

            executionResult.OptionsToShow = GetOptions(player);

            executionResult.MessagesToShow = messages;

            player.PlayerFlag = PlayerFlag.GAME_MENU.ToString();

            return executionResult;
        }

        public bool ShouldHandleMessage(string message, string gameState, string playerFlag)
        {
            return message.ToLower() == "-menu-" || playerFlag == PlayerFlag.GAME_MENU.ToString();
        }

        public List<string> GetOptions(Player player)
        {
            var activeGame = _gameStore.ListGames().FirstOrDefault(a => a.GameName == player.ActiveGameSave.GameName);
            var achievementList = AchievementService.HasPlayerDoneAchievements(activeGame, player);

            var achievementButtonText = $"{Messages.Achievements} ({achievementList.Count(a => a.hasAchieved)}/{achievementList.Count})";

            var list = new List<string>
            {
                "Return to Game",
                Messages.SaveGame,
                Messages.LoadGame,
                achievementButtonText,
                "Main Menu",

            };

            var testFeatures = _configService.GetConfigOrDefault("TestFeatures", "false", true);
            if(bool.TryParse(testFeatures, out var enableTestFeatures) && enableTestFeatures)
            {
                list.Add(Messages.ShowData);
                list.Add(Messages.RefreshGames);
            }

            return list;
        }
    }
}
