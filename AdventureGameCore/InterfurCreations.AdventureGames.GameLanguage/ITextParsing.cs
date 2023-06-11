using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Graph;

namespace InterfurCreations.AdventureGames.GameLanguage
{
    public interface ITextParsing
    {
        string CleanText(string text);
        void ParseAttachment(Player player, PlayerGameSave gameSave, StateAttachment stateAttachment);
        string ParseText(PlayerGameSave gameSave, string text);
        bool ResolveCommand(PlayerGameSave gameSave, string command);
        ParsedStateOption ResolveOption(PlayerGameSave gameSave, string optionText);
        bool ShouldRun(string command, bool onlyRunAfterText, out string restOfCommand);
    }
}