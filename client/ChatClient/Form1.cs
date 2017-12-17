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
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TcpClient client = new TcpClient();
            client.Connect("192.168.1.105", 1025);
            NetworkStream stream = client.GetStream();
            byte type;
            byte[] buffer = Packet.Read(stream, out type);
            MessageBox.Show(Encoding.UTF8.GetString(buffer));
        }
    }
}
