using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace kursovoy
{
    public partial class Admin : Form
    {
        public Admin()
        {
            InitializeComponent();
        }

        private void Admin_Load(object sender, EventArgs e)
        {
            label2.Text = Authorization.User2.FIO;
        }

        private void product_Click(object sender, EventArgs e)
        {
            Products pr = new Products();
            pr.Show();
            this.Close();
        }

        private void users_Click(object sender, EventArgs e)
        {
            Users us = new Users();
            us.Show();
            this.Close();
        }

        private void employee_Click(object sender, EventArgs e)
        {
            Staffs st = new Staffs();
            st.Show();
            this.Close();
        }

        private void books_Click(object sender, EventArgs e)
        {
            ReferenceBooks rb = new ReferenceBooks();
            rb.Show();
            this.Hide();
        }

        private void back_Click(object sender, EventArgs e)
        {
            this.Close();
            Authorization authorization = new Authorization();
            authorization.Show();
        }
    }
}
