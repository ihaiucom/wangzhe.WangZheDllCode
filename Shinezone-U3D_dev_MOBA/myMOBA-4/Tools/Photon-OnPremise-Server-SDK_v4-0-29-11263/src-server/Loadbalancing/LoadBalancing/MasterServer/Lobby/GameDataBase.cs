namespace Photon.LoadBalancing.MasterServer.Lobby
{
    using System.Collections.Generic;

    using System.Data.SQLite;
    using System.Text;

    using ExitGames.Logging;

    public class GameTable
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly SQLiteConnection sqlConnection;

        private readonly SQLiteCommand insertCommand;

        private readonly SQLiteCommand updateCommand;

        private readonly SQLiteCommand deleteCommand;

        private readonly string[] columnNames;

        private readonly int columnCount;

        private readonly string columnPrefix;

        public GameTable(int columnCount, string columnPrefix)
        {
            this.columnCount = columnCount;
            this.columnPrefix = columnPrefix;

            this.columnNames = new string[this.columnCount];
            for (int i = 0; i < this.columnCount; i++)
            {
                columnNames[i] = columnPrefix + i;
            }

            this.sqlConnection = new SQLiteConnection("Data Source=:memory:");
            this.sqlConnection.Open();

            this.CreateTable();

            this.insertCommand = CreateInsertCommand(this.columnCount, this.columnPrefix);
            this.insertCommand.Connection = this.sqlConnection;

            this.updateCommand = CreateUpdateCommand(this.columnCount, this.columnPrefix);
            this.updateCommand.Connection = this.sqlConnection;

            this.deleteCommand = new SQLiteCommand("DELETE FROM Game WHERE Id=@Id", this.sqlConnection);
            this.deleteCommand.Parameters.Add(new SQLiteParameter("@Id", System.Data.DbType.String));

            ////var command = new SQLiteCommand("CREATE INDEX Game_idx ON Game (C0)", this.sqlConnection);
            ////command.ExecuteNonQuery();
        }

        public long MemoryUsed
        {
            get
            {
                return this.sqlConnection.MemoryUsed;
            }
        }

        public void InsertGameState(string gameId, Dictionary<object, object> properties)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Inserting game: {0}", gameId);
            }

            this.insertCommand.Parameters[0].Value = gameId;

            for (int i = 0; i < this.columnCount; i++)
            {
                object value;
                if (properties.TryGetValue(this.columnNames[i], out value))
                {
                    this.insertCommand.Parameters[i + 1].Value = value;
                }
                else
                {
                    this.insertCommand.Parameters[i + 1].Value = null;
                }
            }

            this.insertCommand.ExecuteNonQuery();
        }

        public void Update(string gameId, Dictionary<object, object> properties)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Updating game: {0}", gameId);
            }

            this.updateCommand.Parameters[0].Value = gameId;

            for (int i = 0; i < this.columnCount; i++)
            {
                object value;
                if (properties.TryGetValue(this.columnNames[i], out value))
                {
                    this.updateCommand.Parameters[i + 1].Value = value;
                }
                else
                {
                    this.updateCommand.Parameters[i + 1].Value = null;
                }
            }

            this.updateCommand.ExecuteNonQuery();
        }

        public int Delete(string gameId)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Removing game: {0}", gameId);
            }

            this.deleteCommand.Parameters[0].Value = gameId;
            return this.deleteCommand.ExecuteNonQuery();
        }

        public string FindMatch(string query)
        {
            var command = new SQLiteCommand(this.sqlConnection);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = "SELECT Id FROM Game WHERE " + query + " LIMIT 1";
            return (string)command.ExecuteScalar();
        }

        public List<string> FindMatches(string query, int limit)
        {
            var result = new List<string>();

            var command = new SQLiteCommand(this.sqlConnection);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = "SELECT Id FROM Game WHERE " + query + " LIMIT " + limit;

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(reader.GetString(0));
            }

            return result;
        }

        public List<object[]> FindMatchesWithValues(string query, int limit)
        {
            var result = new List<object[]>();

            var command = new SQLiteCommand(this.sqlConnection);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = "SELECT * FROM Game WHERE " + query + " LIMIT " + limit;

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var row = new object[this.columnCount + 1];
                // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                reader.GetValues(row);
                // ReSharper restore ReturnValueOfPureMethodIsNotUsed
                result.Add(row);
            }

            return result;
        }

        public void AddColumn(string name)
        {
            var sql = string.Format("ALTER TABLE Game ADD COLUMN {0}", name);
            var command = new SQLiteCommand(sql, this.sqlConnection);
            command.ExecuteNonQuery();
        }

        public void DropColumn(string name)
        {
            using (var transaction = this.sqlConnection.BeginTransaction())
            {

                var command = new SQLiteCommand("CREATE TEMPORARY TABLE backup(Id,C0,C1,C2,C3,C4,C5,C6,C7,C8,C9);", this.sqlConnection);
                command.ExecuteNonQuery();
                command = new SQLiteCommand("INSERT INTO backup SELECT Id,C0,C1,C2,C3,C4,C5,C6,C7,C8,C9 FROM Game;", this.sqlConnection);
                command.ExecuteNonQuery();
                command = new SQLiteCommand("DROP TABLE Game;", this.sqlConnection);
                command.ExecuteNonQuery();
                command = new SQLiteCommand("ALTER TABLE backup RENAME TO Game;", this.sqlConnection);
                command.ExecuteNonQuery();
                transaction.Commit();
            }

            var command2 = new SQLiteCommand("VACUUM;", this.sqlConnection);
            command2.ExecuteNonQuery();

        }

        private void CreateTable()
        {
            var sb = new StringBuilder("CREATE TABLE Game(Id TEXT PRIMARY KEY");
            for (int i = 0; i < columnCount; i++)
            {
                sb.AppendFormat(",{0}{1}", columnPrefix, i);
            }

            sb.Append(")");

            var command = new SQLiteCommand(this.sqlConnection);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = sb.ToString();
            command.ExecuteNonQuery();
        }

        private static SQLiteCommand CreateInsertCommand(int columnCount, string columnPrefix)
        {
            var sb = new StringBuilder("INSERT INTO Game VALUES(@Id");
            for (int i = 0; i < columnCount; i++)
            {
                sb.AppendFormat(",@{0}{1}", columnPrefix, i);
            }

            sb.Append(")");

            var command = new SQLiteCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = sb.ToString();

            command.Parameters.Add(new SQLiteParameter("@Id"));
            for (int i = 0; i < columnCount; i++)
            {
                var parameterName = string.Format("@{0}{1}", columnPrefix, i);
                command.Parameters.Add(new SQLiteParameter(parameterName, System.Data.DbType.Object));
            }

            return command;
        }

        private static SQLiteCommand CreateUpdateCommand(int columnCount, string columnPrefix)
        {
            var sb = new StringBuilder("UPDATE Game SET ");
            for (int i = 0; i < columnCount; i++)
            {
                if (i > 0)
                {
                    sb.Append(',');
                }

                sb.AppendFormat("{0}{1}=@{0}{1}", columnPrefix, i);
            }

            sb.Append(" WHERE Id=@Id");

            var command = new SQLiteCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = sb.ToString();

            command.Parameters.Add(new SQLiteParameter("@Id"));
            for (int i = 0; i < columnCount; i++)
            {
                var parameterName = string.Format("@{0}{1}", columnPrefix, i);
                command.Parameters.Add(new SQLiteParameter(parameterName, System.Data.DbType.Object));
            }

            return command;
        }
    }
}
