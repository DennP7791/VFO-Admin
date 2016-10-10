using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.Linq;

namespace WDAdmin.Domain
{
    /// <summary>
    /// Class Migrator.
    /// </summary>
    public class Migrator : IDisposable
    {
        /// <summary>
        /// The _files
        /// </summary>
        private FileInfo[] _files;
        /// <summary>
        /// The _context
        /// </summary>
        private  DataContext _context;
        /// <summary>
        /// The migratio n_ tabl e_ name
        /// </summary>
        const string MIGRATION_TABLE_NAME = "Migration";

        /// <summary>
        /// Initializes a new instance of the <see cref="Migrator"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="path">The path.</param>
        public Migrator(string connectionString, string path = "MigrationScripts")
        {
            Initialize(new DataContext(connectionString), path);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Migrator"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="path">The path.</param>
        public Migrator(DataContext context, string path = "MigrationScripts")
        {
            Initialize(context, path);
        }

        /// <summary>
        /// Initializes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="path">The path.</param>
        private void Initialize(DataContext context, string path)
        {
            _context = context;
            _files = Directory.EnumerateFiles(path, "*.sql")
                .Select(x => new FileInfo(x))
                .OrderBy(x => x.Name)
                .ToArray();
        }

        /// <summary>
        /// Migrates this instance.
        /// </summary>
        public void Migrate()
        {
            CreateMigrationTableIfMissing();
            var applidMigrations = AlreadyAppliedMigrations();
            var migrationFiles = _files.Where(x => !applidMigrations.Contains(Path.GetFileNameWithoutExtension(x.Name))).ToArray();
            ExecuteCommands(BuildCommands(migrationFiles));
        }

        /// <summary>
        /// Tables the exists.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TableExists(string tableName)
        {
            const string sql = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_CATALOG = {0} AND TABLE_NAME = {1}";
            var result = _context.ExecuteQuery<int>(sql, _context.Connection.Database, tableName).ToList();
            System.Diagnostics.Debug.Assert(result.Count == 1);
            return result.Single() == 1;
        }

        /// <summary>
        /// Creates the migration table if missing.
        /// </summary>
        public void CreateMigrationTableIfMissing()
        {
            const string createTableSql = @"
CREATE TABLE Migration (
    Name nvarchar(300) NOT NULL,
    ProductVersion nvarchar(32) NULL
);
";
            if (!TableExists(MIGRATION_TABLE_NAME))
            {
                _context.ExecuteCommand(createTableSql);
            }
        }

        /// <summary>
        /// Alreadies the applied migrations.
        /// </summary>
        /// <returns>System.String[].</returns>
        public string[] AlreadyAppliedMigrations()
        {
            const string sql = @"SELECT Name FROM " + MIGRATION_TABLE_NAME + " ORDER BY Name";
            return _context.ExecuteQuery<string>(sql).ToArray();
        }

        /// <summary>
        /// Builds the commands.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns>System.String[].</returns>
        public string[] BuildCommands(params FileInfo[] files)
        {
            var commands = new string[files.Length];

            string content;
            FileInfo file;
            string migrationName;
            for (var i = 0; i < files.Length; i++)
            {
                file = files[i];
                content = File.ReadAllText(file.FullName);
                migrationName = Path.GetFileNameWithoutExtension(file.Name);
                commands[i] = BuildMigrationCommand(content, migrationName);
            }
            return commands;
        }

        /// <summary>
        /// Builds the migration command.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="migrationName">Name of the migration.</param>
        /// <returns>System.String.</returns>
        public string BuildMigrationCommand(string sql, string migrationName)
        {
            const string sqlTemplate = @"
DECLARE @migration_name nvarchar(MAX) SET @migration_name = '{0}';
INSERT INTO Migration VALUES (
    @migration_name,
    null
);

{1}
";
            return string.Format(sqlTemplate, migrationName, sql);
        }

        /// <summary>
        /// Executes the commands.
        /// </summary>
        /// <param name="commands">The commands.</param>
        /// <returns>System.Int32[].</returns>
        public int[] ExecuteCommands(params string[] commands)
        {
            int[] result = new int[commands.Length];
            if (commands.Any())
            {
                using (var transaction = TransactionScopeUtils.CreateTransactionScope())
                {
                    for (var i = 0; i < commands.Length; i++)
                    {
                        result[i] = _context.ExecuteCommand(commands[i]);
                    }
                    transaction.Complete();
                }
            }
            return result;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
