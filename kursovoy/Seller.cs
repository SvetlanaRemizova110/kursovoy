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
            this.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Products pr = new Products();
            pr.Show();
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Orders pr = new Orders();
            pr.Show();
            this.Close();
        }

        private void Seller_Load(object sender, EventArgs e)
        {
            label2.Text = Authorization.User2.FIO;
        }
    }
}
