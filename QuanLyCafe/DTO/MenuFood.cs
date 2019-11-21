using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe.DTO
{
    public class MenuFood
    {
        public MenuFood(string foodName, int count, float price, float totalPrice = 0,int idCategory=0)
        {
            this.FoodName = foodName;
            this.Count = count;
            this.Price = price;
            this.TotalPrice = totalPrice;
            this.IdCategory = idCategory;
        }
        public MenuFood(DataRow Row)
        {
            this.FoodName = Row["name"].ToString();
            this.Count = (int)Row["count"];
            this.Price = (float)Convert.ToDouble(Row["price"].ToString());
            this.TotalPrice = (float)Convert.ToDouble(Row["TotalPrice"].ToString());
            this.IdCategory = (int)Row["idCategory"];
        }

        private string foodName;
        private int count;
        private float price;
        private float totalPrice;
        private int idCategory;

        public string FoodName { get => foodName; set => foodName = value; }
        public int Count { get => count; set => count = value; }
        public float Price { get => price; set => price = value; }
        public float TotalPrice { get => totalPrice; set => totalPrice = value; }
        public int IdCategory { get => idCategory; set => idCategory = value; }
    }
}
