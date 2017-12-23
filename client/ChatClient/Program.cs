using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ChatClient
{
    static class Program
    {
        public static LoginForm loginForm = null;
        public static MainForm mainForm = null;
        public static Session session = new Session();
        public static ManualResetEvent exitEvent = new ManualResetEvent(true);
        public static ConcurrentDictionary<uint, string> usernameMap =
            new ConcurrentDictionary<uint, string>();
        public static ConcurrentDictionary<uint, ChatForm> chatFormMap =
            new ConcurrentDictionary<uint, ChatForm>();
        private static object logLock = new object();

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            loginForm = new LoginForm();
            Application.Run(loginForm);
            exitEvent.WaitOne();
            Environment.Exit(0);
        }

        public static void WriteLog(uint buddyUid, string m)
        {
            lock (logLock)
            {
                if (!Directory.Exists("log"))
                {
                    Directory.CreateDirectory("log");
                }
                using (var sw = new StreamWriter(Path.Combine("log", buddyUid.ToString() + ".txt"),
                                                 true, Encoding.UTF8))
                {
                    sw.WriteLine(m);
                }
            }
        }
    }
}
