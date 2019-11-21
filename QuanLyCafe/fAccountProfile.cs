using QuanLyCafe.DAO;
using QuanLyCafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyCafe
{
    public partial class fAccountProfile : Form
    {
        private Account loginAccount;

        public Account LoginAccount
        {
            get => loginAccount;
            set
            {
                loginAccount = value;
                //changAccount(loginAccount);
            }
        }
        public fAccountProfile(Account acc)
        {
            InitializeComponent();
            loginAccount = acc;
            changAccount(acc);
        }
        void changAccount(Account acc)
        {
            txbDisplayName.Text = LoginAccount.DisplayName;
            txbUserName.Text = LoginAccount.UserName;

        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        void UpdateAccountInfo()
        {
            string UserName = txbUserName.Text;
            string PassWord = txbPassword.Text;
            string DisplayName = txbDisplayName.Text;
            string ReEnterpass = txbReEnterPass.Text;
            string NewPass = txbNewPass.Text;

            if (!NewPass.Equals(ReEnterpass))
            {
                MessageBox.Show("Mật Khẩu nhập lại không đúng với mật khẩu mới!");
            }
            else
            {
                if (AccountDAO.Instance.UpdateAccount(UserName, DisplayName, PassWord, NewPass))
                {
                    MessageBox.Show("Cập nhật tài khoản thành công");
                    if (updateAccount != null)
                        updateAccount(this, new AccountEvent(AccountDAO.Instance.GetAccountByUserName(UserName)));
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập đúng mật Khẩu");
                }
            }
        }

        private event EventHandler<AccountEvent> updateAccount;
        public event EventHandler<AccountEvent> UpdateAccount
        {
            add { updateAccount += value; }
            remove { updateAccount -= value; }
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateAccountInfo();
        }

        public class AccountEvent : EventArgs
        {
            private Account acc;

            public Account Acc
            {
                get { return acc; }
                set { acc = value; }
            }

            public AccountEvent(Account acc)
            {
                this.Acc = acc;
            }
        }
    }
}
