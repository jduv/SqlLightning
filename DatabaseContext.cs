namespace SqlLightning
{
    using System;
    using System.Data;
    using System.Data.Common;

    /// <summary>
    /// Abstract base class for database connectors.
    /// </summary>
    public abstract class DatabaseContext : IDisposable
    {
        #region Fields & Constants

        private bool disposed;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        /// <param name="dbConn">The database connection to initialize with.</param>
        protected DatabaseContext(DbConnection dbConn)
        {
            this.Connection = dbConn;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        ~DatabaseContext()
        {
            this.OnDispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current connection state for the context.
        /// </summary>
        public ConnectionState State
        {
            get
            {
                return this.Connection.State;
            }
        }

        /// <summary>
        /// Gets the abstract database connection for the context.
        /// </summary>
        protected DbConnection Connection { get; private set; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public void Dispose()
        {
            this.OnDispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {            string me = "Connection State => {0}";

            return string.Format(
                me,
                (this.Connection != null) ? this.Connection.State.ToString() : "Null");
        }

        /// <summary>
        /// Closes this database connection.
        /// </summary>
        public void Close()
        {
            this.Connection.Close();
        }

        /// <summary>
        /// Executes the target function inside a transaction against the context's target database. If the connection
        /// is not open, this method will attempt to open it before performing the operation. Use this method to perform
        /// units of work against the database.
        /// </summary>
        /// <typeparam name="Tcmd">The type of command to execute. This parameter must be a subclass of the
        /// DbCommand type.</typeparam>
        /// <typeparam name="Tres">The type of result that the operation should return.</typeparam>
        /// <param name="command">The command instance to execute.</param>
        /// <param name="f">A function containing some business logic to execute against the database.</param>
        /// <returns>A result of type Tres, returned by the target operation f executed against the database.</returns>
        public Tres ExecuteTransaction<Tcmd, Tres>(Tcmd command, Func<Tcmd, Tres> f) where Tcmd : DbCommand
        {
            this.ValidateConnectionState();
            var transaction = this.Connection.BeginTransaction();
            command.Transaction = transaction;

            try
            {
                var result = f(command);
                transaction.Commit();
                return result;
            }
            catch (Exception)
            {
                // Some kind of SqlException hopefully, else there's no way to roll back if the connection is dead :(
                if (transaction != null && this.State == ConnectionState.Open)
                {
                    transaction.Rollback();
                }

                throw;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Called when the context is disposing. Override for more specific teardown behavior.
        /// </summary>
        /// <param name="disposing">Is the context disposing?</param>
        protected virtual void OnDispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                try
                {
                    this.Close();
                }
                catch (Exception)
                {
                    // Eat exceptions
                }

                this.Connection = null;
                this.disposed = true;
            }
        }

        /// <summary>
        /// Validates the state of the database connection. If something's wrong it will throw an InvalidOperationException,
        /// else it will attempt to open the connection if it's closed.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the connection is in a bad state.</exception>
        protected void ValidateConnectionState()
        {
            if (this.Connection == null)
            {
                throw new InvalidOperationException("Connection is in a bad state! " + this.ToString());
            }

            // Attempt to open the connection. Will throw if something's wrong.
            if (this.Connection.State == ConnectionState.Broken || 
                this.Connection.State == ConnectionState.Closed)
            {
                this.Connection.Open();
            }
        }

        #endregion
    }
}