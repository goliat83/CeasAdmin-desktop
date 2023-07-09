using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ceasAdmin2021
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            frmPrincipal frmPrincipal = new frmPrincipal();
            frmPrincipal.Show();
            
            frmPrincipal.StartPosition = FormStartPosition.CenterScreen;
            frmPrincipal.WindowState = FormWindowState.Maximized;
            
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
        }
    }
}
