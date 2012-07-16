namespace SqlLightning
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// Defines behavior of a database context that works with SQL server. These methods will
    /// only execute stored procedures, no SQL clear text is allowed.
    /// </summary>
    public interface ISqlDbContext : IDisposable
    {
        #region Interface Methods

        /// <summary>
        /// Executes a stored procedure and returns an in-memory DataTable. This method is more
        /// memory intensive than <see cref="ExecuteReader"/> and should only be used for smaller
        /// record sets.
        /// </summary>
        /// <param name="procName">The name of the stored procedure to execute.</param>
        /// <param name="parameters">A list of parameters to pass to the stored procedure.</param>
        /// <returns>A DataTable containing the results of the stored procedure.</returns>
        DataTable ExecuteDataTable(string procName, params StoredProcParam[] parameters);

        /// <summary>
        /// Executes a stored procedure and returns it's result.
        /// </summary>
        /// <param name="procName">The name of the stored procedure to execute.</param>
        /// <param name="parameters">A list of parameters to pass to the stored procedure.</param>
        /// <returns>The return value of the stored procedure.</returns>
        int ExecuteNonQuery(string procName, params StoredProcParam[] parameters);

        /// <summary>
        /// Executes a stored procedure and returns a SqlDataReader.
        /// </summary>
        /// <param name="procName">The name of the stored procedure to execute.</param>
        /// <param name="parameters">A list of parameters to pass to the stored procedure.</param>
        /// <returns>A SqlDataReader containing the result of the stored procedure.</returns>
        SqlDataReader ExecuteReader(string procName, params StoredProcParam[] parameters);

        /// <summary>
        /// Performs a bulk copy of the contents of the target data reader to the named table.
        /// </summary>
        /// <param name="tableName">The name of the table to copy the data to.</param>
        /// <param name="reader">A data reader implenetation containing the data to copy.</param>
        /// <param name="hasIdentity">Does the target table have an identity associated with it? This
        /// switch will dictate whether or not the method should shift column mappings to accomodate for
        /// IDENTITY columns.</param>
        void ExecuteSqlBulkCopy(string tableName, IDataReader reader, bool hasIdentity);

        /// <summary>
        /// Executes a stored procedure with a return value of type T as one of it's parameters. Note that the
        /// list of parameters passed to this method must contain a parameter with a direction type
        /// of ParameterDirection.ReturnValue else this method will throw an ArgumentException.
        /// </summary>
        /// <typeparam name="T">The return type of the value returned from the stored procedure.</typeparam>
        /// <param name="procName">The name of the stored procedure to execute.</param>
        /// <param name="parameters">A list of parameters to pass to the stored procedure. The list must
        /// contain exactly one parameter with a direction type of ParameterDirection.ReturnValue.</param>
        /// <returns>The return value of the stored procedure.</returns>
        T ExecuteWithReturnValue<T>(string procName, params StoredProcParam[] parameters);
        
        #endregion
    }
}
