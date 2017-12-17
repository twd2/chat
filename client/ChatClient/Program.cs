using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ChatClient
{
    static class Program
    {
        public static Session session = new Session();
        public static ManualResetEvent exitEvent = new ManualResetEvent(true);

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
            exitEvent.WaitOne();
            Debug.Print("here");
            Environment.Exit(0);
        }
    }
}
