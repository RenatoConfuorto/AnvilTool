using AnvilTool.Constants;
using AnvilTool.DbEngine;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AnvilTool.Helpers
{
    public class SQLiteHelper
    {
        public static SQLiteProxy Proxy { get; private set; }
        public static void CheckDataFile()
        {
            CheckDataFileExists();
        }
        private static void CheckDataFileExists()
        {
            if (!Directory.Exists(Consts.ApplicationFolderDataPath))
                Directory.CreateDirectory(Consts.ApplicationFolderDataPath);

            bool fileCheck = File.Exists(Consts.ApplicationDataFilePath);
            Proxy = new SQLiteProxy($"Data Source = {Consts.ApplicationDataFilePath}");
            Proxy.OpenConnection();
            //File has been created by DbConnect.Open(), create tables;
            if (!fileCheck)
                CreateTables(Proxy);
            //Proxy.CloseConnection();
        }

        public static void CloseConnection()
        {
            Proxy.CloseConnection();
            //Proxy.Dispose();
        }

        private static void CreateTables(SQLiteProxy proxy)
        {
            string CreateTablesStatement = GetCreateTablesStatement();
            proxy.Execute(CreateTablesStatement);
        }

        private static string GetCreateTablesStatement()
        {
            string result = String.Empty;
            Uri scriptFile = new Uri("pack://application:,,,/AnvilTool;component/DbEngine/TablesScript.sql");
            //Uri scriptFile = new Uri("../DbEngine/TablesScript.sql", UriKind.Relative);
            var streamInfo = Application.GetResourceStream(scriptFile);
            if (streamInfo != null)
            {
                using (var sr = new StreamReader(streamInfo.Stream))
                {
                    result = sr.ReadToEnd();
                }
            }

            return result;
        }
    }
}
