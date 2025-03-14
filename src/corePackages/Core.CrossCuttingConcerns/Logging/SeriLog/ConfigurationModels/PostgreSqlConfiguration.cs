namespace Core.CrossCuttingConcerns.Logging.SeriLog.ConfigurationModels;

public class PostgreSqlConfiguration
{
    public string ConnectionString { get; set; }
    public string TableName { get; set; }
    public bool NeedAutoCreateTable { get; set; }

    public PostgreSqlConfiguration()
    {
        ConnectionString = string.Empty;
        TableName = string.Empty;
    }

    public PostgreSqlConfiguration(string connectionString, string tableName, bool needAutoCreateTable)
    {
        ConnectionString = connectionString;
        TableName = tableName;
        NeedAutoCreateTable = needAutoCreateTable;
    }
}
