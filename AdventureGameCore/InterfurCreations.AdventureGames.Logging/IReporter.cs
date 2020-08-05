namespace InterfurCreations.AdventureGames.Logging

{
    public interface IReporter
    {
        void ReportMessage(string message);
        bool UserReportMessage(string message);

        void Initialise();

        void ReportError(string error);
    }
}
