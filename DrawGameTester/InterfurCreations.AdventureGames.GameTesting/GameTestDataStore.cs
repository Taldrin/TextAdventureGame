using InterfurCreations.AdventureGames.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.GameTesting
{
    public class GameTestDataStore
    {
        public Dictionary<string, int> StateVisits;
        public Dictionary<string, int> OptionVisits;
        public HashSet<UniqueTestDataItemWithSave> PotentialEndStates;
        public HashSet<UniqueTestDataItemWithSave> ErrorMessages;
        public HashSet<UniqueTestDataItemWithSave> GrammarMistakes;
        public HashSet<UniqueTestDataItemWithSave> VariableNames;
        public HashSet<UniqueTestDataItemWithSave> WarningMessages;

        public GameTestDataStore()
        {
            StateVisits = new Dictionary<string, int>();
            OptionVisits = new Dictionary<string, int>();
            PotentialEndStates = new HashSet<UniqueTestDataItemWithSave>();
            ErrorMessages = new HashSet<UniqueTestDataItemWithSave>();
            GrammarMistakes = new HashSet<UniqueTestDataItemWithSave>();
            VariableNames = new HashSet<UniqueTestDataItemWithSave>();
            WarningMessages = new HashSet<UniqueTestDataItemWithSave>();
        }

        public void WarningMessage(string message, PlayerGameSave gameSave)
        {
            WarningMessages.Add(new UniqueTestDataItemWithSave {Value = message, GameSave = gameSave });
        }

        public void PotentialEndStateFound(string stateId, PlayerGameSave gameSave)
        {
            PotentialEndStates.Add(new UniqueTestDataItemWithSave { Value = stateId, GameSave = gameSave });
        }

        public void ErrorMessageEncountered(string errorMessage, PlayerGameSave gameSave)
        {
            ErrorMessages.Add(new UniqueTestDataItemWithSave { Value = errorMessage, GameSave = gameSave });
        }

        public void GrammarMistakeFound(string grammarMessage, PlayerGameSave gameSave)
        {
            GrammarMistakes.Add(new UniqueTestDataItemWithSave { Value = grammarMessage, GameSave = gameSave });
        }

        public void VariableNameEncountered(string variableName)
        {
            VariableNames.Add(variableName);
        }

        internal void CheckForNewVariables(PlayerGameSave gameState)
        {
            gameState.GameSaveData.ForEach(a => VariableNameEncountered(a.Name));
        }

        public void StateVisited(string id)
        {
            if (StateVisits.TryGetValue(id, out int value))
                StateVisits[id] = value++;
            else
                StateVisits.Add(id, 1);
        }

        public void OptionChosen(string id)
        {
            if (OptionVisits.TryGetValue(id, out int value))
                OptionVisits[id] = value++;
            else
                OptionVisits.Add(id, 1);
        }

        public int GetTimesChosen(string id)
        {
            if (OptionVisits.TryGetValue(id, out int value))
                return OptionVisits[id];
            else
                return 0;
        }

        public List<string> GetAllStatesVisited()
        {
            return StateVisits.Keys.ToList();
        }
    }
}
