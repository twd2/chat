using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnListUser_Click(object sender, EventArgs e)
        {
            new ListUserForm().Show();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.loginForm.Close();
            Program.exitEvent.Set();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
