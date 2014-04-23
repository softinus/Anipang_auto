using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AniPang
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmAnipang frm1 = new frmAnipang();
            frm1.Show();
            //this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmAllGame frm2 = new frmAllGame();
            frm2.Show();
            //this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmKutar1 frm3 = new frmKutar1();
            frm3.Show();
            //this.Hide();
        }
    }
}
