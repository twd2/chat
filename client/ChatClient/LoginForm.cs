using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Enabled = false;
            new Thread(() =>
            {
                LoginResponse.Types.Code code = 
                    Program.session.Login(txtUsername.Text, txtPassword.Text);
                Invoke(new Action(() =>
                {
                    switch (code)
                    {
                        case LoginResponse.Types.Code.Success:
                        {
                            Program.exitEvent.Reset();
                            new MainForm().Show();
                            Hide();
                            break;
                        }
                        case LoginResponse.Types.Code.UserNotFound:
                        {
                            MessageBox.Show("用户未找到。");
                            Enabled = true;
                            break;
                        }
                        case LoginResponse.Types.Code.PasswordError:
                        {
                            MessageBox.Show("密码错误。");
                            Enabled = true;
                            break;
                        }
                        default:
                        {
                            break;
                        }
                    }
                }));
            }).Start();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            new RegisterForm().Show();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            Enabled = false;
            new Thread(() =>
            {
                try
                {
                    // TODO: configurable
                    Program.session.Connect("192.168.1.105", 1025);
                    Invoke(new Action(() =>
                    {
                        Enabled = true;
                    }));
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        MessageBox.Show(ex.Message);
                        Environment.Exit(1);
                    }));
                }
            }).Start();
        }
    }
}
