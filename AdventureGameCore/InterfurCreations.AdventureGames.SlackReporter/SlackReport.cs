using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Services.Interfaces;
using SlackAPI;
using Slacker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InterfurCreations.AdventureGames.Logging;

namespace InterfurCreations.AdventureGames.SlackReporter
{
    public class SlackReport : IReporter
    {
        private static string _webHook;
        private static string _userMessageWebHook;
        private static string _errorWebHook;

        private ISlackMessageWriter _messageWriter;
        private ISlackMessageWriter _userMessageWriter;
        private ISlackMessageWriter _errorMessageWriter;

        private IConfigurationService _configService;

        private string _appName;

        public SlackReport(IConfigurationService configService)
        {
            _configService = configService;
            _appName = _configService.GetConfig("TypeName");

            _webHook = _configService.GetConfigOrDefault("SlackWebhook", null);
            _userMessageWebHook = _configService.GetConfigOrDefault("SlackUserMessageWebhook", null);
            _errorWebHook = _configService.GetConfigOrDefault("SlackErrorMessageWebhook", null);

            if (_webHook != null)
            {
                ISlackConnectionProvider connectionProvider = new SlackConnectionProvider(_webHook);
                _messageWriter = new SlackMessageWriter(connectionProvider);
            }
            if(_userMessageWebHook != null)
                _userMessageWriter = new SlackMessageWriter(new SlackConnectionProvider(_userMessageWebHook));
            if (_errorWebHook != null)
                _errorMessageWriter = new SlackMessageWriter(new SlackConnectionProvider(_errorWebHook));
        }

        public void Initialise()
        {
            _messageWriter.Write("bot-reporting", "Slacker", "Slack reporting enabled for bot: " + _appName);
        }

        public void ReportMessage(string message)
        {
            try
            {
                _messageWriter.Write("bot-reporting", "Slacker", DateTime.Now + " - " + _appName + " - " + message);
            } catch(Exception e) { Log.LogMessage("Slack bot broke: " + e.Message); }
        }

        public bool UserReportMessage(string message)
        {
            try
            {
                _userMessageWriter.Write("bot-messages", "Slacker", DateTime.Now + " - " + _appName + " - " + message);
                return true;
            }
            catch (Exception e) { Log.LogMessage("Slack bot broke: " + e.Message); return false; }
        }

        public void ReportError(string error)
        {
            try
            {
                _errorMessageWriter.Write("bot-error-reports", "Slacker", DateTime.Now + " - " + _appName + " - " + error);
            }
            catch (Exception e) { Log.LogMessage("Slack bot broke: " + e.Message); }
        }
    }
}
