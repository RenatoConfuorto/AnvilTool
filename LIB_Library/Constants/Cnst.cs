using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Constants
{
    public class Cnst
    {
        #region User Data
        /// <summary>
        /// Name of the folder where the application data is stored
        /// </summary>
        public static readonly string ApplicationFolderDataName = "Anvil Tool";
        /// <summary>
        /// Full Path of the application data folder
        /// </summary>
        public static readonly string ApplicationFolderDataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"\\{ApplicationFolderDataName}";
        #endregion
    }
}
