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

        #region Read
        public List<Server> GetData()
        {
            List<Server> data = new List<Server>(); 
            string Statement = "SELECT * FROM SERVER";
            var reader = GetDataReader(Statement);
            if(reader != null)
            {
                while(reader.Read())
                {
                    Server s = new Server()
                    {
                        Id = ConvertToInt(reader["ID"]),
                        Name = ConvertToString(reader["NAME"])
                    };
                    data.Add(s);
                }
            }

            foreach(var s in data)
                ReadMaterials(s);

            return data;
        }

        private void ReadMaterials(Server s)
        {
            s.Materials = new ObservableCollection<Material>();
            string Statement = "SELECT * FROM MATERIAL WHERE ServerId = @SERVER_ID";
            var reader = GetDataReader(Statement, "SERVER_ID", s.Id);
            if(reader != null)
            {
                while(reader.Read())
                {
                    Material m = new Material()
                    {
                        Id = ConvertToInt(reader["ID"]),
                        Name = ConvertToString(reader["NAME"]),
                        ServerId = s.Id
                    };
                    s.Materials.Add(m);
                }
            }

            foreach (var m in s.Materials)
                ReadProducts(m);
        }

        private void ReadProducts(Material m)
        {
            m.Products = new ObservableCollection<Product>();
            string Statement = "SELECT * FROM PRODUCT WHERE MaterialId = @MATERIAL_ID";
            var reader = GetDataReader(Statement, "MATERIAL_ID", m.Id);
            if(reader != null)
            {
                while(reader.Read())
                {
                    Product p = new Product()
                    {
                        Id = ConvertToInt(reader["ID"]),
                        Name = ConvertToString(reader["NAME"]),
                        Target = ConvertToInt(reader["TARGET"]),
                        MaterialId = m.Id
                    };
                    m.Products.Add(p);
                }
            }

            foreach (var p in m.Products)
                ReadMoves(p);
        }

        private void ReadMoves(Product p)
        {
            p.FinalSeq = new ObservableCollection<Move>();
            string Statement = "SELECT * from MOVE WHERE ProductId = @PRODUCT_ID ORDER BY SEQ";
            var reader = GetDataReader(Statement, "PRODUCT_ID", p.Id);
            if(reader != null)
            {
                while(reader.Read())
                {
                    p.FinalSeq.Add(GetMove(ConvertToInt(reader["DELTA"])));
                }
            }
        }

        private Move GetMove(int delta)
        {
            return Consts.Moves.FirstOrDefault(m => m.Delta == delta);
        }
        #endregion

        #region Insert
        public bool InsertServer(Server s)
        {
            string Statement = "INSERT INTO Server (Id, Name) VALUES (@Id, @Name);";
            SQLiteParameters param = new SQLiteParameters();
            param.Add("Id", s.Id);
            param.Add("Name", s.Name);

            return Execute(Statement, param);
        }

        public bool InsertMaterial(Material m)
        {
            string Statement = "INSERT INTO Material (Id, Name, ServerId) VALUES (@Id, @Name, @ServerId);";
            SQLiteParameters param = new SQLiteParameters();
            param.Add("Id", m.Id);
            param.Add("Name", m.Name);
            param.Add("ServerId", m.ServerId);

            return Execute(Statement, param);
        }

        public bool InsertProduct(Product p)
        {
            bool res = true;
            string Statement = "INSERT INTO Product (Id, Name, Target, MaterialId) VALUES (@Id, @Name, @Target, @MaterialId);";
            SQLiteParameters param = new SQLiteParameters();
            param.Add("Id", p.Id);
            param.Add("Name", p.Name);
            param.Add("Target", p.Target);
            param.Add("MaterialId", p.MaterialId);

            var trans = GetTransaction();

            try
            {
                res &= Execute(Statement, param, trans, false);

                for (int i = 0; i < p.FinalSeq.Count; i++)
                    res &= InsertMove(trans, p.FinalSeq[i], i + 1, p.Id);
            }catch(Exception ex)
            {

            }

            if (res)
                trans.Commit();
            else trans.Rollback();

                return res;
        }

        private bool InsertMove(IDbTransaction trans, Move move, int Seq, int pId)
        {
            string Statement = "INSERT INTO Move (Id, Delta, ProductId, SEQ) VALUES (@Id, @Delta, @ProductId, @Seq);";
            SQLiteParameters param = new SQLiteParameters();
            param.Add("Id", GetNextIntValue("Move", "Id"));
            param.Add("Delta", move.Delta);
            param.Add("ProductId", pId);
            param.Add("Seq", Seq);

            return Execute(Statement, param, trans, false);
        }
        #endregion
    }
}
