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
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            Enabled = false;
            new Thread(() =>
            {
                RegisterResponse r =
                    Program.session.Register(txtUsername.Text, txtPassword.Text);
                Invoke(new Action(() =>
                {
                    switch (r.Code)
                    {
                        case RegisterResponse.Types.Code.Success:
                        {
                            MessageBox.Show("注册成功，您的用户号为" + r.Uid.ToString() + "。");
                            Close();
                            break;
                        }
                        case RegisterResponse.Types.Code.UserExists:
                        {
                            MessageBox.Show("用户已存在。");
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
    }
}
