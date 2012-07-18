namespace SqlLightning
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// Holds all the pertinate data for a stored procedure parameter. Basically a light
    /// wrapper over the SqlParameter class.
    /// </summary>
    public sealed class StoredProcParam
    {
        #region Fields & Constants

        /// <summary>
        /// Internal SqlParameter so that in/out, out, and return value parameter types
        /// work.
        /// </summary>
        private SqlParameter sqlParam;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Prevents a default instance of the <see cref="StoredProcParam"/> class from being created.
        /// Use factory methods instead.
        /// </summary>
        private StoredProcParam()
        {
            this.sqlParam = new SqlParameter();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the parameter's value.
        /// </summary>
        public object Value
        {
            get
            {
                return this.sqlParam.Value;
            }

            private set
            {
                this.sqlParam.Value = value;
            }
        }

        /// <summary>
        /// Gets the parameter's type.
        /// </summary>
        public DbType Type
        {
            get
            {
                return this.sqlParam.DbType;
            }

            private set
            {
                this.sqlParam.DbType = value;
            }
        }

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.sqlParam.ParameterName;
            }

            private set
            {
                this.sqlParam.ParameterName = value;
            }
        }

        /// <summary>
        /// Gets the parameter's direction.
        /// </summary>
        public ParameterDirection Direction
        {
            get
            {
                return this.sqlParam.Direction;
            }

            private set
            {
                this.sqlParam.Direction = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Allows conversion to a SqlParameter.
        /// </summary>
        /// <param name="param">The <see cref=" StoredProcParam"/> to cast.</param>
        /// <returns>A compatible SqlParameter instance.</returns>
        public static implicit operator SqlParameter(StoredProcParam param)
        {
            return param.sqlParam;
        }

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

        #endregion
    }
}
