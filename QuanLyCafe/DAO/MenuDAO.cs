using QuanLyCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyCafe.DAO
{
    public class MenuDAO
    {
        private static MenuDAO instance;

        public static MenuDAO Instance 
        {
            get { if (instance == null) instance = new MenuDAO();return MenuDAO.instance; }
            private set => instance = value;  
        }
        private MenuDAO() { }

        public List<MenuFood> GetListMenuByTable(int id)
        {
            List<MenuFood> listMenu = new List<MenuFood>();
            String query = "SELECT f.name,bi.count,f.price,f.price*bi.count AS TotalPrice, f.idCategory FROM dbo.BillInfo AS bi,dbo.Bill AS b,dbo.Food f WHERE bi.idBill =b.id AND bi.idFood =f.id AND b.status=0 AND b.idTable=" + id;
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach(DataRow item in data.Rows)
            {
                MenuFood menu = new MenuFood(item);
                listMenu.Add(menu);

            }

            return listMenu;
        }
    }
}
