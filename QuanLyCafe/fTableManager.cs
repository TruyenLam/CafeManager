using QuanLyCafe.DAO;
using QuanLyCafe.DTO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using static QuanLyCafe.fAccountProfile;

namespace QuanLyCafe
{
    public partial class fTableManager : Form
    {
        private Account loginAccount;

        public Account LoginAccount 
        { 
            get => loginAccount;
            set => loginAccount = value;
        }

        public fTableManager(Account acc)
        {
            InitializeComponent();

            this.loginAccount = acc;
            changeAccount(acc.Type);
            loadTable();
            loadCategory();
            LoadcbTable(cbSwitchTable);
        }
        #region Method
        void changeAccount(int type)
        {
            adminToolStripMenuItem.Enabled = type == 1 ? true : false;
            thôngTinTàiKhoảnToolStripMenuItem.Text += " (" + LoginAccount.DisplayName + ")";
        }

        void loadCategory()
        {
            List<Category> listcategory = CategoryDAO.Instance.GetListCategory();
            cbCatagory.DataSource = listcategory;
            cbCatagory.DisplayMember = "Name";

        }

        void loadFoodListByCategoryID(int id)
        {
            List<Food> listfood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cbFood.DataSource = listfood;
            cbFood.DisplayMember = "Name";
        }


        void loadTable()
        {

            flpTable.Controls.Clear();
            List<Table> tablelist = TableDAO.Instance.LoadTableList();
            foreach (Table item in tablelist)
            {
                Button btn = new Button() { Width = TableDAO.TableWidth, Height = TableDAO.TableHeight };
                btn.Text = item.Name + Environment.NewLine + item.Status;
                btn.Click += btn_Click;
                btn.Tag = item; //gan id cho button
                //btn.Name = "table"+item.ID.ToString();
                switch (item.Status)
                {
                    case "Trống":
                        btn.BackColor = Color.White;
                        break;
                    default:
                        btn.BackColor = Color.LightGoldenrodYellow;
                        break;
                }
                flpTable.Controls.Add(btn);
            }
        }
        //load combox table phần chuyển bàn
        public void LoadcbTable(ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.LoadTableList();
            cb.DisplayMember = "Name";

        }

        //hiển thị bàn ăn chi tiết
        void ShowBill(int id)
        {
            lsvBill.Items.Clear();
            float totalPrice = 0;

            List<MenuFood> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);
            foreach (MenuFood item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());
                lsvItem.SubItems.Add(item.IdCategory.ToString());

                totalPrice += item.TotalPrice;

                lsvBill.Items.Add(lsvItem);
            }
            CultureInfo culture = new CultureInfo("vi-VN");

            txbTotalPrice.Text = totalPrice.ToString("c", culture);

        }

        #endregion

        #region Event
        //lấy id bill cho từng bàn ăn hiển thị lên chi tiết lên tren
        private void btn_Click(object sender, EventArgs e)
        {
            int TableID = ((sender as Button).Tag as Table).ID;
            lsvBill.Tag = (sender as Button).Tag;
            ShowBill(TableID);

        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAccountProfile f = new fAccountProfile(loginAccount);
            f.UpdateAccount += f_UpdateAccount;
            f.ShowDialog();
        }

        void f_UpdateAccount(object sender, AccountEvent e)
        {
            thôngTinTàiKhoảnToolStripMenuItem.Text = "Thông tin tài khoản (" + e.Acc.DisplayName + ")";
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAdmin f = new fAdmin();

            f.InsertFood += f_InsertFood;
            f.DeleteFood += f_DeleteFood;
            f.UpdateFood += f_UpdateFood;

            f.ShowDialog();

        }

        private void f_UpdateFood(object sender, EventArgs e)
        {
            loadFoodListByCategoryID((cbCatagory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);

        }

        private void f_DeleteFood(object sender, EventArgs e)
        {
            loadFoodListByCategoryID((cbCatagory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
            loadTable();
        }

        private void f_InsertFood(object sender, EventArgs e)
        {
            loadFoodListByCategoryID((cbCatagory.SelectedItem as Category).ID);
            if(lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        private void cbCatagory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;
            ComboBox cb = sender as ComboBox;

            if (cb.SelectedItem == null)
                return;

            Category selected = cb.SelectedItem as Category;
            id = selected.ID;
            loadFoodListByCategoryID(id);
        }
        private void btnAddFood_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            if(table == null)
            {
                MessageBox.Show("Bạn chưa chọn bàn !");
                return;
            }
            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int count = (int)nmFoodCount.Value;
            int idfood = (cbFood.SelectedItem as Food).Id;
            if (idBill == -1)
            {
                BillDAO.Instance.InsertBill(table.ID);

                BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxBill(), idfood, count);
            }
            else
            {
                //bill đã tồn tại thì lấy idBill
                //sử lý trên sql proc USP_insertBillInfo
                BillInfoDAO.Instance.InsertBillInfo(idBill, idfood, count);
            }
            ShowBill(table.ID);


            loadTable();
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int discount = (int)nmDisCount.Value;

            CultureInfo culture = new CultureInfo("vi-VN");
            NumberFormatInfo nbf = culture.NumberFormat;

            //double totalprice = Convert.ToDouble(txbTotalPrice.Text.Split(',')[0]);
            double totalprice = double.Parse(txbTotalPrice.Text.Split(',')[0], nbf);

            //double totalprice = Convert.ToDouble(txbTotalPrice.Text.Split(',')[0]);

            double finalprice = totalprice - (totalprice / 100) * discount;

            if (idBill != -1)
            {
                if (MessageBox.Show(string.Format("Bạn có chắt thanh toán hóa đơn cho bàn {0} \n Tổng tiền -(Tổng tiền/100) x Giảm Giá => {1} - ({1}/100) x {2}={3}", table.Name, totalprice, discount, finalprice), "Thông Báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    BillDAO.Instance.checkOut(idBill, discount,(float)finalprice);
                    ShowBill(table.ID);
                    loadTable();
                }
            }
        }
        //load category va food khi nhan doubleclick vao list bill
        private void lsvBill_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            if (lsvBill.SelectedItems.Count > 0)
            {
                string idca = lsvBill.SelectedItems[0].SubItems[4].Text.ToString();
                string namefood = lsvBill.SelectedItems[0].SubItems[0].Text.ToString();
                int idCategory = Convert.ToInt32(idca);

                Category name = CategoryDAO.Instance.GetCategory(idCategory);
                cbCatagory.SelectedIndex = cbCatagory.FindString(name.Name);
                cbFood.SelectedIndex = cbFood.FindString(namefood);
            }

        }
        private void btnSwitchTable_Click(object sender, EventArgs e)
        {
            int id1 = (lsvBill.Tag as Table).ID;
            int id2 = (cbSwitchTable.SelectedItem as Table).ID;

            if (MessageBox.Show(String.Format("Bạn có thật sự muốn chuyển bàn {0} qua bàn {1}", (lsvBill.Tag as Table).Name, (cbSwitchTable.SelectedItem as Table).Name), "Thông Báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                TableDAO.Instance.SwitchTable(id1, id2);
                loadTable();
            }

        }
        private void combine_table_Click(object sender, EventArgs e)
                {
                    int id1 = (lsvBill.Tag as Table).ID;
                    int id2 = (cbSwitchTable.SelectedItem as Table).ID;
                    if(MessageBox.Show(string.Format("Bạn có thật sự muốn GỘP {0} ĐẾN  {1}",(lsvBill.Tag as Table).Name,(cbSwitchTable.SelectedItem as Table).Name),"Thông Báo GỌP BÀN",MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                    {
                        TableDAO.Instance.combine_table(id1,id2);
                        loadTable();
                    }
                }

        #endregion
    }
}
