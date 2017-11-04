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
            Packet.LoginRequest loginReq = new Packet.LoginRequest();
            loginReq.username = new string('g', 1024);
            loginReq.password = "twd3";
            Debug.Print(loginReq.h.size.ToString());
            Debug.Print(loginReq.h.type.ToString());
            Packet.HostToNetwork(loginReq);
            Debug.Print(loginReq.h.size.ToString());
            Debug.Print(loginReq.h.type.ToString());

            byte[] bytes = Util.StructureToByteArray(loginReq);
            Packet.Hello loginReq2 = 
                Util.ByteArrayToStructure<Packet.Hello>(bytes);
            Packet.NetworkToHost(loginReq2);
            // MessageBox.Show(Marshal.SizeOf(typeof(Packet.Hello)).ToString());

            TcpClient client = new TcpClient();
            client.Connect("192.168.1.105", 1025);
            NetworkStream stream = client.GetStream();
            Packet.Hello obj = (Packet.Hello)Packet.Read(stream);
            Packet.NetworkToHost(obj);
            MessageBox.Show(obj.message);
        }
    }
}
