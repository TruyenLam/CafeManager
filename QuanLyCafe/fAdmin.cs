using QuanLyCafe.DAO;
using QuanLyCafe.DTO;
using System;
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
        private void txbTableID_TextChanged(object sender, EventArgs e)
        {
           
        }
        private void btnAddFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int id = Convert.ToInt32(txbFoodID.Text);
            int idCategory = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;
            //kiểm tra food đã có hay chưa
            Food lisfood = FoodDAO.Instance.GetFoodByID(id);
            if (name == lisfood.Name.ToString())
            {
                MessageBox.Show("Tên món đã có vui long đặt tên khác");
            }//ket thuc kiểm tra nếu đã có kết thuc
            else
            {
                if (FoodDAO.Instance.InsertFood(name, idCategory, price))
                {
                    MessageBox.Show("Thêm món thành công");
                    LoadListFood();

                    if(insertFood != null)
                    {
                        insertFood(this,new EventArgs());
                    }
                }
                else
                {
                    MessageBox.Show("Thêm món thất bại");
                }
            }
        }

        private void btnEditFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int id = Convert.ToInt32(txbFoodID.Text);
            int idCategory = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;
            //kiểm tra food đã có hay chưa
            Food lisfood = FoodDAO.Instance.GetFoodByID(id);
            if (name == lisfood.Name.ToString())
            {
                MessageBox.Show("Tên món đã có vui long đặt tên khác");
            }//ket thuc kiểm tra nếu đã có kết thuc
            else
            {
                if (FoodDAO.Instance.UpdatFood(id, name, idCategory, price))
                {
                    MessageBox.Show("Cập nhật món thành công");
                    LoadListFood();
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
            
        }
        private void btnDeleteFood_Click(object sender, EventArgs e)
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

            }
            if(FoodDAO.Instance.DeleteFood(idfood))
            {
                MessageBox.Show(String.Format("Đã xoa món ''{0}'' thành công", FoodName));
                LoadListFood();
                //goi event
                if(deleteFood != null)
                {
                    deleteFood(this, new EventArgs());
                }
            }
            else
            {
                MessageBox.Show(String.Format("Xoa món ''{0}'' Thất Bại", FoodName));
            }

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
            string name = txbCategoryName.Text;
            int id = Convert.ToInt32(txbCategoryID.Text);
            if(CategoryDAO.Instance.UpdateCategory(id,name))
            {
                
                MessageBox.Show("Cập nhật Danh mục Thành Công");
                LoadListCategory();
            }
            else
            {
                MessageBox.Show("Cập nhật Category thất bại");
            }
        }
        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            string name = txbCategoryName.Text;
            int id = Convert.ToInt32(txbCategoryID.Text);
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


        #endregion

        
    }
}
