namespace SqlLightning
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// Holds all the pertinate data for a stored procedure parameter, and can convert to miscellaneous
    /// database parameter types as needed.
    /// </summary>
    public sealed class StoredProcParam
    {
        #region Constructors & Destructors

        /// <summary>
        /// Prevents a default instance of the <see cref="StoredProcParam"/> class from being created.
        /// Use factory methods instead.
        /// </summary>
        private StoredProcParam() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the parameter's value.
        /// </summary>
        public object Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the parameter's type.
        /// </summary>
        public DbType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the parameter's direction.
        /// </summary>
        public ParameterDirection Direction
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Factory method for creating an input parameter.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The parameter's type.</param>
        /// <param name="value">the parameter's value.</param>
        /// <returns>A <see cref="StoredProcParam"/> that is initialized to function as an
        /// input value to a stored procedure.</returns>
        public static StoredProcParam InParam(string name, DbType type, object value)
        {
            return new StoredProcParam()
            {
                Direction = ParameterDirection.Input,
                Name = name,
                Value = value,
                Type = type
            };
        }

        /// <summary>
        /// Factory method for creating an output parameter.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The parameter's type.</param>
        /// <returns>A <see cref="StoredProcParam"/> that is initialized to function as an
        /// output value to a stored procedure.</returns>
        public static StoredProcParam OutParam(string name, DbType type)
        {
            return new StoredProcParam()
            {
                Direction = ParameterDirection.Output,
                Name = name,
                Type = type,                
            };
        }

        /// <summary>
        /// Factory method for creating an input/output parameter.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The parameter's type.</param>
        /// <param name="value">the parameter's value.</param>
        /// <returns>A <see cref="StoredProcParam"/> that is initialized to function as an
        /// input/output value to a stored procedure.</returns>
        public static StoredProcParam InOutParam(string name, DbType type, object value)
        {
            return new StoredProcParam()
            {
                Direction = ParameterDirection.InputOutput,
                Name = name,
                Type = type,                
                Value = value
            };
        }

        /// <summary>
        /// Factory method for creating a return value parameter.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The parameter's type.</param>
        /// <returns>A <see cref="StoredProcParam"/> that is initialized to function as a
        /// return value to a stored procedure.</returns>
        public static StoredProcParam ReturnValue(string name, DbType type)
        {
            return new StoredProcParam()
            {
                Direction = ParameterDirection.ReturnValue,
                Name = name,
                Type = type
            };
        }

        /// <summary>
        /// Converts this <see cref="StoredProcParam"/> into a SqlParameter.
        /// </summary>
        /// <returns>A SqlParameter based on the information contained inside this 
        /// <see cref="StoredProcParam"/> instance.</returns>
        public SqlParameter ToSqlParam()
        {
            return new SqlParameter()
            {
                Direction = this.Direction,
                ParameterName = this.Name,                
                Value = (this.Direction == ParameterDirection.Input || this.Direction == ParameterDirection.InputOutput) ? this.Value : null,
                DbType = this.Type
            };            
        }

        #endregion
    }
}
