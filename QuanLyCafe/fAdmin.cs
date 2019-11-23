using QuanLyCafe.DAO;
using QuanLyCafe.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace QuanLyCafe
{
    public partial class fAdmin : Form
    {
        BindingSource foodList = new BindingSource();
        BindingSource CategoryList = new BindingSource();
        BindingSource TableList = new BindingSource();
        public fAdmin()
        {
            InitializeComponent();
            Loadform();
        }

        #region Method
        void Loadform()
        {
            dtgvFood.DataSource = foodList;
            dtgvCategory.DataSource = CategoryList;
            dtgvTable.DataSource = TableList;
            LoadDateTimePickerBill();

            LoadListBillByDay(dtpkFromDate.Value, dtpkToDate.Value);

            LoadListFood();

            LoadListTable();

            LoadListCategory();

            AddFoodBinding();

            AddCategoryBinding();

            AddTableBinding();

            LoadCategoryIntoCombobox(cbFoodCategory);
        }
        void AddFoodBinding()
        {
            txbFoodID.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Id"));
            txbFoodName.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "name"));
            nmFoodPrice.DataBindings.Add(new Binding("Value", dtgvFood.DataSource, "Price"));

        }
        void AddCategoryBinding()
        {
            txbCategoryID.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "id"));
            txbCategoryName.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource,"name"));
        }
        void AddTableBinding()
        {
            txbTableID.DataBindings.Add(new Binding("Text", dtgvTable.DataSource,"ID"));
            txbTableName.DataBindings.Add(new Binding("Text",dtgvTable.DataSource,"name"));
            cbTableStatus.SelectedIndex = 0;
            
        }
        void LoadDateTimePickerBill()
        {
            DateTime today = DateTime.Now;
            dtpkFromDate.Value = new DateTime(today.Year, today.Month, 1);
            dtpkToDate.Value = dtpkFromDate.Value.AddMonths(1).AddDays(-1);
        }
        void LoadListBillByDay(DateTime checkIn, DateTime checkOut)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetBillListByDate(checkIn, checkOut);
        }
        void LoadListFood()
        {
            foodList.DataSource = FoodDAO.Instance.GetListFood();
        }
        void LoadListTable()
        {
            TableList.DataSource = TableDAO.Instance.LoadTableList(); ;
        }
        void LoadListCategory()
        {
            CategoryList.DataSource = CategoryDAO.Instance.GetListCategory();
        }
        void LoadCategoryIntoCombobox(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "name";
        }
       
        #endregion

        #region Event
        private void BtnViewBill_Click(object sender, EventArgs e)
        {
            LoadListBillByDay(dtpkFromDate.Value, dtpkToDate.Value);
        }

        private void btnViewFood_Click(object sender, EventArgs e)
        {
            LoadListFood();
        }

        private void txbFoodID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtgvFood.SelectedCells.Count > 0)
                {
                    int id = (int)dtgvFood.SelectedCells[0].OwningRow.Cells["Categoryid"].Value;
                    Category category = CategoryDAO.Instance.GetCategory(id);
                    cbFoodCategory.SelectedItem = category;
                    int index = -1;
                    int i = 0;
                    foreach (Category item in cbFoodCategory.Items)
                    {
                        if (item.ID == category.ID)
                        {
                            index = i;
                            break;
                        }
                        i++;
                    }
                    cbFoodCategory.SelectedIndex = index;
                }
            }
            catch { }

        }
        private void txbTableID_TextChanged(object sender, EventArgs e)
        {
           if(dtgvTable.SelectedCells.Count >0)
            {
              
                int id = (int)dtgvTable.SelectedCells[0].OwningRow.Cells["idTable"].Value;
                Table tablelist = TableDAO.Instance.GetTableByID(id);
                string status = tablelist.Status;
                
                cbTableStatus.SelectedIndex = cbTableStatus.FindString(status);
                    
            }
        }
        private void btnAddFood_Click(object sender, EventArgs e)
        {
            //try
            //{
            //int id=0;
            string name = txbFoodName.Text;

            int idCategory = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;
            if(foodList.Count <0)
            {
                MessageBox.Show("chưa có món nào","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            //kiểm tra food đã có hay chưa
            else 
            {
                
                //id = Convert.ToInt32(txbFoodID.Text);
                List<Food> checkfood = FoodDAO.Instance.GetFoodByNameAndCategoryID(name,idCategory);
                foreach(Food item in checkfood)
                {
                    if (name == item.Name.ToString())
                    {
                        MessageBox.Show("Tên món đã có trong danh mục vui long đặt tên khác");
                        return;
                    }//ket thuc kiểm tra nếu đã có kết thuc
                }
                if (FoodDAO.Instance.InsertFood(name, idCategory, price))
                {
                    MessageBox.Show("Thêm món thành công");
                    LoadListFood();

                    if (insertFood != null)
                    {
                        insertFood(this, new EventArgs());
                    }
                }
                else
                {
                    MessageBox.Show("Thêm món thất bại");
                }
            }
            
                
                
            
            //}
            //catch { }

        }

        private void btnEditFood_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txbFoodName.Text;
                int id = Convert.ToInt32(txbFoodID.Text);
                int idCategory = (cbFoodCategory.SelectedItem as Category).ID;
                float price = (float)nmFoodPrice.Value;

                if (FoodDAO.Instance.UpdatFood(id, name, idCategory, price))
                {
                    MessageBox.Show("Cập nhật món thành công");
                    if (chkCategory.Checked)
                    {
                        LoadSearchFoodByCategory(idCategory);
                    }
                    else
                    {
                        LoadListFood();
                    }
                    //event
                    if (updateFood != null)
                    {
                        updateFood(this, new EventArgs());
                    }
                }
                else
                {
                    MessageBox.Show("Sữa món thất bại");
                }

            }
            catch { }
            
        }
        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            DialogResult resultDeleteFood = MessageBox.Show("Bạn thật sự muốn xóa Món này","Thông Báo",MessageBoxButtons.YesNo,MessageBoxIcon.Question);

            if (resultDeleteFood == DialogResult.Yes)
            {
                try
                {
                    int idfood = Convert.ToInt32(txbFoodID.Text);
                    string FoodName = txbFoodName.Text.ToString();
                    int checkFoodInBillInfo = (int)DataProvider.Instance.ExecuteScalar("SELECT COUNT(*) FROM dbo.BillInfo WHERE idFood =" + idfood);
                    if (checkFoodInBillInfo > 0)//nếu có thong tin Food trong BillInfo thi xoa BillInfo trước xong xoa Food
                    {
                        DialogResult result = MessageBox.Show(String.Format("Nếu bạn xóa món ''{0}'' thì sẻ xóa luôn tất cả các Bill liên quan tới Món ''{0}''", FoodName), "Thông Báo", MessageBoxButtons.OKCancel);
                        if (result == DialogResult.OK)
                        {
                            //MessageBox.Show("xoa billinfo");
                            BillInfoDAO.Instance.DeleteBillInfoByFoodID(idfood);
                        }
                        else
                            return;

                    }
                    if (FoodDAO.Instance.DeleteFood(idfood))
                    {
                        MessageBox.Show(String.Format("Đã xoa món ''{0}'' thành công", FoodName));
                        LoadListFood();
                        //goi event
                        if (deleteFood != null)
                        {
                            deleteFood(this, new EventArgs());
                        }
                    }
                    else
                    {
                        MessageBox.Show(String.Format("Xoa món ''{0}'' Thất Bại", FoodName));
                    }

                }
                catch { }

            }
            else
                return;
            
        }

        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add { insertFood += value; }
            remove { insertFood -= value; }
        }

        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }
        }

        private event EventHandler updateFood;
        public event EventHandler UpdateFood
        {
            add { updateFood += value; }
            remove { updateFood -= value; }
        }
        private void btnShowCategory_Click(object sender, EventArgs e)
        {
            LoadListCategory();
        }
        private void btnEditCategory_Click(object sender, EventArgs e)
        {
            if(CategoryList.Count>0)
            {
                string name = txbCategoryName.Text;
                int id = Convert.ToInt32(txbCategoryID.Text);
                if (CategoryDAO.Instance.UpdateCategory(id, name))
                {

                    MessageBox.Show("Cập nhật Danh mục Thành Công");
                    LoadListCategory();
                }
                else
                {
                    MessageBox.Show("Cập nhật Category thất bại");
                }
            }
            else
                MessageBox.Show("Không có dữ liệu");
        }
        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            string name = txbCategoryName.Text;
            if(name == "")
            {
                MessageBox.Show("Tên danh mục không được để trống","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            //int id = Convert.ToInt32(txbCategoryID.Text);
            int CheckCategoryName = (int)DataProvider.Instance.ExecuteScalar("SELECT COUNT(*) FROM dbo.FoodCategory WHERE name =N'"+name+"'");
            if(CheckCategoryName >0)
            {
                MessageBox.Show(String.Format("Danh mục ''{0}'' đã có vui lòng điền tên danh mục khác",name));
            }
            else 
            {
                if(CategoryDAO.Instance.InsertCategory(name))
                {
                    MessageBox.Show("Đã thêm danh mục thành công");
                    LoadListCategory();
                }
            }
        }
        private void btnTableShow_Click(object sender, EventArgs e)
        {
            LoadListTable();
        }
        List<Food> SearchFoodByName(string name)
        {
            List<Food> listFood = FoodDAO.Instance.SearchFoodByName(name);

            return listFood;
        }

        void LoadSearchFoodByCategory(int idCategory)
        {
            foodList.DataSource = FoodDAO.Instance.GetFoodByCategoryID(idCategory);
        }
        #endregion

        private void btnTableAdd_Click(object sender, EventArgs e)
        {
            int id;
            string name = txbTableName.Text;
            string status = cbTableStatus.SelectedItem.ToString();
            if (txbTableID.Text != "")
            { 
                 id = Convert.ToInt32(txbTableID.Text);
                //kiểm trả tên bàn trùng lập hay không
                Table checkTableName = TableDAO.Instance.GetTableByID(id);
                if (name == checkTableName.Name)
                {
                    MessageBox.Show(string.Format("Tên bàn thêm mới ''{0}'' trung với tên bàn củ ''{1}''", name, checkTableName.Name));
                    return;
                }
            }

            //Thêm bàn mới
            if (TableDAO.Instance.InsertTable(name, status))
            {
                MessageBox.Show(string.Format("Thêm bàn ''{0}'' thành công", name), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadListTable();
            }
            else
                MessageBox.Show(string.Format("Không thể Thêm bàn ''{0}''", name), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);



        }

        private void btnTableEdit_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbTableID.Text);
            string name = txbTableName.Text;
            string status = cbTableStatus.SelectedItem.ToString();

            //kiểm trả tên bàn trùng lập hay không
            Table checkTableName = TableDAO.Instance.GetTableByID(id);
            if (name == checkTableName.Name)
            {
                //MessageBox.Show(string.Format("Tên bàn thêm mới ''{0}'' trung với tên bàn củ ''{1}''", name, checkTableName.Name));
                return;
            }
            else
            {
                if (TableDAO.Instance.UpdateTable(id, name, status))
                {
                    MessageBox.Show(string.Format("Sữa Tên Bàn ''{0}'' thành công", name), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadListTable();
                }
                else
                    MessageBox.Show(string.Format("Không thể sữa đổi tên bàn ''{0}''", name), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

        }

        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            //kiem tra co food nào thuoc category ko nếu có phải chuyển qua category hoặc xóa hết tất cả food đó mới xoa được
            //kiêm tra food có tồn tại trong category không
            if(CategoryList.Count>0)
            {
                int id = Convert.ToInt32(txbCategoryID.Text);
                string query = string.Format("SELECT COUNT(*) FROM dbo.Food WHERE idCategory ={0}", id);
                int checkFood = (int)DataProvider.Instance.ExecuteScalar(query);
                if (checkFood > 0)
                {
                    MessageBox.Show(string.Format("Có tồn tại Món  trong danh muc ''{0}'' bằng cách kìm kiếm theo danh mục và xóa hay chuyển sang danh muc khác mới xóa được", txbCategoryName.Text), "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tcAdmin.SelectedTab = tbFood;
                    //chkCategory.Checked = true;
                    LoadSearchFoodByCategory(id);

                }
                else
                {
                    if (CategoryDAO.Instance.DeleteCategory(id))
                    {
                        MessageBox.Show(string.Format("Đã xóa Danh Mục ''{0}'' Thành Công", txbCategoryName.Text), "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadListCategory();
                    }
                }
            }
            else
                MessageBox.Show("Không có dữ liệu", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void SearchFood_Click(object sender, EventArgs e)
        {
           
            
            float price = (float)nmFoodPrice.Value;
            string name = txbFoodName.Text;
            int search =0;
            if (chkNameFood.Checked)
                search += 1;
            if (chkCategory.Checked)
                search += 3;
            if (chkFoodPriceLess.Checked)
                search += 5;
            if (chkFoodPriceBiger.Checked)
                search += 7;
            try
            {
                int idCategory = (cbFoodCategory.SelectedItem as Category).ID;
                switch (search)
                {
                    case 1://tim theo name gan đúng
                        {
                            foodList.DataSource = SearchFoodByName(name);
                            break;
                        }
                    case 3: //tim theo danh mục
                        {
                            foodList.DataSource = FoodDAO.Instance.GetFoodByCategoryID(idCategory);
                            break;
                        }
                    case 5://giá nhỏ hơn
                        {

                            foodList.DataSource = FoodDAO.Instance.GetFoodPriceLess(price);
                            break;

                        }
                    case 7://giá lớn hơn
                        {

                            foodList.DataSource = FoodDAO.Instance.GetFoodPriceBiger(price);
                            break;
                        }
                    case 4:
                        {
                            foodList.DataSource = FoodDAO.Instance.GetFoodByNameAndCategoryID(name, idCategory);
                            break;
                        }
                }
            }
            catch { }


        }
    }
}
