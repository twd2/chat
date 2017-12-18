using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatClient
{
    class Session
    {
        uint uid = 0;
        bool connected = false;
        TcpClient tcpClient = null;
        NetworkStream stream = null;
        Thread thread = null;

        ManualResetEvent loginEvent = new ManualResetEvent(false);
        LoginResponse lastLoginResponse = null;

        ManualResetEvent registerEvent = new ManualResetEvent(false);
        RegisterResponse lastRegisterResponse = null;

        public delegate void OnListUserResponse(ListUserResponse r);
        public OnListUserResponse onListUserResponse = null;

        public delegate void OnListBuddyResponse(ListBuddyResponse r);
        public OnListBuddyResponse onListBuddyResponse = null;

        Reset lastReset = null;
        public delegate void OnClosed(Reset r);
        public OnClosed onClosed = null;

        public Session()
        {

        }

        public void Connect(string server, int port)
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(server, port);
            stream = tcpClient.GetStream();
            connected = true;
            thread = new Thread(Handle);
            thread.Start();
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
            if (onListUserResponse != null)
            {
                onListUserResponse(r);
            }
        }

        private void HandleListBuddy(ListBuddyResponse r)
        {
            Debug.Print("handling list buddy");
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
            Debug.Print("handling message");
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

        public void Close()
        {
            if (!connected || tcpClient == null)
            {
                return;
            }
            connected = false;
            stream.Close();
            stream.Dispose();
            stream = null;
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
