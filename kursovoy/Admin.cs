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

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            Authorization authorization = new Authorization();
            authorization.Show();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Products pr = new Products();
            pr.Show();
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Users us = new Users();
            us.Show();
            this.Close();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Staffs st = new Staffs();
            st.Show();
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            import i = new import();
            i.Show();
            this.Close();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            ReferenceBooks rb = new ReferenceBooks();
            rb.Show();
            this.Hide();
        }

        private void Admin_Load(object sender, EventArgs e)
        {
            label2.Text = Authorization.User2.FIO;
        }
    }
}
