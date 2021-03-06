﻿using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatClient
{
    class Session
    {
        uint uid = 0;
        string username = "";
        bool connected = false;
        TcpClient tcpClient = null;
        NetworkStream rawStream = null;
        SslStream stream = null;
        Thread thread = null;

        ManualResetEvent loginEvent = new ManualResetEvent(false);
        LoginResponse lastLoginResponse = null;

        ManualResetEvent registerEvent = new ManualResetEvent(false);
        RegisterResponse lastRegisterResponse = null;

        ManualResetEvent readyEvent = new ManualResetEvent(false);

        public delegate void OnListUserResponse(ListUserResponse r);
        public OnListUserResponse onListUserResponse = null;

        public delegate void OnListBuddyResponse(ListBuddyResponse r);
        public OnListBuddyResponse onListBuddyResponse = null;

        public delegate void OnMessage(Message m);
        public OnMessage onMessage = null;

        Reset lastReset = null;
        public delegate void OnClosed(Reset r);
        public OnClosed onClosed = null;

        public uint Uid
        {
            get
            {
                return uid;
            }
        }

        public string Username
        {
            get
            {
                return username;
            }
        }

        public Session()
        {

        }

        public void Connect(string server, int port)
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(server, port);
            rawStream = tcpClient.GetStream();
            stream = new SslStream(rawStream, false, validateRemoteCert, selectLocalCert,
                                   EncryptionPolicy.RequireEncryption);
            stream.AuthenticateAsClient(server, new X509CertificateCollection(), 
                                        SslProtocols.Tls12, false);
            Debug.Print("Using cipher " + stream.CipherAlgorithm.ToString());
            connected = true;
            thread = new Thread(Handle);
            thread.Start();
        }

        private bool validateRemoteCert(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (certificate.GetType() == typeof(X509Certificate2))
            {
                X509Certificate2 cert = (X509Certificate2)certificate;
                Debug.Print("Remote Cert: " + cert.Thumbprint.Replace(" ", "").ToUpper());
                Debug.Print("Trusted Cert: " + Properties.Settings.Default.Fingerprint.Replace(" ", "").ToUpper());
                return cert.Thumbprint.Replace(" ", "").ToUpper() ==
                    Properties.Settings.Default.Fingerprint.Replace(" ", "").ToUpper();
            }
            return false;
        }

        private X509Certificate selectLocalCert(
            object sender,
            string targetHost,
            X509CertificateCollection localCertificates,
            X509Certificate remoteCertificate,
            string[] acceptableIssuers)
        {
            return null;
        }

        private void Handle()
        {
            while (connected)
            {
                byte type;
                byte[] buffer = Packet.Read(stream, out type);

                if (buffer == null)
                {
                    Debug.Print("broken");
                    break;
                }

                Debug.Print("Type: {0}", type);

                lastReset = null;

                // dispatch
                switch (type)
                {
                    case Packet.PACKET_LOGIN:
                    {
                        LoginResponse r = LoginResponse.Parser.ParseFrom(buffer);
                        HandleLogin(r);
                        break;
                    }
                    case Packet.PACKET_REGISTER:
                    {
                        RegisterResponse r = RegisterResponse.Parser.ParseFrom(buffer);
                        HandleRegister(r);
                        break;
                    }
                    case Packet.PACKET_LIST_USER:
                    {
                        ListUserResponse r = ListUserResponse.Parser.ParseFrom(buffer);
                        HandleListUser(r);
                        break;
                    }
                    case Packet.PACKET_LIST_BUDDY:
                    {
                        ListBuddyResponse r = ListBuddyResponse.Parser.ParseFrom(buffer);
                        HandleListBuddy(r);
                        break;
                    }
                    case Packet.PACKET_ADD_BUDDY:
                    {
                        AddBuddyResponse r = AddBuddyResponse.Parser.ParseFrom(buffer);
                        HandleAddBuddy(r);
                        break;
                    }
                    case Packet.PACKET_REMOVE_BUDDY:
                    {
                        RemoveBuddyResponse r = RemoveBuddyResponse.Parser.ParseFrom(buffer);
                        HandleRemoveBuddy(r);
                        break;
                    }
                    case Packet.PACKET_MESSAGE:
                    {
                        Message r = Message.Parser.ParseFrom(buffer);
                        HandleMessage(r);
                        break;
                    }
                    case Packet.PACKET_RESET:
                    {
                        Reset r = Reset.Parser.ParseFrom(buffer);
                        HandleReset(r);
                        break;
                    }
                    case Packet.PACKET_RAW:
                    {
                        Debug.Print(Encoding.UTF8.GetString(buffer));
                        break;
                    }
                    default:
                    {
                        LoginResponse r = LoginResponse.Parser.ParseFrom(buffer);
                        break;
                    }
                }
            }
            Close();
        }

        private void HandleLogin(LoginResponse r)
        {
            Debug.Print("handling login");
            if (r.Code == LoginResponse.Types.Code.Success)
            {
                uid = r.Uid;
            }
            else
            {
                username = "";
            }
            lastLoginResponse = r;
            loginEvent.Set();
        }

        private void HandleRegister(RegisterResponse r)
        {
            Debug.Print("handling register");
            lastRegisterResponse = r;
            registerEvent.Set();
        }

        private void HandleListUser(ListUserResponse r)
        {
            Debug.Print("handling list user");
            readyEvent.WaitOne();
            if (onListUserResponse != null)
            {
                onListUserResponse(r);
            }
        }

        private void HandleListBuddy(ListBuddyResponse r)
        {
            Debug.Print("handling list buddy");
            readyEvent.WaitOne();
            if (onListBuddyResponse != null)
            {
                onListBuddyResponse(r);
            }
        }

        private void HandleAddBuddy(AddBuddyResponse r)
        {
            Debug.Print("handling add buddy");
        }

        private void HandleRemoveBuddy(RemoveBuddyResponse r)
        {
            Debug.Print("handling remove buddy");
        }

        private void HandleMessage(Message r)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime dt = epoch.Add(TimeSpan.FromTicks((long)r.Timestamp * TimeSpan.TicksPerSecond)).ToLocalTime();
            Debug.Print(string.Format("message from {0} @ {1}: {2}", r.Uid, dt.ToString(), r.Msg));
            readyEvent.WaitOne();
            if (onMessage != null)
            {
                onMessage(r);
            }
        }

        private void HandleReset(Reset r)
        {
            Debug.Print("handling reset");
            lastReset = r;
        }

        public LoginResponse.Types.Code Login(string username, string password)
        {
            loginEvent.Reset();
            LoginRequest q = new LoginRequest();
            q.Username = username;
            q.Password = password;
            lock (this)
            {
                Packet.Write(stream, q);
            }
            loginEvent.WaitOne();
            this.username = username;
            return lastLoginResponse.Code;
        }

        public RegisterResponse Register(string username, string password)
        {
            registerEvent.Reset();
            RegisterRequest q = new RegisterRequest();
            q.Username = username;
            q.Password = password;
            lock (this)
            {
                Packet.Write(stream, q);
            }
            registerEvent.WaitOne();
            return lastRegisterResponse;
        }

        public void ListUser()
        {
            ListUserRequest q = new ListUserRequest();
            lock (this)
            {
                Packet.Write(stream, q);
            }
        }

        public void ListBuddy()
        {
            ListBuddyRequest q = new ListBuddyRequest();
            lock (this)
            {
                Packet.Write(stream, q);
            }
        }

        public void AddBuddy(uint uid)
        {
            AddBuddyRequest q = new AddBuddyRequest();
            q.Uid = uid;
            lock (this)
            {
                Packet.Write(stream, q);
            }
        }

        public void SendMessage(uint destUid, Message.Types.Type type, string msg)
        {
            Message q = new Message();
            q.Uid = destUid;
            q.Type = type;
            q.Msg = msg;
            q.Data = ByteString.Empty;
            lock (this)
            {
                Packet.Write(stream, q);
            }
        }

        public void SendMessage(uint destUid, string msg)
        {
            SendMessage(destUid, Message.Types.Type.Message, msg);
        }

        public void SendInputing(uint destUid)
        {
            SendMessage(destUid, Message.Types.Type.Inputing, "");
        }

        public void SendGraphics(uint destUid, string msg)
        {
            SendMessage(destUid, Message.Types.Type.Graphics, msg);
        }

        public void SendFile(uint destUid, string filename)
        {
            FileInfo fi = new FileInfo(filename);
            Message q = new Message();
            q.Uid = destUid;
            q.Type = Message.Types.Type.File;
            q.Msg = fi.Name;
            using (FileStream fs = fi.OpenRead())
            {
                q.Data = ByteString.FromStream(fs);
            }
            // TODO: fragmentation
            lock (this)
            {
                Packet.Write(stream, q);
            }
        }

        public void SetReady()
        {
            readyEvent.Set();
        }

        public void Close()
        {
            if (!connected || tcpClient == null)
            {
                return;
            }
            connected = false;
            if (stream != null)
            {
                stream.Close();
                stream.Dispose();
                stream = null;
            }
            if (rawStream != null)
            {
                rawStream.Close();
                rawStream.Dispose();
                rawStream = null;
            }
            tcpClient.Close();
            tcpClient = null;
            thread = null;
            if (onClosed != null)
            {
                onClosed(lastReset);
            }
        }
    }
}
