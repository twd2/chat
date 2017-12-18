using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        public class UserWrapper
        {
            public ListBuddyResponse.Types.User user;

            public UserWrapper(ListBuddyResponse.Types.User user)
            {
                this.user = user;
            }

            public override string ToString()
            {
                return string.Format("{0} ({1})", user.Username, user.Online ? "在线" : "离线");
            }
        }

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
            Program.session.onMessage -= OnMessage;
            Program.session.onListBuddyResponse -= OnListBuddyResponse;
            Program.session.onClosed -= OnSessionClosed;
            Program.loginForm.Close();
            Program.exitEvent.Set();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Program.session.onClosed += OnSessionClosed;
            Program.session.onListBuddyResponse += OnListBuddyResponse;
            Program.session.onMessage += OnMessage;
            labHello.Text = Program.session.Username + "，您好！";
            btnRefresh_Click(null, null);
        }

        private void OnListBuddyResponse(ListBuddyResponse r)
        {
            Invoke(new Action(() =>
            {
                lstBuddies.Items.Clear();
                foreach (ListBuddyResponse.Types.User u in r.Users)
                {
                    Program.usernameMap[u.Uid] = u.Username;
                    lstBuddies.Items.Add(new UserWrapper(u));
                    Debug.Print(u.Username);
                }
            }));
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                Program.session.ListBuddy();
            }).Start();
        }

        private void lstBuddies_DoubleClick(object sender, EventArgs e)
        {
            if (lstBuddies.SelectedIndex == -1)
            {
                MessageBox.Show("您没有选择任何用户。");
                return;
            }
            UserWrapper uw = (UserWrapper)lstBuddies.Items[lstBuddies.SelectedIndex];
            if (!Program.chatFormMap.ContainsKey(uw.user.Uid))
            {
                Program.chatFormMap[uw.user.Uid] = new ChatForm(uw.user.Uid);
            }
            Program.chatFormMap[uw.user.Uid].Show();
            Program.chatFormMap[uw.user.Uid].Focus();
        }

        public void OnMessage(Message m)
        {
            new Thread(() =>
            {
                Invoke(new Action(() =>
                {
                    if (!Program.chatFormMap.ContainsKey(m.Uid))
                    {
                        Program.chatFormMap[m.Uid] = new ChatForm(m.Uid);
                        Program.chatFormMap[m.Uid].Show();
                    }
                    Program.chatFormMap[m.Uid].OnMessage(m);
                }));
            }).Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            btnRefresh_Click(null, null);
        }
    }
}
