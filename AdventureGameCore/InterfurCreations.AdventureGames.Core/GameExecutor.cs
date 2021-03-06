using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Core.Objects;
using InterfurCreations.AdventureGames.Core.Services;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Exceptions;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace InterfurCreations.AdventureGames.Core
{
    public class GameExecutor : IGameExecutor
    {
        private List<IMessageHandler> _messageHandlers;
        private IDataStore _dataStore;
        private IReporter _reporter;
        private IAccessService _accessService;

        public GameExecutor(IEnumerable<IMessageHandler> messageHandlers, IDataStore dataStore, IReporter reporter, 
            IAccessService accessService)
        {
            _messageHandlers = messageHandlers.ToList();
            _dataStore = dataStore;
            _reporter = reporter;
            _accessService = accessService;
            MessageHandlerHelpers.SetHandlers(messageHandlers);
        }

        public List<string> GetPossibleOptionsFromState(PlayerState state)
        {
            var handler = GetMessageHandlerForState("", state);

            try
            {
                return handler.GetOptions(state.player);
            }
            catch (AdventureGameException e)
            {
                if (e.ShouldReset)
                {
                    var result = TryResetState(state);
                    _dataStore.SaveChanges();
                    result.MessagesToShow.Add(new MessageResult { Message = "Your game was reset due to an error: " + e.Message });
                    _reporter.ReportError(ErrorMessageHelper.MakeMessage(e, state));
                    return result.OptionsToShow;
                }
            }
            catch (Exception e)
            {
                var result = TryResetState(state);
                _dataStore.SaveChanges();
                result.MessagesToShow.Add(new MessageResult { Message = "Your game was reset due to an error: " + e.Message });
                _reporter.ReportError(ErrorMessageHelper.MakeMessage(e, state));
                return result.OptionsToShow;
            }

            return new List<string> {"Error occured. Retry."};
        }

        public ExecutionResult ProcessNewMessage(string message, PlayerState state)
        {
            ExecutionResult result = null;
            if (!_accessService.DoesPlayerHaveAccess(state.player))
            {
                result = HandleNoAccess(message, state.player);
                _dataStore.SaveChanges();
                return result;
            }
            try
            {
                var handler = GetMessageHandlerForState(message, state);
                result = handler.HandleMessage(message, state.player);
                _dataStore.SaveChanges();
                return result;
            }
            catch (AdventureGameException e)
            {
                if (e.ShouldReset)
                {
                    try
                    {
                        _reporter.ReportError(ErrorMessageHelper.MakeMessage(e, state, "Handling message: " + message));
                        result = TryResetState(state);
                        _dataStore.SaveChanges();
                        result.MessagesToShow.Add(new MessageResult { Message = "Your game was reset due to an error: " + e.Message });
                        return result;
                    }
                    catch (Exception exc)
                    {
                        return ExecutionResultHelper.SingleMessage("An error has been encountered and automatically reported. Apologies! We usually fix any errors within 24 hours. If you are in" +
                    " a stuck state on Telegram or Discord, try typing '-Menu-'. If you are in browser, try refreshing. \n\nError message: " + exc.Message, null);
                    }
                }
                else
                {
                    _reporter.ReportError(ErrorMessageHelper.MakeMessage(e, state, "Game was NOT reset. Handling message: " + message));
                    return ExecutionResultHelper.SingleMessage("An error has been encountered and automatically reported. Apologies! We usually fix any errors within 24 hours. If you are in" +
                                    " a stuck state on Telegram or Discord, try typing '-Menu-'. If you are in browser, try refreshing. \n\nError message: " + e.Message, null);
                }
            }
            catch (DbUpdateConcurrencyException e)
            {
                e.Entries.Single().Reload();
                if (result != null)
                {
                    return result;
                }
                else
                {
                    _reporter.ReportError(ErrorMessageHelper.MakeMessage(e, state, "DB Update Exception handling message: " + message));
                    return new ExecutionResult
                    {
                        MessagesToShow = new List<MessageResult> { new MessageResult { Message = "DB Update error. Your game was NOT reset. Error message: " + e.Message } },
                        OptionsToShow = new List<string> { Messages.GameMenu }
                    };
                }
            }
            catch (Exception e)
            {
                _reporter.ReportError(ErrorMessageHelper.MakeMessage(e, state, "Handling message: " + message));
                result = TryResetState(state);
                _dataStore.SaveChanges();
                result.MessagesToShow.Add(new MessageResult { Message = "Your game was reset due to an error: " + e.Message });
                return result;
            }
        }

        private ExecutionResult HandleNoAccess(string message, Player player)
        {
            var result = _accessService.TryGrantAccess(player, message);
            if (!result.result)
            {
                return ExecutionResultHelper.SingleMessage("You do not have access to these adventure games. Please enter an access token to be granted access. Reason for token failure: " + result.reason, new List<string>());
            } else
            {
                return ExecutionResultHelper.SingleMessage("Access granted! Welcome aboard, sailor!", new List<string> {"Let's go!" });
            }
        }

        private IMessageHandler GetMessageHandlerForState(string message, PlayerState state)
        {
            var availableHandlers = _messageHandlers.Where(a => a.ShouldHandleMessage(message, state.player.ActiveGameSave?.GameName, state.player.PlayerFlag)).ToList();
            if (availableHandlers.Count > 1)
                return availableHandlers.OrderByDescending(a => a.Priorioty).FirstOrDefault();
            else if (availableHandlers.Count == 0)
                throw new AdventureGameException("Could not find a message handler to process message: " + message);
            else
                return availableHandlers.FirstOrDefault();
        }

        public ExecutionResult TryResetState(PlayerState state)
        {
            state.player.ActiveGameSave = new PlayerGameSave();
            return NewGame(state);
        }


        private ExecutionResult NewGame(PlayerState state)
        {
            state.player.ActiveGameSave = new PlayerGameSave
            {
                GameName = null,
                GameSaveData = new List<PlayerGameSaveData>(),
                StateId = null,
            };
            state.player.PlayerFlag = PlayerFlag.MAIN_MENU.ToString();
            return ProcessNewMessage("", state);
        }
    }
}
