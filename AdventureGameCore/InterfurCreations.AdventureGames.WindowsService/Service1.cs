using InterfurCreations.AdventureGames.BotMain;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.WindowsService
{
    public partial class AdventureBot : ServiceBase
    {
        public AdventureBot()
        {
            Log.LogMessage("Bot Service Main entry called. Initialising Service.");
            var type = new ConfigurationService().GetConfig("TypeName");
            this.ServiceName = type;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log.LogMessage("Bot service started and running. Calling Main method.");
            ProgramRun.Run();
        }

        protected override void OnStop()
        {
        }
    }
}
