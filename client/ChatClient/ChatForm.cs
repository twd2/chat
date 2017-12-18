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

        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ChatForm _;
            Program.chatFormMap.TryRemove(buddyUid, out _);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                Program.session.SendMessage(buddyUid, txtSend.Text);
            }).Start();
            string msg = string.Format("{0} ({1}) {2}\n{3}\n",
                Program.session.Username, Program.session.Uid, DateTime.Now.ToString(),
                txtSend.Text);
            ShowAndLog(msg);
        }

        public void OnMessage(Message m)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime dt = epoch.Add(TimeSpan.FromTicks((long)m.Timestamp * TimeSpan.TicksPerSecond)).ToLocalTime();
            string msg = string.Format("{0} ({1}) {2}\n{3}\n",
                buddyUsername, buddyUid, dt.ToString(),
                m.Msg);
            ShowAndLog(msg);
        }

        private void ShowAndLog(string msg)
        {
            txtSession.AppendText(msg.Replace("\n", "\r\n") + "\r\n");
            Program.WriteLog(buddyUid, msg);
        }
    }
}
