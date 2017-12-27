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
    public partial class GraphicsForm : Form
    {
        uint buddyUid;
        bool isDown = false;
        Point downPoint = Point.Empty;
        Graphics g = null;
        Bitmap img = null;

        public GraphicsForm(uint buddyUid)
        {
            this.buddyUid = buddyUid;
            InitializeComponent();
        }

        private void GraphicsForm_Load(object sender, EventArgs e)
        {
            img = new Bitmap(1920, 1200); // TODO
            g = Graphics.FromImage(img);
            g.Clear(Color.White);
            canvas.Image = img;
        }

        public void OnMessage(Message m)
        {
            try
            {
                string[] args = m.Msg.Split(',');
                int x0 = int.Parse(args[0]),
                    y0 = int.Parse(args[1]),
                    x1 = int.Parse(args[2]),
                    y1 = int.Parse(args[3]);
                g.DrawLine(Pens.Black, x0, y0, x1, y1);
                UpdateImage();
            }
            catch (Exception)
            {
                
            }
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            isDown = true;
            downPoint = e.Location;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDown)
            {
                return;
            }
            g.DrawLine(Pens.Black, downPoint, e.Location);
            string packet = downPoint.X.ToString() + "," + downPoint.Y.ToString() + "," +
                            e.Location.X.ToString() + "," + e.Location.Y.ToString();
            new Thread(() =>
            {
                Program.session.SendGraphics(buddyUid, packet);
            }).Start();
            downPoint = e.Location;
            UpdateImage();
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            isDown = false;
        }

        private void UpdateImage()
        {
            canvas.Refresh();
        }
    }
}
