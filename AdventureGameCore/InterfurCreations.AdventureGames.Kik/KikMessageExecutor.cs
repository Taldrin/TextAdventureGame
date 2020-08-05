using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Core.Objects;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Kik.KikObjects;
using InterfurCreations.AdventureGames.Kik.KikObjects.Send;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Kik
{
    public class KikMessageExecutor
    {
        private readonly IPlayerDatabaseController _playerController;
        private readonly IAccountController _accountController;
        private readonly IReporter _reporter;
        private readonly IGameExecutor _gameExecutor;

        public KikMessageExecutor(IPlayerDatabaseController playerController, IAccountController accountController, IReporter reporter, IGameExecutor gameExecutor)
        {
            _playerController = playerController;
            _accountController = accountController;
            _reporter = reporter;
            _gameExecutor = gameExecutor;
        }

        public async Task ProcessMessage(KikGenericRecieveMessageDataObject kikGenericMessage)
        {
            string messageBody = "";
            if (kikGenericMessage is KikMessageDataObject message)
            {
                if (message.body != null)
                    messageBody = message.body;
            }

            var playerState = _playerController.GetPlayerByKikChannel(kikGenericMessage.chatId);
            if (playerState == null)
            {
                _reporter.ReportMessage("The bot was started in a new chat, with username: " + kikGenericMessage.from);
                playerState = _accountController.CreateNewKikAccount(kikGenericMessage.chatId, kikGenericMessage.from);
            }

            var executionResult = _gameExecutor.ProcessNewMessage(messageBody, new PlayerState { player = playerState });

            if (executionResult != null)
            {
                var batchesToSend = BuildResponses(executionResult, kikGenericMessage.chatId, kikGenericMessage.from);
                foreach (var batch in batchesToSend)
                    await KikMethods.SendMessageAsync(KikService._kikHttp, batch);
            }
        }

        public async Task ProcessInvalidInput(KikGenericRecieveMessageDataObject kikGenericMessage)
        {
            await KikMethods.SendMessageAsync(KikService._kikHttp, new KikMessagesSendDataObject {
                messages = new List<KikGenericSendMessageDataObject>
                {
                    new KikMessageSendWithoutKeyboardDataObject
                    {
                        body = "That is not a valid input! Please use the suggested response buttons or type in your reply.",
                        chatId = kikGenericMessage.chatId,
                        to = kikGenericMessage.from,
                        type = "text",
                    }
                }
            });
        }

        private List<KikMessagesSendDataObject> BuildResponses(ExecutionResult result, string chatId, string chatName)
        {
            List<KikMessagesSendDataObject> batchesToSend = new List<KikMessagesSendDataObject>();

            var keyboard = new KikKeyboardDataObject();
            keyboard.type = "suggested";
            keyboard.responses = new List<KikKeyboardKeyDataObject>();

            result.OptionsToShow.ForEach(a =>
            {
                keyboard.responses.Add(new KikKeyboardKeyDataObject
                {
                    body = a,
                    type = "text"
                });
            });

            var batchedMessages = result.MessagesToShow.Batch(5);
            batchedMessages.ForEach(batch =>
            {
                var dto = new KikMessagesSendDataObject();
                dto.messages = new List<KikGenericSendMessageDataObject>();

                batch.ForEach(a =>
                {
                    if (!string.IsNullOrEmpty(a.ImageUrl))
                    {
                        var messageDto = new KikPictureMessageDataObject();
                        messageDto.chatId = chatId;
                        messageDto.to = chatName;
                        messageDto.type = "picture";
                        messageDto.picUrl = a.ImageUrl;
                        messageDto.keyboards = new List<KikKeyboardDataObject> { keyboard };
                        dto.messages.Add(messageDto);
                    }
                    else
                    {
                        var formattedMessage = a.Message.Replace("\r", "");
                        var messageDto = new KikMessageSendWithKeyboardDataObject();
                        messageDto.body = formattedMessage;
                        messageDto.chatId = chatId;
                        messageDto.to = chatName;
                        messageDto.type = "text";
                        messageDto.keyboards = new List<KikKeyboardDataObject> { keyboard };
                        dto.messages.Add(messageDto);
                    }
                });
                batchesToSend.Add(dto);
            });

            return batchesToSend;
        }
    }
}
