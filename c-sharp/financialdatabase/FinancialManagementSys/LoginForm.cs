using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1SQL
{
    public partial class LoginForm : Form
    {

        public LoginForm()
        {
            InitializeComponent();
            UIEventHandlers loginUIEvebt = new UIEventHandlers();
            UIEventHandlers.LoadIniSettings(textBoxUser,
                                            textBoxIPAdd,
                                            textBoxPath,
                                            textBoxBackupUtility);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            UIEventHandlers.Login(this, textBoxUser,
                                        textBoxIPAdd,
                                        textBoxPath,
                                        textBoxBackupUtility,
                                        textBoxPassword);

        }

    }


}
