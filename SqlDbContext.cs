namespace SqlLightning
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;

    /// <summary>
    /// Database context for connecting to SQL Server instances.
    /// </summary>
    public class SqlDbContext : DatabaseContext, ISqlDbContext
    {
        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDbContext"/> class.
        /// </summary>
        /// <param name="dbConn">The database connection to initialize with.</param>
        private SqlDbContext(DbConnection dbConn) : base(dbConn) { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the SQL connection for the context.
        /// </summary>
        protected new SqlConnection Connection
        {
            get
            {
                return base.Connection as SqlConnection;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new instance of the <see cref="SqlDbContext"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to use when connecting to the target SQL instance.</param>
        /// <returns>A new <see cref="SqlDbContext"/> instance pointed to the target database.</returns>
        public static SqlDbContext Create(string connectionString)
        {
            var dbConn = new SqlConnection(connectionString);
            var context = new SqlDbContext(dbConn);
            context.ValidateConnectionState();
            return context;
        }

        /// <inheritdoc />
        public int ExecuteNonQuery(string procName, params StoredProcParam[] parameters)
        {
            return this.ExecuteTransaction(
                this.GetCommand(procName, parameters),
                (command) =>
                {
                    return command.ExecuteNonQuery();
                });
        }

        /// <inheritdoc />
        public SqlDataReader ExecuteReader(string procName, params StoredProcParam[] parameters)
        {
            return this.ExecuteTransaction(
                this.GetCommand(procName, parameters),
                (command) =>
                {
                    return command.ExecuteReader();
                });
        }

        /// <inheritdoc />
        public DataTable ExecuteDataTable(string procName, params StoredProcParam[] parameters)
        {
            return this.ExecuteTransaction(
                this.GetCommand(procName, parameters),
                (command) =>
                {
                    var table = new DataTable();
                    var reader = command.ExecuteReader();
                    table.Load(reader);
                    return table;
                });
        }

        /// <inheritdoc />
        public void ExecuteSqlBulkCopy(string tableName, IDataReader reader, bool hasIdentity)
        {
            this.ValidateConnectionState();
            var transaction = this.Connection.BeginTransaction();

            try
            {
                var bulkCopy = new SqlBulkCopy(this.Connection, SqlBulkCopyOptions.Default, transaction);
                bulkCopy.DestinationTableName = tableName;

                // True up the field names.
                if (hasIdentity)
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        bulkCopy.ColumnMappings.Add(i, i + 1);
                    }
                }

                bulkCopy.WriteToServer(reader);
                transaction.Commit();
            }
            catch (Exception)
            {
                // Try to roll back if we can
                if (transaction != null && this.State == ConnectionState.Open)
                {
                    transaction.Rollback();
                }
            }
        }

        /// <inheritdoc />
        public T ExecuteWithReturnValue<T>(string procName, params StoredProcParam[] parameters)
        {
            // There should ever only be one return value in a parameters collection
            var retVal = parameters.SingleOrDefault(p => p.Direction == ParameterDirection.ReturnValue);
            if (retVal == null)
            {
                throw new ArgumentException("The supplied parameter list does not contain a return value!", "retVal");
            }

            var cmd = this.GetCommand(procName, parameters);
            if (!cmd.Parameters.Contains(retVal))
            {
                cmd.Parameters.Add(retVal);
            }

            return this.ExecuteTransaction(
                cmd,
                (command) =>
                {
                    command.ExecuteNonQuery();

                    // Attempt to hard cast to T, expect exceptions if it doesn't work.
                    return (T)retVal.Value;
                });
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Retrieves a SqlCommand instance for the target stored procedure name and parameter list.
        /// </summary>
        /// <param name="procName">The name of the stored procedure to execute.</param>
        /// <param name="parameters">A list of parameters to pass to the stored procedure.</param>
        /// <returns>A SqlCommand instance ready to execute.</returns>
        protected SqlCommand GetCommand(string procName, params StoredProcParam[] parameters)
        {
            var cmd = new SqlCommand(procName, this.Connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            foreach (var param in parameters)
            {
                cmd.Parameters.Add(param);                
            }

            return cmd;
        }

        #endregion
    }
}
