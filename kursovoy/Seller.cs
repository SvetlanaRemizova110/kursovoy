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
    public partial class Seller : Form
    {
        public Seller()
        {
            InitializeComponent();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Authorization ad = new Authorization();
            ad.Show();
            this.Hide();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Products pr = new Products();
            pr.Show();
            this.Hide();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Orders pr = new Orders();
            pr.Show();
            this.Hide();
        }
    }
}
