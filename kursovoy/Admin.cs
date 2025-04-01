using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            Authorization ad = new Authorization();
            ad.Show(); //Открытие формы авторизации
            this.Hide(); //Скрытие главной формы администратора
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Products pr = new Products();
            pr.Show();
            this.Hide();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Users us = new Users();
            us.Show();
            this.Hide();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Staffs st = new Staffs();
            st.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            import i = new import();
            this.Hide();
            i.Show();
        }
    }
}
