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
            Program.session.onClosed -= OnSessionClosed;
            Program.loginForm.Close();
            Program.exitEvent.Set();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Program.session.onClosed += OnSessionClosed;
        }

        private void OnSessionClosed(Reset r)
        {
            Invoke(new Action(() =>
            {
                if (r != null)
                {
                    switch (r.Code)
                    {
                        case Reset.Types.Code.ProtocolMismatch:
                        {
                            MessageBox.Show("协议不匹配。");
                            break;
                        }
                        case Reset.Types.Code.Kicked:
                        {
                            MessageBox.Show("您已被踢下线。");
                            break;
                        }
                        default:
                        {
                            MessageBox.Show("出现了未知错误。");
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("服务器连接已断开。");
                }
                Environment.Exit(1);
            }));
        }
    }
}
