using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Exceptions;
using InterfurCreations.AdventureGames.GameLanguage;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Graph.Store;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InterfurCreations.AdventureGames.Core.MessageHandlers
{
    public class MainMenuMessageHandler : IMessageHandler
    {
        private readonly IGameRetrieverService _gameStore;
        private readonly IGameProcessor _gameProcessor;
        private readonly ITextParsing _textParsing;

        public int Priorioty => 2;

        public MainMenuMessageHandler(IGameRetrieverService gameStore, IGameProcessor gameProcessor, ITextParsing textParsing)
        {
            _gameStore = gameStore;
            _gameProcessor = gameProcessor;
            _textParsing = textParsing;
        }

        public ExecutionResult HandleMessage(string message, Player player)
        {
            var games = _gameStore.ListGames();
            if (player.PlayerFlag == PlayerFlag.MAIN_MENU.ToString() || player.PlayerFlag == PlayerFlag.MICRO_GAMES_MENU.ToString())
            {
                if (message.ToLower() == Messages.MiniGames.ToLower())
                {
                    player.PlayerFlag = PlayerFlag.MICRO_GAMES_MENU.ToString();
                    return MiniGameMessage(games);
                }
                if(message.ToLower() == Messages.Return.ToLower())
                {
                    player.PlayerFlag = PlayerFlag.MAIN_MENU.ToString();
                    return MainMenuMessage(games, player);
                }
                if(message.ToLower() == Messages.AIAdventures.ToLower())
                {
                    player.PlayerFlag = PlayerFlag.AI_ADVENTURES.ToString();
                    return AIAdventuresMessage(player);
                }
                var gameSave = player.ActiveGameSave;

                var gamesFound = games.Where(a => a.GameName.ToLower() == message.ToLower()).ToList();

                var newGameBeingPlayed = gamesFound.FirstOrDefault();

                if (newGameBeingPlayed == null)
                {
                    if (player.PlayerFlag == PlayerFlag.MICRO_GAMES_MENU.ToString())
                        return MiniGameMessage(games);
                    return MainMenuMessage(games, player);
                }

                var execResult = GameService.LaunchGameForPlayer(newGameBeingPlayed, gameSave, player, _gameProcessor);
                execResult.OptionsToShow.Add("-Menu-");
                return execResult;
            }
            else
            {
                player.PlayerFlag = PlayerFlag.MAIN_MENU.ToString();
                return MainMenuMessage(games, player);
            }
        }

        public bool ShouldHandleMessage(string message, string gameState, string playerFlag)
        {
            if (gameState == null || (playerFlag == null || playerFlag == PlayerFlag.MAIN_MENU.ToString() || playerFlag == PlayerFlag.MICRO_GAMES_MENU.ToString()))
                return true;
            return false;
        }

        public ExecutionResult MainMenuMessage(List<DrawGame> games, Player player)
        {
            var filteredList = games.Where(a => a.Metadata?.Category != Messages.MiniGames).ToList();
            if (filteredList == null)
                filteredList = new List<DrawGame>();
            List<string> options = new List<string>();
            options = filteredList.Select(a => a.GameName).ToList();
            options.Add(Messages.MiniGames);
            options.Add(Messages.AIAdventures);
            options.Add(Messages.ContactUs);
            options.Add(Messages.LoadGame);
            options.Add($"{Messages.Achievements} - ({AchievementService.CountAchievementsCompletedForGames(games, player)}/{AchievementService.CountTotalAchievements(games)})");
            var execResult = new ExecutionResult();
            var messages = new List<MessageResult>();
            messages.Add(new MessageResult
            {
                Message = "All characters in this game are 18+! All characters are random, and not based on any pre-existing characters. These games feature adult content, including mild fetishes."
            });
            messages.Add(new MessageResult
            {
                Message = "Games Available:"
            });
            messages.AddRange(filteredList.Select(a => new MessageResult { Message = a.GameName + " - " + a.Metadata?.Description }));
            messages.Add(
                  new MessageResult
                  {
                      Message = "Enter a game to play!"
                  });
            execResult.OptionsToShow = options;
            execResult.MessagesToShow = messages;
            return execResult;
        }

        public ExecutionResult AIAdventuresMessage(Player player)
        {
            var execResult = new ExecutionResult();
            var messages = new List<MessageResult>();
            List<string> options = new List<string>();
            options.Add("ChatGPT 1");
            options.Add(Messages.Return);

            messages.Add(new MessageResult
            {
                Message = "AI Test games!"
            });
            execResult.OptionsToShow = options;
            execResult.MessagesToShow = messages;
            return execResult;
        }

        public ExecutionResult MiniGameMessage(List<DrawGame> games)
        {
            var filteredList = games.Where(a => a.Metadata?.Category == Messages.MiniGames).ToList();
            if (filteredList == null)
                filteredList = new List<DrawGame>();
            List<string> options = new List<string>();
            options = filteredList.Select(a => a.GameName).ToList();
            options.Add(Messages.Return);
            var execResult = new ExecutionResult();
            var messages = new List<MessageResult>();
            messages.Add(new MessageResult
            {
                Message = "These are shorter and much more to the point, but entertaining nonetheless! Sometimes we try new story ideas or topics or game types here! Give them a try!"
            });
            messages.Add(new MessageResult
            {
                Message = "Micro Games Available:"
            });
            messages.AddRange(filteredList.Select(a => new MessageResult { Message = a.GameName + " - " + a.Metadata?.Description }));
            messages.Add(
                  new MessageResult
                  {
                      Message = "Enter a game to play!"
                  });
            execResult.OptionsToShow = options;
            execResult.MessagesToShow = messages;
            return execResult;
        }

        public List<string> GetOptions(Player player)
        {
            if (player.PlayerFlag == PlayerFlag.MAIN_MENU.ToString())
            {
                var games = _gameStore.ListGames();
                return MainMenuMessage(games, player).OptionsToShow;
            }
            else if (player.PlayerFlag == PlayerFlag.MICRO_GAMES_MENU.ToString())
            {
                var games = _gameStore.ListGames();
                return MiniGameMessage(games).OptionsToShow;
            }
            else throw new AdventureGameException("Invalid player state", true);
        }
    }
}
