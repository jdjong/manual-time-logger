namespace ManualTimeLogger.Persistence
{
    public interface IRepositoryFactory
    {
        IRepository Create(string fileName);
        string GetFilesBasePath();
    }
}