using LIB.Constants;
using LIB.Entities;
using LIB.Entities.StoredData;

using SQLiteEngine.Helpers;
using SQLiteEngine.Proxy;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WPF_Core.Interfaces.DbBrowser;

namespace LIB.DbEngine
{
    public class RecipesProxy
    {
        private static SQLiteProxy proxy = SQLiteHelper.Proxy;


        #region Read
        public List<Server> GetData()
        {
            List<Server> data = new List<Server>();
            string Statement = "SELECT * FROM SERVER";
            var reader = proxy.GetDataReader(Statement);
            if (reader != null)
            {
                while (reader.Read())
                {
                    Server s = new Server()
                    {
                        Id = proxy.ConvertToInt(reader["ID"]),
                        Name = proxy.ConvertToString(reader["NAME"])
                    };
                    data.Add(s);
                }
            }

            foreach (var s in data)
                ReadMaterials(s);

            return data;
        }

        private void ReadMaterials(Server s)
        {
            s.Materials = new ObservableCollection<Material>();
            string Statement = "SELECT * FROM MATERIAL WHERE ServerId = @SERVER_ID";
            var reader = proxy.GetDataReader(Statement, "SERVER_ID", s.Id);
            if (reader != null)
            {
                while (reader.Read())
                {
                    Material m = new Material()
                    {
                        Id = proxy.ConvertToInt(reader["ID"]),
                        Name = proxy.ConvertToString(reader["NAME"]),
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
            var reader = proxy.GetDataReader(Statement, "MATERIAL_ID", m.Id);
            if (reader != null)
            {
                while (reader.Read())
                {
                    Product p = new Product()
                    {
                        Id = proxy.ConvertToInt(reader["ID"]),
                        Name = proxy.ConvertToString(reader["NAME"]),
                        Target = proxy.ConvertToInt(reader["TARGET"]),
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
            var reader = proxy.GetDataReader(Statement, "PRODUCT_ID", p.Id);
            if (reader != null)
            {
                while (reader.Read())
                {
                    p.FinalSeq.Add(GetMove(proxy.ConvertToInt(reader["DELTA"])));
                }
            }
        }

        private Move GetMove(int delta)
        {
            return Cnst.Moves.FirstOrDefault(m => m.Delta == delta);
        }
        #endregion

        #region Insert
        public bool InsertServer(Server s)
        {
            string Statement = "INSERT INTO Server (Id, Name) VALUES (@Id, @Name);";
            s.Id = proxy.GetNextIntValue("Server", "Id");
            IParametersBase param = SQLiteHelper.GetParameterInstance();
            param.Add("Id", s.Id);
            param.Add("Name", s.Name);

            return proxy.Execute(Statement, param);
        }

        public bool InsertMaterial(Material m)
        {
            string Statement = "INSERT INTO Material (Id, Name, ServerId) VALUES (@Id, @Name, @ServerId);";
            m.Id = proxy.GetNextIntValue("Material", "Id");
            IParametersBase param = SQLiteHelper.GetParameterInstance();
            param.Add("Id", m.Id);
            param.Add("Name", m.Name);
            param.Add("ServerId", m.ServerId);

            return proxy.Execute(Statement, param);
        }

        public bool InsertProduct(Product p)
        {
            bool res = true;
            string Statement = "INSERT INTO Product (Id, Name, Target, MaterialId) VALUES (@Id, @Name, @Target, @MaterialId);";
            p.Id = proxy.GetNextIntValue("Product", "Id");
            IParametersBase param = SQLiteHelper.GetParameterInstance();
            param.Add("Id", p.Id);
            param.Add("Name", p.Name);
            param.Add("Target", p.Target);
            param.Add("MaterialId", p.MaterialId);

            var trans = proxy.GetTransaction();

            try
            {
                res &= proxy.Execute(Statement, param, trans, false);

                for (int i = 0; i < p.FinalSeq.Count; i++)
                    res &= InsertMove(trans, p.FinalSeq[i], i + 1, p.Id);
            }
            catch (Exception ex)
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
            IParametersBase param = SQLiteHelper.GetParameterInstance();
            param.Add("Id", proxy.GetNextIntValue("Move", "Id"));
            param.Add("Delta", move.Delta);
            param.Add("ProductId", pId);
            param.Add("Seq", Seq);

            return proxy.Execute(Statement, param, trans, false);
        }
        #endregion

        #region Delete
        public bool DeleteServer(Server s)
        {
            string Statement = "Delete FROM Server Where Id = @Id";
            IDbTransaction trans = proxy.GetTransaction();

            bool res = proxy.Execute(Statement, "Id", s.Id, trans, false);

            foreach (var mat in s.Materials)
                res &= DeleteMaterial(mat, trans, false);

            if (res)
                trans.Commit();
            else trans.Rollback();

            return res;
        }

        public bool DeleteMaterial(Material m, IDbTransaction trans = null, bool commit = true)
        {
            string Statement = "Delete from Material where Id = @Id";
            bool res = proxy.Execute(Statement, "Id", m.Id, trans, commit);

            foreach (var prod in m.Products)
                res &= DeleteProduct(prod, trans, commit);

            return res;
        }
        public bool DeleteProduct(Product p, IDbTransaction trans = null, bool commit = true)
        {
            string Statement = "Delete from product Where Id = @Id";
            bool res = proxy.Execute(Statement, "Id", p.Id, trans, commit);

            for (int i = 0; i < p.FinalSeq.Count; i++)
                res &= DeleteMove(p.FinalSeq[i], i + 1, p.Id, trans);

            return res;
        }

        private bool DeleteMove(Move move, int Seq, int pId, IDbTransaction trans)
        {
            string Statement = "Delete from move where Seq = @Seq and ProductId = @ProductId";
            IParametersBase param = SQLiteHelper.GetParameterInstance();
            param.Add("Seq", Seq);
            param.Add("ProductId", pId);

            return proxy.Execute(Statement, param, trans, false);
        }
        #endregion
    }
}
