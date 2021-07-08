using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.GameLanguage;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Graph.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfurCreations.AdventureGames.Core.MessageHandlers
{
    public class AchievementMessageHandler : IMessageHandler
    {
        public int Priorioty => 10;

        private readonly IGameRetrieverService _gameStore;
        private readonly IGameProcessor _gameProcessor;
        private readonly ITextParsing _textParsing;
        private readonly IStatisticsService _statService;

        public AchievementMessageHandler(IGameRetrieverService gameStore, IGameProcessor gameProcessor, ITextParsing textParsing,
            IStatisticsService statService)
        {
            _gameStore = gameStore;
            _gameProcessor = gameProcessor;
            _textParsing = textParsing;
            _statService = statService;
        }

        public List<string> GetOptions(Player player)
        {
            return HandleMessage(Messages.Achievements, player).OptionsToShow;
        }

        public ExecutionResult HandleMessage(string message, Player player)
        {
            if(message == Messages.Return)
                return MessageHandlerHelpers.ReturnToMainMenu(player);

            var games = _gameStore.ListGames();

            List<string> optionsToSend = new List<string>();
            optionsToSend.Add(Messages.Return);

            var gameAchievementList = games.Select(a => (AchievementService.HasPlayerDoneAchievements(a, player), a.GameName)).ToList();
            gameAchievementList.ForEach(a =>
            {
                optionsToSend.Add($"{a.GameName} ({a.Item1.Count(b => b.hasAchieved)}/{a.Item1.Count()})");
            });

            var selectedGame = games.FirstOrDefault(a => message.StartsWith(a.GameName));

            if(selectedGame != null)
            {
                var achievmentListString = AchievementService.HasPlayerDoneAchievements(selectedGame, player).OrderBy(a => a.hasAchieved).Select(a => $"{(a.hasAchieved ? "UNLOCKED! " : "")}{a.achievement.Name} - {a.achievement.Description} {AchievementService.GetPercentageAchieved(_statService, selectedGame, a.achievement.Name)}").ToList();
                var responseMessage = string.Join("\n\n", achievmentListString);
                var result = ExecutionResultHelper.SingleMessage(responseMessage, optionsToSend);
                result.MessagesToShow.Insert(0, new MessageResult
                {
                    Message = "Achievements for game: " + selectedGame.GameName
                });
                return result;
            }

            else
            {
                player.PlayerFlag = PlayerFlag.ACHIEVEMENTS.ToString();
                var messageToSend = $"You have completed a total of: {gameAchievementList.Sum(a => a.Item1.Count(b => b.hasAchieved))} out of {gameAchievementList.Sum(a => a.Item1.Count())}";

                return new ExecutionResult
                {
                    MessagesToShow = new List<MessageResult> { new MessageResult { Message = messageToSend } },
                    OptionsToShow = optionsToSend
                };
            }
        }

        public bool ShouldHandleMessage(string message, string gameState, string playerFlag)
        {
            if((message.StartsWith(Messages.Achievements) && playerFlag == PlayerFlag.MAIN_MENU.ToString()) 
                || playerFlag == PlayerFlag.ACHIEVEMENTS.ToString())
            {
                return true;
            }
            return false;
        }
    }
}
