using Microsoft.Data.Sqlite;

namespace CqrsProject.Commons.Test.Database;

public class SqliteConnectionPull
{
    private static readonly SqliteConnectionPull _instance = new SqliteConnectionPull();

    public static SqliteConnectionPull Instance => _instance;

    private readonly Dictionary<string, SqliteConnection> _dictionaryConnection;

    private SqliteConnectionPull()
    {
        _dictionaryConnection = new Dictionary<string, SqliteConnection>();
    }

    public SqliteConnection GetOpenedConnection(string connectionString)
    {
        if (_dictionaryConnection.TryGetValue(connectionString, out var connection))
            return connection;

        connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        _dictionaryConnection.Add(connectionString, connection);

        return connection;
    }
}
