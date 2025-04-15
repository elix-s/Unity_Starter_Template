public abstract class DataMigrator<T> where T : IVersionedData
{
    public abstract T Migrate(string json, int oldVersion, int newVersion);
}

