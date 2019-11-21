using QuanLyCafe.DTO;
using System;
using System.Data;
using System.Linq;


namespace QuanLyCafe.DAO
{
    public class BillDAO
    {
        private static BillDAO instance;
        public static BillDAO Instance
        {
            get { if (instance == null) instance = new BillDAO(); return BillDAO.instance; }
            private set => instance = value;
        }
        private BillDAO() { }

        public void checkOut(int id, int discount, float totalPrice)
        {
            string query = " UPDATE dbo.Bill SET DateCheckOut = GETDATE(), status=1 ," + "discount = " + discount + ", totalPrice = " + totalPrice + " WHERE id =" + id;
            DataProvider.Instance.ExecuteNonQuery(query);
        }

        public int GetUncheckBillIDByTableID(int id)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * FROM dbo.Bill WHERE idTable =" + id + " AND status = 0");
            if (data.Rows.Count > 0)
            {
                Bill bill = new Bill(data.Rows[0]);
                return bill.ID;
            }
            else
            {
                return -1;
            }

        }

        public void InsertBill(int id)
        {
            string query = "EXEC dbo.USP_InsertBill @idTable";
            DataProvider.Instance.ExecuteQuery(query, new object[] { id });
        }

        public int GetMaxBill()
        {

            try //cố gắng thực hiện
            {
                return (int)DataProvider.Instance.ExecuteScalar("SELECT MAX(id) FROM dbo.Bill");
            }
            catch
            {
                return 1;
            }
        }

        public DataTable GetBillListByDate(DateTime checkIn, DateTime checkOut)
        {
            return DataProvider.Instance.ExecuteQuery("USP_GetListBillByDate @checkIn , @checkOut", new object[] {checkIn,checkOut });
        }

    }
}
