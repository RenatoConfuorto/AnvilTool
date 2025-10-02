using LIB.Entities;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Constants
{
    public class Cnst
    {
        public const int MIN_POS = 0;
        public const int MAX_POS = 150;

        public const int MAX_ITER = 100;
        public const int MAX_FINAL_SEQ_NUM = 3;

        //DirectStepCalcComputeShortest
        public const int MAX_SEQ_LENGHT = 15;       // Maximum allowed number of elements accepted in the sequece
        public const int EXIT_SEQ_LEN = 6;          // If a sequence has less than this number, than is automatically accepted
        public const int START_SEQ_TOL = 2;

        public static readonly ObservableCollection<Move> Moves = new ObservableCollection<Move>()
        {
            { new Move {MoveEn = MovesEnum.LightHit, Label = "Light Hit", Delta = -3 } },
            { new Move {MoveEn = MovesEnum.MediumHit, Label = "Medium Hit", Delta = -6 } },
            { new Move {MoveEn = MovesEnum.Punch, Label = "Punch", Delta = +2 } },
            { new Move {MoveEn = MovesEnum.Bend, Label = "Bend", Delta = +7 } },
            { new Move {MoveEn = MovesEnum.HardHit, Label = "Hard hit", Delta = -9 } },
            { new Move {MoveEn = MovesEnum.Draw, Label = "Draw", Delta = -15 } },
            { new Move {MoveEn = MovesEnum.Upset, Label = "Upset", Delta = +13 } },
            { new Move {MoveEn = MovesEnum.Shrink, Label = "Shrink", Delta = +16 } },
        };

        public enum MovesEnum
        {
            None = -1,
            LightHit,
            MediumHit,
            HardHit,
            Draw,
            Punch,
            Bend,
            Upset,
            Shrink,
        }

        public enum RecipesMode
        {
            SelectRecipe,
            SaveRecipe
        }

        /// <summary>
        /// Name of the folder where the application data is stored
        /// </summary>
        public static readonly string ApplicationFolderDataName = "Anvil Tool";
        /// <summary>
        /// Name of the data file
        /// </summary>
        public static string ApplicationDataFileName
        {
            get
            {
#if DEBUG
                return "Test_Data.db";
#else
                return "Data.db";
#endif

            }
        }
        /// <summary>
        /// Name of the downloaded images files folder
        /// </summary>
        public static string ApplicationFolderImagesPath
        {
            get => "Images";
        }
        public static string ApplicationCreateTableScript
        {
            get => "pack://application:,,,/LIB_Library;component/DbEngine/TablesScript.sql";
        }
        /// <summary>
        /// Full Path of the application data folder
        /// </summary>
        public static readonly string ApplicationFolderDataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"\\{ApplicationFolderDataName}";
        /// <summary>
        ///  Full path of the application data file
        /// </summary>
        public static readonly string ApplicationDataFilePath = $"{ApplicationFolderDataPath}\\{ApplicationDataFileName}";
    }
}
