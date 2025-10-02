using AnvilTool.Constants;
using AnvilTool.Entities;
using AnvilTool.Entities.StoredData;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WPF_Core.Interfaces.DbBrowser;
using WPF_Core.Proxy;

namespace AnvilTool.DbEngine
{
    public class SQLiteProxy : Base.ProxyCore
    {
        public SQLiteProxy(string _connectionString)
            : base(_connectionString)
        {

        }
        #region Base
        public override bool Execute(string Statement, IParametersBase parameters)
        {
            return Execute(Statement, parameters, null, true);
        }

        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }

        protected override void AddQueryParametersValue(IDataParameterCollection _commandParameters, IParametersBase _parametersValues)
        {
            if (_commandParameters is SQLiteParameterCollection parameterCollection)
            {
                List<string> _parKeys = _parametersValues.GetParametersKeys();
                foreach (string _parKey in _parKeys)
                {
                    parameterCollection.AddWithValue(_parKey, _parametersValues[_parKey]);
                }
            }
        }
        #endregion

    }
}
