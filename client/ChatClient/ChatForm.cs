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
        uint buddy_uid;

        public ChatForm(uint buddy_uid)
        {
            this.buddy_uid = buddy_uid;
            InitializeComponent();
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            Text = string.Format("与 {0} 的聊天",
                Program.usernameMap.ContainsKey(buddy_uid)
                ? Program.usernameMap[buddy_uid]
                : "UID=" + buddy_uid.ToString());
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                Program.session.SendMessage(buddy_uid, txtSend.Text);
            }).Start();
        }
    }
}
