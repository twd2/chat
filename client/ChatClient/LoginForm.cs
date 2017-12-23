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
                            Program.mainForm = new MainForm();
                            Program.mainForm.Show();
                            Program.session.SetReady();
                            Program.session.onClosed -= OnSessionClosed;
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
            new RegisterForm().ShowDialog();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            Enabled = false;
            new Thread(() =>
            {
                try
                {
                    Program.session.onClosed += OnSessionClosed;
                    Program.session.Connect(Properties.Settings.Default.Server,
                                            Properties.Settings.Default.Port);
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
