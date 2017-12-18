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
    public partial class ChatForm : Form
    {
        uint buddyUid;
        string buddyUsername = "";

        public ChatForm(uint buddyUid)
        {
            this.buddyUid = buddyUid;
            InitializeComponent();
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            buddyUsername = Program.usernameMap.ContainsKey(buddyUid)
                ? Program.usernameMap[buddyUid]
                : "UID=" + buddyUid.ToString();
            Text = string.Format("与 {0} 的聊天", buddyUsername);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                Program.session.SendMessage(buddyUid, txtSend.Text);
            }).Start();
        }
    }
}
