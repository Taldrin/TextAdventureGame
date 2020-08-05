using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Core.MessageHandlers
{
    public class SendUsAMessageHandler : IMessageHandler
    {
        public int Priorioty => 6;

        private readonly IReporter _reporter;


        public SendUsAMessageHandler(IReporter reporter)
        {
            _reporter = reporter;
        }

        public List<string> GetOptions(Player player)
        {
            return new List<string> { "Return to menu" };
        }

        public ExecutionResult HandleMessage(string message, Player player)
        {
            if (player.PlayerFlag == PlayerFlag.MAIN_MENU.ToString())
            {
                player.PlayerFlag = PlayerFlag.SEND_US_MESSAGE.ToString();
                return ExecutionResultHelper.SingleMessage("You can send us a message here! Just type in your message and send it as you" +
                            " would any other message, and it'll find its way to us :) \n\nYou can use this to send" +
                            " us anything you like, including bug reports and ideas! \n\n If you're on Discord, use 'ft.YourMessageHere'. \n\n" +
                            " If you would like us to contact you back, please remember to send contact details, such as your Telegram or Discord!",
                     new List<string> { "Return to menu" }, true);
            } else if(message == "Return to menu")
            {
                return MessageHandlerHelpers.ReturnToMainMenu(player);
            } else
            {
                _reporter.UserReportMessage(message);
                return ExecutionResultHelper.SingleMessage("Message recieved! Thank you!", GetOptions(player), true);
            }
        }

        public bool ShouldHandleMessage(string message, string gameState, string playerFlag)
        {
            if (playerFlag == PlayerFlag.SEND_US_MESSAGE.ToString() || (playerFlag == PlayerFlag.MAIN_MENU.ToString() && message == Messages.ContactUs))
                return true;
            return false;
        }
    }
}
