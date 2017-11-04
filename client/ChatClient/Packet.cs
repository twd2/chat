﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace ChatClient
{
    class Packet
    {
        private const int PLACEHOLDER_SIZE = 1;

        public const string MAGIC = "chatserv";
        public const ushort TYPE_HELLO = 0;
        public const ushort TYPE_LOGIN_REQ = 1;
        public const ushort TYPE_LOGIN_RES = 2;

        public const ushort LOGIN_SUCCEEDED = 0;
        public const ushort LOGIN_USER_NOT_FOUNT = 1;
        public const ushort LOGIN_BAD_PASSWORD = 2;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class Head
        {
            public ushort size; // whole packet size
            public ushort type;

            public Head()
            {
                size = (ushort)Marshal.SizeOf(typeof(Head));
                type = 0;
            }

            public Head(ushort size, ushort type)
            {
                this.size = size;
                this.type = type;
            }
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class Hello
        {
            [MarshalAs(UnmanagedType.Struct)]
            public Head h;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] magic;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PLACEHOLDER_SIZE)]
            public string message;

            public Hello()
                : this(0)
            {
            }

            public Hello(ushort message_size)
            {
                h = new Head((ushort)(Marshal.SizeOf(typeof(Hello)) - PLACEHOLDER_SIZE + message_size),
                             TYPE_HELLO);
                magic = Encoding.ASCII.GetBytes(MAGIC);
                message = "";
            }
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class LoginRequest
        {
            [MarshalAs(UnmanagedType.Struct)]
            public Head h;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string username;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string password;

            public LoginRequest()
            {
                h = new Head((ushort)Marshal.SizeOf(typeof(LoginRequest)), TYPE_LOGIN_REQ);
                username = "";
                password = "";
            }
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class LoginResponse
        {
            [MarshalAs(UnmanagedType.Struct)]
            public Head h;
            public ushort code;

            public LoginResponse(ushort code)
            {
                h = new Head((ushort)Marshal.SizeOf(typeof(LoginResponse)), TYPE_LOGIN_RES);
                this.code = code;
            }
        };

        public static void NetworkToHost<T>(T obj)
        {
            Type t = typeof(T);
            foreach (FieldInfo field in t.GetFields())
            {
                Debug.Print(field.Name);
                if (field.FieldType == typeof(ushort))
                {
                    ushort us = (ushort)field.GetValue(obj);
                    field.SetValue(obj, (ushort)IPAddress.NetworkToHostOrder((short)us));
                }
                else if (field.FieldType == typeof(short))
                {
                    short us = (short)field.GetValue(obj);
                    field.SetValue(obj, IPAddress.NetworkToHostOrder(us));
                }
                else if (field.FieldType == typeof(uint))
                {
                    uint us = (uint)field.GetValue(obj);
                    field.SetValue(obj, (uint)IPAddress.NetworkToHostOrder((int)us));
                }
                else if (field.FieldType == typeof(int))
                {
                    int us = (int)field.GetValue(obj);
                    field.SetValue(obj, IPAddress.NetworkToHostOrder(us));
                }
                else if (field.FieldType == typeof(Head))
                {
                    NetworkToHost((Head)field.GetValue(obj));
                }
            }
        }

        public static void HostToNetwork<T>(T obj)
        {
            Type t = typeof(T);
            foreach (FieldInfo field in t.GetFields())
            {
                Debug.Print(field.Name);
                if (field.FieldType == typeof(ushort))
                {
                    ushort us = (ushort)field.GetValue(obj);
                    field.SetValue(obj, (ushort)IPAddress.HostToNetworkOrder((short)us));
                }
                else if (field.FieldType == typeof(short))
                {
                    short us = (short)field.GetValue(obj);
                    field.SetValue(obj, IPAddress.HostToNetworkOrder(us));
                }
                else if (field.FieldType == typeof(uint))
                {
                    uint us = (uint)field.GetValue(obj);
                    field.SetValue(obj, (uint)IPAddress.HostToNetworkOrder((int)us));
                }
                else if (field.FieldType == typeof(int))
                {
                    int us = (int)field.GetValue(obj);
                    field.SetValue(obj, IPAddress.HostToNetworkOrder(us));
                }
                else if (field.FieldType == typeof(Head)) {
                    HostToNetwork((Head)field.GetValue(obj));
                }
            }
        }

        public static object Read(Stream stream)
        {
            byte[] sizeBuffer = new byte[sizeof(ushort)];
            if (Util.SafeRead(stream, sizeBuffer, 0, sizeBuffer.Length) <= 0)
            {
                return null;
            }
            ushort size = (ushort)((sizeBuffer[0] << 8) + sizeBuffer[1]);
            byte[] buffer = new byte[size];
            if (Util.SafeRead(stream, buffer, sizeBuffer.Length, buffer.Length - sizeBuffer.Length) <= 0)
            {
                return null;
            }
            // Array.Copy(sizeBuffer, buffer, sizeBuffer.Length);
            Head head = Util.ByteArrayToStructure<Head>(buffer);
            int offset = 0;
            switch (IPAddress.NetworkToHostOrder(head.type))
            {
                case TYPE_HELLO:
                    Hello packet = Util.ByteArrayToStructure<Hello>(buffer);
                    packet.h.size = (ushort)IPAddress.HostToNetworkOrder((short)size); // fix size
                    // fix message
                    offset = Marshal.SizeOf(typeof(Hello)) - PLACEHOLDER_SIZE;
                    packet.message = Encoding.UTF8.GetString(buffer, offset, buffer.Length - offset - 1);
                    return packet;
                case TYPE_LOGIN_REQ:
                    break;
                case TYPE_LOGIN_RES:
                    break;
                default:
                    break;
            }
            return null;
        }
    }
}