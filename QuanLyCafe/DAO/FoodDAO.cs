﻿using QuanLyCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe.DAO
{
    public class FoodDAO
    {
        private static FoodDAO instance;

        public static FoodDAO Instance 
        {
            get { if (instance == null) instance = new FoodDAO();return FoodDAO.instance; }
            private set => instance = value; 
        }
        private FoodDAO() { }

        public List<Food> GetFoodByCategoryID(int id)
        {
            string query = " SELECT * FROM dbo.Food WHERE idCategory= "+id;
            List<Food> listFood = new List<Food>();
            DataTable data = DataProvider.Instance.ExecuteQuery(query);
            foreach (DataRow item in data.Rows)
            {
                Food food = new Food(item);
                listFood.Add(food);
            }
            return listFood;
        }
        public Food GetFoodByID(int id)
        {
            string query = " SELECT * FROM dbo.Food WHERE ID= " + id;
            
            DataTable data = DataProvider.Instance.ExecuteQuery(query);
            Food listFood = new Food(data.Rows[0]);
            return listFood;
        }
        public List<Food> GetListFood()
        {
            string query = "SELECT * FROM dbo.Food";
            List<Food> listFood = new List<Food>();
            DataTable data = DataProvider.Instance.ExecuteQuery(query);
            foreach(DataRow item in data.Rows)
            {
                Food food = new Food(item);
                listFood.Add(food);
            }
            return listFood;
        }
        public bool InsertFood(string name, int idCategory, float price)
        {
            string query = string.Format("INSERT dbo.Food ( name, idCategory, price ) VALUES  ( N'{0}',{1},{2})",name,idCategory,price);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
       public bool UpdatFood(int id,string name, int idCategory,float price)
        {
            string query = string.Format("UPDATE dbo.Food SET name =N'{0}',idCategory = {1},price = {2} WHERE id={3}",name,idCategory,price,id);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
        public bool DeleteFood(int idFood)
        {
            // phải xoa billInfo trước khi xoa Food

            string query = string.Format("Delete Food where id = {0}", idFood);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }
    }
}