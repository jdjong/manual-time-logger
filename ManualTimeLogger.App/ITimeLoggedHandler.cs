using ManualTimeLogger.Domain;

namespace ManualTimeLogger.App
{
    public interface ITimeLoggedHandler
    {
        void HandleTimeLogged(LogEntry logEntry);
    }
}