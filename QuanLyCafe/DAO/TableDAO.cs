using QuanLyCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace QuanLyCafe.DAO
{
    public class TableDAO
    {
        private static TableDAO instance;

        public static TableDAO Instance
        {
            get { if (instance == null) instance = new TableDAO(); return TableDAO.instance; }
            private set => instance = value;
        }
        private TableDAO() { }

        public static int TableWidth = 100;
        public static int TableHeight = 100;

        public List<Table> LoadTableList()
        {
            List<Table> tableList = new List<Table>();
            DataTable data = DataProvider.Instance.ExecuteQuery("USP_GetTableList");
            foreach (DataRow item in data.Rows)
            {
                Table table = new Table(item);
                tableList.Add(table);
            }
            return tableList;
        }

        public void SwitchTable(int id1, int id2)
        {
            DataProvider.Instance.ExecuteQuery("USP_SwitchTable @idTable1 , @idtable2", new object[] { id1, id2 });
        }
        public void combine_table(int id1, int id2)
        {
            DataProvider.Instance.ExecuteQuery("USP_CombineTable @idTable1 , @idTable2", new object[] { id1, id2 });
        }
        public Table GetTableByID(int id)
        {
            string query = " SELECT * FROM dbo.TableFood WHERE id=" + id;

            DataTable data = DataProvider.Instance.ExecuteQuery(query);
            Table listTable = new Table(data.Rows[0]);        
           
            return listTable;
        }
        public bool InsertTable(string name,string status)
        {
            string query = string.Format("INSERT dbo.TableFood(name,status)VALUES(N'{0}',N'{1}')", name, status);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
        public bool UpdateTable(int id, string name, string status)
        {
            string query=string.Format("UPDATE dbo.TableFood SET name =N'{0}',status=N'{1}' WHERE id ={2}",name, status,id);
            int result= DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
        public bool DeleteTable(int id, string name, string status)
        {
            int result=0;
            return result > 0;
        }
    }
}
