using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services
{
    public class InputController : IInputController
    {
        private ICommunicator _communicator;

        public InputController(ICommunicator communicator)
        {
            _communicator = communicator;
        }

        public void Setup()
        {
            _communicator.SetupAsync();
        }
    }
}
