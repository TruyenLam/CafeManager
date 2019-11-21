using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe.DTO
{
    public class BillInfo
    {
        private BillInfo(int id, int billID, int foodID, int count)
        {
            this.Id = id;
            this.BillID = billID;
            this.FoodID = foodID;
            this.Count = count;
        }
        public BillInfo(DataRow row )
        {
            this.Id = (int)row["id"];
            this.BillID = (int)row["idBill"];
            this.FoodID = (int)row["idFood"];
            this.Count = (int)row["count"];
        }
        
        private int id;
        private int BillID;
        private int FoodID;
        private int count;

        public int Id { get => id; set => id = value; }
        public int BillID1 { get => BillID; set => BillID = value; }
        public int FoodID1 { get => FoodID; set => FoodID = value; }
        public int Count { get => count; set => count = value; }
    }
}
