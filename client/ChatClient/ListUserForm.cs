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
    public partial class ListUserForm : Form
    {
        public class UserWrapper
        {
            public ListUserResponse.Types.User user;

            public UserWrapper(ListUserResponse.Types.User user)
            {
                this.user = user;
            }

            public override string ToString()
            {
                return string.Format("{0} ({1})", user.Username, user.Online ? "在线" : "离线");
            }
        }

        public ListUserForm()
        {
            InitializeComponent();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                Program.session.ListUser();
            }).Start();
        }

        private void ListUserForm_Load(object sender, EventArgs e)
        {
            Program.session.onListUserResponse += OnListUserResponse;
            btnRefresh_Click(null, null);
        }

        private void ListUserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.session.onListUserResponse -= OnListUserResponse;
        }

        private void OnListUserResponse(ListUserResponse r)
        {
            Invoke(new Action(() =>
            {
                lstUsers.Items.Clear();
                foreach (ListUserResponse.Types.User u in r.Users)
                {
                    Program.usernameMap[u.Uid] = u.Username;
                    lstUsers.Items.Add(new UserWrapper(u));
                    Debug.Print(u.Username);
                }
            }));
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (lstUsers.SelectedIndex == -1)
            {
                MessageBox.Show("您没有选择任何用户。");
                return;
            }
            UserWrapper uw = (UserWrapper)lstUsers.Items[lstUsers.SelectedIndex];
            new Thread(() =>
            {
                Program.session.AddBuddy(uw.user.Uid);
            }).Start();
        }
    }
}
