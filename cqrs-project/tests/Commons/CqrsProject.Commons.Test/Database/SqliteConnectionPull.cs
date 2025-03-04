using System.Collections.Concurrent;
using Microsoft.Data.Sqlite;

namespace CqrsProject.Commons.Test.Database;

public class SqliteConnectionPull
{
    private readonly ConcurrentDictionary<string, SqliteConnection> _dictionaryConnection;

    public SqliteConnectionPull()
    {
        _dictionaryConnection = new ConcurrentDictionary<string, SqliteConnection>();
    }

    public SqliteConnection GetOpenedConnection(string connectionString)
    {
        if (_dictionaryConnection.TryGetValue(connectionString, out var connection))
            return connection;

        connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        if (!_dictionaryConnection.TryAdd(connectionString, connection))
            _dictionaryConnection.TryGetValue(connectionString, out connection);

        return connection!;
    }
}
