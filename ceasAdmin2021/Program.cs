using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ceasAdmin2021
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            frmPrincipal frmPrincipal = new frmPrincipal();
            //frmPrincipal.Show();
            

            frmLogin frmLogin = new frmLogin();
            frmLogin.Show();



            Application.Run(frmPrincipal);
        }
    }
}

