using AnvilTool.Entities;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using WPF_Core.Dependency;
using WPF_Core.Helpers;
using WPF_Core.Interfaces.DbBrowser;

namespace AnvilTool.DbEngine.Base
{
    public abstract class ProxyCore : IProxyBase
    {
        private string _connectionString;

        private IDbConnection _connection;

        //private static IUnityContainer _container = UnityHelper.Current.GetLocalContainer();

        //
        // Riepilogo:
        //     Connection string with Db
        protected string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            private set
            {
                _connectionString = value;
            }
        }

        //
        // Riepilogo:
        //     Connection instance with Db
        public IDbConnection Connection
        {
            get
            {
                return _connection;
            }
            protected set
            {
                _connection = value;
            }
        }

        //
        // Riepilogo:
        //     Creates a new instance of the class
        //
        // Parametri:
        //   connectionString:
        //     Connection string
        public ProxyCore(string connectionString)
        {
            try
            {
                ConnectionString = connectionString;
                Connection = CreateConnection(connectionString);
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message, "Proxy Core - Init", MessageBoxButton.OK, MessageBoxImage.Hand);
                //LoggerHelper.GetSystemLogger()?.LogError(ex.Message, ".ctor");
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Proxy Core - Init", MessageBoxButton.OK, MessageBoxImage.Hand);
                //LoggerHelper.GetSystemLogger()?.LogError(ex2.Message, ".ctor");
            }
        }

        //
        // Riepilogo:
        //     Creates the connection to Db for the proxy
        //
        // Parametri:
        //   connectionString:
        protected abstract IDbConnection CreateConnection(string connectionString);

        public virtual void OpenConnection()
        {
            if (Connection == null)
            {
                throw new ArgumentNullException("Connection");
            }

            Connection.Open();
        }

        public virtual void CloseConnection()
        {
            if (Connection == null)
            {
                throw new ArgumentNullException("Connection");
            }

            Connection.Close();
        }

        public virtual int GetNextIntValue(string TableName, string FieldName, string whereCondition = null)
        {
            int result = 0;
            if (Connection == null)
            {
                throw new NullReferenceException("Connection is null");
            }

            string text = "SELECT (MAX(" + FieldName + ") + 1) AS MAX FROM " + TableName + " ";
            if (!string.IsNullOrWhiteSpace(whereCondition))
            {
                text += whereCondition;
            }

            IDataReader dataReader = GetDataReader(text);
            if (dataReader != null)
            {
                dataReader.Read();
                result = ConvertToInt(dataReader["MAX"], 1);
            }

            return result;
        }

        public bool CheckIntValue(string TableName, string FieldName, int IntValue, string whereCondition = null)
        {
            bool result = false;
            if (Connection == null)
            {
                throw new NullReferenceException("Connection is null");
            }

            string text = "SELECT " + FieldName + " AS CHECK_FIELD FROM " + TableName + " WHERE " + FieldName + " = @VALUE";
            if (!string.IsNullOrWhiteSpace(whereCondition))
            {
                text = text + " AND " + whereCondition;
            }

            IDataReader dataReader = GetDataReader(text, "@VALUE", IntValue);
            if (dataReader != null)
            {
                result = dataReader.Read();
            }

            return result;
        }

        public virtual IDbTransaction GetTransaction()
        {
            if (Connection == null)
            {
                throw new ArgumentNullException("Connection");
            }

            return Connection.BeginTransaction();
        }

        public virtual bool Execute(string Statement, IParametersBase parameters)
        {
            return Execute(Statement, parameters, null);
        }

        public virtual bool Execute(string Statement, IParametersBase parameters, IDbTransaction transaction, bool commit = true)
        {
            bool result = false;
            try
            {
                using (IDbCommand dbCommand = Connection.CreateCommand())
                {

                    if (transaction != null)
                    {
                        dbCommand.Transaction = transaction;
                    }
                    else
                    {
                        dbCommand.Transaction = Connection.BeginTransaction();
                    }
                    dbCommand.CommandText = Statement;
                    if (parameters != null && parameters.Count() > 0)
                    {
                        AddQueryParametersValue(dbCommand.Parameters, parameters);
                    }

                    if (dbCommand.ExecuteNonQuery() > 0)
                    {
                        result = true;
                    }

                    if (commit)
                    {
                        dbCommand.Transaction.Commit();
                    }
                }
            }
            catch (Exception innerException)
            {
                throw new Exception("Error in SQLiteProxy.Execute", innerException);
            }

            return result;
        }

        public virtual bool Execute(string Statement, string parameterName, object parameterValue)
        {
            return Execute(Statement, parameterName, parameterValue, null);
        }

        public virtual bool Execute(string Statement, string parameterName, object parameterValue, IDbTransaction transaction, bool commit = true)
        {
            //IParametersBase parametersBase = UnityContainerExtensions.Resolve<IParametersBase>(_container, Array.Empty<ResolverOverride>());
            IParametersBase parametersBase = new SQLiteParameters();
            parametersBase.Add(parameterName, parameterValue);
            return Execute(Statement, parametersBase, null);
        }

        public virtual bool Execute(string Statement)
        {
            return Execute(Statement, null, null);
        }

        public virtual bool Execute(string Statement, IDbTransaction transaction, bool commit = true)
        {
            return Execute(Statement, null, transaction, commit);
        }

        public virtual IDataReader GetDataReader(string Statement, IParametersBase parameters)
        {
            IDataReader result = null;
            using (IDbCommand dbCommand = Connection.CreateCommand())
            {
                dbCommand.CommandText = Statement;
                if (parameters != null && parameters.Count() > 0)
                {
                    AddQueryParametersValue(dbCommand.Parameters, parameters);
                }

                result = dbCommand.ExecuteReader();
            }

            return result;
        }

        public virtual IDataReader GetDataReader(string Statement, string parameterName, object parameterValue)
        {
            //IParametersBase parametersBase = UnityContainerExtensions.Resolve<SQLite>(_container, Array.Empty<ResolverOverride>());
            IParametersBase parametersBase = new SQLiteParameters();
            parametersBase.Add(parameterName, parameterValue);
            return GetDataReader(Statement, parametersBase);
        }

        public virtual IDataReader GetDataReader(string Statement)
        {
            return GetDataReader(Statement, null);
        }

        public int ConvertToInt(object value, int defaultValue = 0)
        {
            if (value != DBNull.Value)
            {
                return Convert.ToInt32(value);
            }

            return defaultValue;
        }

        public int? ConvertToNullableInt(object value)
        {
            if (value != DBNull.Value)
            {
                return Convert.ToInt32(value);
            }

            return null;
        }

        public long ConvertToLong(object value, long defaultValue = 0L)
        {
            if (value != DBNull.Value)
            {
                return Convert.ToInt64(value);
            }

            return defaultValue;
        }

        public long? ConvertToNullableLong(object value)
        {
            if (value != DBNull.Value)
            {
                return Convert.ToInt64(value);
            }

            return null;
        }

        public float ConvertToFloat(object value, float defaultValue = 0f)
        {
            if (value != DBNull.Value)
            {
                return Convert.ToSingle(value);
            }

            return defaultValue;
        }

        public float? ConvertToNullableFloat(object value)
        {
            if (value != DBNull.Value)
            {
                return Convert.ToSingle(value);
            }

            return null;
        }

        public double ConvertToDouble(object value, double defaultValue = 0.0)
        {
            if (value != DBNull.Value)
            {
                return Convert.ToDouble(value);
            }

            return defaultValue;
        }

        public double? ConvertToNullableDouble(object value)
        {
            if (value != DBNull.Value)
            {
                return Convert.ToDouble(value);
            }

            return null;
        }

        public string ConvertToString(object value)
        {
            if (value != DBNull.Value)
            {
                return Convert.ToString(value);
            }

            return null;
        }

        public bool ConvertToBoolean(object value)
        {
            if (value != DBNull.Value)
            {
                return Convert.ToBoolean(value);
            }

            return false;
        }

        public bool? ConvertToNullableBoolean(object value)
        {
            if (value != DBNull.Value)
            {
                return Convert.ToBoolean(value);
            }

            return null;
        }

        public DateTime ConvertToDateTime(object value)
        {
            if (value != DBNull.Value)
            {
                return Convert.ToDateTime(value);
            }

            return DateTime.MinValue;
        }

        public DateTime? ConvertToNullableDateTime(object value)
        {
            if (value != DBNull.Value)
            {
                return Convert.ToDateTime(value);
            }

            return null;
        }

        public byte[] ConvertToBytes(object value)
        {
            if (value != DBNull.Value)
            {
                return value as byte[];
            }

            return null;
        }

        //
        // Riepilogo:
        //     Define the logics to add the parameters value to the System.Data.IDbCommand parameters
        //     collection according to the Db type used
        //
        // Parametri:
        //   _commandParameters:
        //     Command parameters
        //
        //   _parametersValues:
        //     Parameters given by the user
        //
        // Commenti:
        //     When the method is called a check for null reference and element present in collection
        //     has already been performed for _parametersValues argument
        protected abstract void AddQueryParametersValue(IDataParameterCollection _commandParameters, IParametersBase _parametersValues);
    }
}
