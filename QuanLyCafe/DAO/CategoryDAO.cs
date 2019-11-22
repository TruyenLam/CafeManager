using QuanLyCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe.DAO
{
    public class CategoryDAO
    {
        private static CategoryDAO instance;

        public static CategoryDAO Instance 
        {
            get { if (instance == null) instance = new CategoryDAO();return CategoryDAO.instance; }
            
            private set => instance = value; 
        }
        private CategoryDAO() { }

        public List<Category> GetListCategory()
        {
            List<Category> listCategory = new List<Category>();
            string query= "SELECT * FROM dbo.FoodCategory";
            DataTable data = DataProvider.Instance.ExecuteQuery(query);
            foreach(DataRow item in data.Rows)
            {
                Category category = new Category(item);
                listCategory.Add(category); 

            }
            return listCategory;
        }
        public Category GetCategory(int id)
        {
            
            string query = "SELECT * FROM dbo.FoodCategory WHERE id = "+id;
            DataTable data = DataProvider.Instance.ExecuteQuery(query);
            Category nameCategory = new Category(data.Rows[0]);

            return nameCategory;
        }


        public bool UpdateCategory(int id, string name)
        {
            string query = string.Format("UPDATE dbo.FoodCategory SET	name=N'{0}' WHERE id={1}", name, id);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
        public bool InsertCategory(string name)
        {
            string query = string.Format("INSERT dbo.FoodCategory (name) VALUES(N'{0}')", name);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
        public bool DeleteCategory(int id)
        {
            string query= string.Format("DELETE dbo.FoodCategory WHERE id ={0}", id);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

    }
}
