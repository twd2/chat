using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class ChatForm : Form
    {
        public const double INPUTING_INTERVAL = 1.0;

        uint buddyUid;
        string buddyUsername = "";
        bool isInputing = false;
        DateTime lastReceiveInputing = DateTime.Now;
        DateTime lastSendInputing =
            DateTime.Now.Subtract(TimeSpan.FromSeconds(INPUTING_INTERVAL));

        GraphicsForm canvasForm = null;

        public ChatForm(uint buddyUid)
        {
            this.buddyUid = buddyUid;
            InitializeComponent();
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            UpdateUsername();
            timer1.Interval = (int)(INPUTING_INTERVAL * 1000);
            timer1.Enabled = true;
        }

        public void UpdateUsername()
        {
            buddyUsername = Program.usernameMap.ContainsKey(buddyUid)
                ? Program.usernameMap[buddyUid]
                : "UID=" + buddyUid.ToString();
            UpdateTitle();
        }

        public void UpdateTitle()
        {
            Text = string.Format("与 {0} 的聊天{1}", buddyUsername, isInputing ? " - 正在输入..." : "");
            if (canvasForm != null)
            {
                canvasForm.Text = string.Format("与 {0} 的共享画板", buddyUsername);
            }
        }

        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (canvasForm != null && !canvasForm.IsDisposed)
            {
                canvasForm.Close();
                canvasForm = null;
            }
            ChatForm _;
            Program.chatFormMap.TryRemove(buddyUid, out _);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (txtSend.Text == "")
            {
                return;
            }
            string text = txtSend.Text;
            txtSend.Text = "";
            new Thread(() =>
            {
                Program.session.SendMessage(buddyUid, text);
            }).Start();
            string msg = string.Format("{0} ({1}) {2}\n{3}\n",
                Program.session.Username, Program.session.Uid, DateTime.Now.ToString(),
                text);
            ShowAndLog(msg);
            txtSend.Focus();
        }

        public void OnMessage(Message m)
        {
            switch (m.Type)
            {
                case Message.Types.Type.Message:
                {
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    DateTime dt = epoch.Add(TimeSpan.FromTicks((long)m.Timestamp * TimeSpan.TicksPerSecond)).ToLocalTime();
                    string msg = string.Format("{0} ({1}) {2}\n{3}\n",
                    buddyUsername, buddyUid, dt.ToString(),
                    m.Msg);
                    ShowAndLog(msg);
                    break;
                }
                case Message.Types.Type.File:
                {
                    OnFileMessage(m);
                    break;
                }
                case Message.Types.Type.Inputing:
                {
                    isInputing = true;
                    lastReceiveInputing = DateTime.Now;
                    UpdateTitle();
                    break;
                }
                case Message.Types.Type.Graphics:
                {
                    EnsureCanvas();
                    canvasForm.OnMessage(m);
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        private void OnFileMessage(Message m)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime dt = epoch.Add(TimeSpan.FromTicks((long)m.Timestamp * TimeSpan.TicksPerSecond)).ToLocalTime();
            string msg = string.Format("{0} ({1}) {2}\n文件：{3}\n",
               buddyUsername, buddyUid, dt.ToString(),
               m.Msg);
            ShowAndLog(msg);
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "所有文件 (*.*)|*.*";
                sfd.Title = "收到新文件 " + m.Msg;
                sfd.FileName = m.Msg;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, m.Data.ToByteArray());
                }
            }
        }

        private void ShowAndLog(string msg)
        {
            txtSession.AppendText(msg.Replace("\n", "\r\n") + "\r\n");
            Program.WriteLog(buddyUid, msg);
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "所有文件 (*.*)|*.*";
                ofd.Title = "发送文件";
                ofd.Multiselect = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var filename in ofd.FileNames)
                    {
                        FileInfo fi = new FileInfo(filename);
                        string msg = string.Format("{0} ({1}) {2}\n文件：{3}\n",
                            Program.session.Username, Program.session.Uid, DateTime.Now.ToString(),
                            fi.Name);
                        ShowAndLog(msg);
                    }

                    new Thread(() =>
                    {
                        string[] filenames = ofd.FileNames;
                        foreach (var filename in filenames)
                        {
                            Program.session.SendFile(buddyUid, filename);
                        }
                    }).Start();
                }
            }
        }

        private void txtSend_TextChanged(object sender, EventArgs e)
        {
            if (DateTime.Now.Subtract(lastSendInputing).TotalSeconds > INPUTING_INTERVAL * 0.95)
            {
                new Thread(() =>
                {
                    Program.session.SendInputing(buddyUid);
                }).Start();
                lastSendInputing = DateTime.Now;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.Subtract(lastReceiveInputing).TotalSeconds > INPUTING_INTERVAL)
            {
                isInputing = false;
                UpdateTitle();
            }
        }

        private void btnCanvas_Click(object sender, EventArgs e)
        {
            EnsureCanvas();
        }

        private void EnsureCanvas()
        {
            if (canvasForm == null || canvasForm.IsDisposed)
            {
                canvasForm = new GraphicsForm(buddyUid);
                UpdateTitle();
            }
            canvasForm.Show();
        }
    }
}
